using UnityEngine;
using System.Collections;
using System.IO;
#if ZTK_DATA_SERIALIZATION
using Serialization;
#endif

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SerializedResourceAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			ZedLogger.Log("Loading player data from: " + path);
			#if ZTK_DATA_SERIALIZATION
			TextAsset textAsset = Resources.Load<TextAsset>(path);
			T newData = (T) UnitySerializer.Deserialize<T>(textAsset.bytes);
			return newData;
			#else
			return null;
			#endif
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			ZedLogger.Log("Save data for resource adapter is not supported.");
			return false;
		}
	}
}
