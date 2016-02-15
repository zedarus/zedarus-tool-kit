using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data.Adapters;
#if ZTK_USE_SQL
using SimpleSQL;
#endif

namespace Zedarus.ToolKit.Data.Adapters
{
	public class SQLiteAdapter : IDataAdapter
	{
		public T LoadData<T>(string path) where T : class
		{
			return null;
		}
		
		public bool SaveData<T>(T data, string path) where T : class
		{
			return true;
		}

		#if ZTK_USE_SQL
		private static SimpleSQLManager dbManager = null;

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
		#endif
	}
}
