using UnityEngine;
using System.Collections;
using System.IO;
#if !ZTK_DISABLE_SERIALIZATION
using Serialization;
#endif

namespace Zedarus.ToolKit.Data.Adapters
{
	#if !ZTK_DISABLE_SERIALIZATION
	public class SerializedPersistentFileAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			ZedLogger.Log("Loading data from: " + filepath);
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(filepath, true);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			ZedLogger.Log("Saving data to: " + filepath);
			UnitySerializer.SerializeToFile(data, filepath, true);
			return true;
		}
	}
	#endif
}
