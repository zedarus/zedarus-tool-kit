using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Helpers
{
	public class FileDownloader : MonoBehaviour
	{
		static public FileDownloader DownloadTextFile(string url, Dictionary<string, string> parameters, System.Action<string> callback, bool escape = true)
		{
			GameObject go = new GameObject("File Downloader");
			FileDownloader downloader = go.AddComponent<FileDownloader>();
			downloader.StartDownloadingCoroutine(url, parameters, callback, escape);
			return downloader;
		}

		static public FileDownloader PostJSON(string url, string json, Dictionary<string, string> parameters, System.Action<string> callback, bool escape = true)
		{
			GameObject go = new GameObject("File Uploader");
			FileDownloader downloader = go.AddComponent<FileDownloader>();
			downloader.StartUploadingCoroutine(url, json, parameters, callback, escape);
			return downloader;
		}

		public void StartDownloadingCoroutine(string url, Dictionary<string, string> parameters, System.Action<string> callback, bool escape)
		{
			StartCoroutine(LoadTextFileFromServer(url, parameters, callback, escape));
		}

		public void StartUploadingCoroutine(string url, string json, Dictionary<string, string> parameters, System.Action<string> callback, bool escape)
		{
			StartCoroutine(UploadJsonToServer(url, json, parameters, callback, escape));
		}

		private IEnumerator LoadTextFileFromServer(string url, Dictionary<string, string> parameters, System.Action<string> callback, bool escape)
		{
			// TODO: check if url already has parameters first
			string paramsText = "";
			foreach (KeyValuePair<string, string> param in parameters)
			{
				if (escape)
					paramsText += (paramsText.Length > 0 ? "&" : "" ) + WWW.EscapeURL(param.Key) + "=" + WWW.EscapeURL(param.Value);
				else
					paramsText += (paramsText.Length > 0 ? "&" : "" ) + param.Key + "=" + param.Value;
			}
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
				ZedLogger.Log("FileDownloader: loading file from server failed: " + www.error, LoggerContext.Server);
				Destroy(gameObject, 1f);
				callback(null);
				yield break;
			}
			
			callback(www.text);
			Destroy(gameObject, 1f);
		}

		private IEnumerator UploadJsonToServer(string url, string json, Dictionary<string, string> parameters, System.Action<string> callback, bool escape)
		{
			string paramsText = "";
			foreach (KeyValuePair<string, string> param in parameters)
			{
				if (escape)
					paramsText += (paramsText.Length > 0 ? "&" : "" ) + WWW.EscapeURL(param.Key) + "=" + WWW.EscapeURL(param.Value);
				else
					paramsText += (paramsText.Length > 0 ? "&" : "" ) + param.Key + "=" + param.Value;
			}
			if (paramsText.Length > 0) url += "?" + paramsText;
			
			ZedLogger.Log("FileDownloader: starting posting JSON to server: " + url, LoggerContext.Server);

			WWWForm form = new WWWForm();
			form.AddField("data", json);

			WWW www = new WWW(url, form);
			float elapsedTime = 0f;

			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime >= 10f) break;
				yield return null;
			}
			
			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				ZedLogger.Log("FileDownloader: posting JSON to server failed: " + www.error, LoggerContext.Server);
				Destroy(gameObject, 1f);
				callback(null);
				yield break;
			}
			
			callback(www.text);
			Destroy(gameObject, 1f);
		}
	}
}
