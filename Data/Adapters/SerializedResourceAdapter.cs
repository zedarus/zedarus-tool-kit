using UnityEngine;
using System.Collections;
using System.IO;
using Serialization;

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SerializedResourceAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			ZedLogger.Log("Loading player data from: " + path);
			TextAsset textAsset = Resources.Load<TextAsset>(path);
			T newData = (T) UnitySerializer.Deserialize<T>(textAsset.bytes);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			ZedLogger.Log("Save data for resource adapter is not supported.");
			return false;
		}
	}
}
