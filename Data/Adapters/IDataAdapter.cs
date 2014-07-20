using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Adapters
{
	public interface IDataAdapter
	{
		T LoadData<T>(string path) where T : class;
		bool SaveData<T>(T data, string path) where T : class;
	}
}
