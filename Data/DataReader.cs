using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Data.Adapters;

namespace Zedarus.ToolKit.Data
{
	public class DataReader<DataType, Adapter> where DataType : class where Adapter : IDataAdapter, new()
	{	
		#region Controls
		public DataType Load(string path)
		{
			IDataAdapter adapter = new Adapter();
			if (adapter != null)
				return adapter.LoadData<DataType>(path);
			else
				return null;
		}
		
		public bool Save(DataType data, string path)
		{
			IDataAdapter adapter = new Adapter();
			if (adapter != null)
				return adapter.SaveData<DataType>(data, path);
			else
				return false;
		}
		#endregion
	}
}
