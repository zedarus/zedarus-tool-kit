using UnityEngine;
using System.Collections;
using System.IO;
#if ZTK_DATA_SERIALIZATION
using Serialization;
#endif

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SerializedPersistentFileAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			ZedLogger.Log("Loading data from: " + filepath);
			#if ZTK_DATA_SERIALIZATION
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(filepath, true);
			return newData;
			#else
			return null;
			#endif
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			string filepath = Path.Combine(Application.persistentDataPath, path);
			ZedLogger.Log("Saving data to: " + filepath);
			#if ZTK_DATA_SERIALIZATION
			UnitySerializer.SerializeToFile(data, filepath, true);
			return true;
			#else
			return false;
			#endif
		}
	}
}
