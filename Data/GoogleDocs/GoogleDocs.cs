using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Zedarus.ToolKit.Data
{
	public class GoogleDocs
	{
		private const string webServiceUrl = "https://script.google.com/macros/s/AKfycbxuK54xYe2_44rcUV3WOsPO_uvZnkzTtpzX1fj8PzcrwsBAmRTQ/exec";
		private const string spreadsheetId = "1PmtE3iJphKFkNPG24VjFJWRXPvYNJFS1U3KnHC-NXb8";

		public static void Test()	
		{
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add("action", "getAllTables");
			form.Add("ssid", spreadsheetId);

//			UpdateStatus("Establishing Connection at URL " + webServiceUrl);

			UnityWebRequest www = UnityWebRequest.Post(webServiceUrl, form);
			www.Send();

			DateTime startTime = DateTime.UtcNow;

			while (!www.isDone)
			{
				if ((DateTime.UtcNow - startTime).TotalSeconds >= 20.0)
				{
//					ProcessResponse("TIME_OUT");
					break;
				}

			}

//			TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
//			int secondsSinceEpoch = (int)t.TotalSeconds;

			if (www.isError)
			{
//				ProcessResponse(MSG_CONN_ERR + "Connection error after " + elapsedTime.ToString() + " seconds: " + www.error);
//				yield break;
				Debug.Log("Error: " + www.error);
			}
			else
			{
				Debug.Log("Success: " + www.downloadHandler.text);
			}

		}
	}
}
