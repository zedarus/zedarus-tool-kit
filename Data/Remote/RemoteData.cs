using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Data.Game;

namespace Zedarus.ToolKit.Data.Remote
{
	public class RemoteData<GD> where GD : GameData
	{
		public void ProcessRemoteData(string json)
		{
			Debug.Log(JsonUtility.FromJson<GD>(json));
			//GD data = JsonUtility.FromJson(json, typeof(GD));
			//Debug.Log(data.APISettings.IntertitialsDelay);
		}
	}
}
