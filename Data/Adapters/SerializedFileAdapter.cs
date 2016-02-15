using UnityEngine;
using System.Collections;
using System.IO;
#if ZTK_DATA_SERIALIZATION
using Serialization;
#endif

namespace Zedarus.ToolKit.Data.Adapters
{ 
	#if !ZTK_DISABLE_SERIALIZATION
	public class SerializedFileAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			ZedLogger.Log("Loading data from: " + path);
			#if ZTK_DATA_SERIALIZATION
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(path, true);
			return newData;
			#else
			return null;
			#endif
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			ZedLogger.Log("Saving data to: " + path);
			#if ZTK_DATA_SERIALIZATION
			UnitySerializer.SerializeToFile(data, path, true);
			return true;
			#else
			return false;
			#endif
		}
	}
	#endif
}
