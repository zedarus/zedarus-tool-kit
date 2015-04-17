using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data.Adapters;
using SimpleSQL;

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SQLiteAdapter : IDataAdapter
	{
		private static SimpleSQLManager dbManager = null;

		public T LoadData<T>(string path) where T : class
		{
			//Manager.QueryGeneric();
			return null;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			//ZedLogger.Log("Saving data to: " + path);
			return true;
		}

		static public SimpleSQLManager Manager
		{
			get
			{
				if (SQLiteAdapter.dbManager == null)
				{
					GameObject go = GameObject.Find("DB Manager");
					if (go != null)
						SQLiteAdapter.dbManager = go.GetComponent<SimpleSQLManager>();
				}
				return SQLiteAdapter.dbManager;
			}
		}
	}
}
