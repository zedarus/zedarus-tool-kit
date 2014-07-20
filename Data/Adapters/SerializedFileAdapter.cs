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
			Debug.Log("Loading data from: " + path);
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(path, true);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			Debug.Log("Saving data to: " + path);
			UnitySerializer.SerializeToFile(data, path, true);
			return true;
		}
	}
}
