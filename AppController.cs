using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.Toolkit.Data.Game;

namespace Zedarus.ToolKit
{
	public class AppController<GameDataClass, PlayerDataClass> : SimpleSingleton<AppController<GameDataClass, PlayerDataClass>> 
		where GameDataClass : GameData where PlayerDataClass : PlayerData
	{
		#region Properties
		static private bool initialized = false;
		private DataManager<GameDataClass, PlayerDataClass> _data;
		#endregion

		#region Unity Methods
		public static void Start()
		{
			CreateInstance();
		}

		public AppController()
		{
			if (!initialized)
				Init();

			Launch();
			//Destroy(gameObject);
		}
		#endregion

		#region Init
		private void Init()
		{
			InitEvents();
			InitAPI();
			APIManager.Instance.Init();

			InitGameData();
			InitPlayerData("default.dat");
			
			initialized = true;
		}

		protected virtual void InitEvents()
		{
			
		}

		protected virtual void InitAPI()
		{
			
		}

		protected virtual void InitGameData()
		{
			_data = new DataManager<GameDataClass, PlayerDataClass>();
		}

		protected virtual void InitPlayerData(string filename, APIState customAPIState = null)
		{
			_data.Load(filename);
			/*DataManager.Instance.Load(filename);
			if (customAPIState == null)
			{
				DataManager.Instance.Player.AddModel<APIState>();
				customAPIState = DataManager.Instance.Player.GetModel<APIState>();
			}
			APIManager.Instance.UseState(customAPIState);*/
		}

		protected virtual void Launch()
		{
			
		}
		#endregion

		#region Getters
		public DataManager<GameDataClass, PlayerDataClass> Data
		{
			get { return _data; }
		}
		#endregion
	}
}
