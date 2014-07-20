using UnityEngine;
using System.Collections;
using System.IO;
using Serialization;

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SerializedFileAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			//string filepath = Path.Combine(Application.persistentDataPath, filename);
			Debug.Log("Loading player data from: " + path);
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(path, true);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			//string filepath = Path.Combine(Application.persistentDataPath, filename);
			Debug.Log("Saving player data to: " + path);
			UnitySerializer.SerializeToFile(data, path, true);
			return true;
		}
	}
}
