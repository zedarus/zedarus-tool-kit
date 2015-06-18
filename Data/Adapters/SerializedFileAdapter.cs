using UnityEngine;
using System.Collections;
using System.IO;
#if !ZTK_DISABLE_SERIALIZATION
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
			T newData = (T) UnitySerializer.DeserializeFromFile<T>(path, true);
			return newData;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			ZedLogger.Log("Saving data to: " + path);
			UnitySerializer.SerializeToFile(data, path, true);
			return true;
		}
	}
	#endif
}
