using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Helpers
{
	public class FileDownloader : MonoBehaviour
	{
		static public FileDownloader DownloadTextFile(string url, Dictionary<string, string> parameters, System.Action<string> callback)
		{
			GameObject go = new GameObject("File Downloader");
			FileDownloader downloader = go.AddComponent<FileDownloader>();
			downloader.StartDownloadingCoroutine(url, parameters, callback);
			return downloader;
		}

		public void StartDownloadingCoroutine(string url, Dictionary<string, string> parameters, System.Action<string> callback)
		{
			StartCoroutine(LoadTextFileFromServer(url, parameters, callback));
		}

		private IEnumerator LoadTextFileFromServer(string url, Dictionary<string, string> parameters, System.Action<string> callback)
		{
			// TODO: check if url already has parameters first
			string paramsText = "";
			foreach (KeyValuePair<string, string> param in parameters)
				paramsText += (paramsText.Length > 0 ? "&" : "" ) + WWW.EscapeURL(param.Key) + "=" + WWW.EscapeURL(param.Value);
			if (paramsText.Length > 0) url += "?" + paramsText;

			ZedLogger.Log("FileDownloader: starting text file download from: " + url, LoggerContext.Server);
			WWW www = new WWW(url);
			float elapsedTime = 0f;
			
			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime >= 10f) break;
				yield return null;
			}
			
			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				ZedLogger.Log("FileDownloader: Loading file from server failed: " + www.error, LoggerContext.Server);
				Destroy(gameObject, 1f);
				callback(null);
				yield break;
			}
			
			callback(www.text);
			Destroy(gameObject, 1f);
		}
	}
}
