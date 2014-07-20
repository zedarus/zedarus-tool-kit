using UnityEngine;
using System.Collections;
using System.IO;
using Serialization;

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SerializedPersistentFileAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			Debug.Log("Loading data from: " + filepath);
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(filepath, true);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			Debug.Log("Saving data to: " + filepath);
			UnitySerializer.SerializeToFile(data, filepath, true);
			return true;
		}
	}
}
