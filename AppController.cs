using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Remote;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Settings;

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
			IDs.Init();
		}

		protected virtual void InitAPI()
		{
			
		}

		protected virtual void InitGameData()
		{
			_data = new DataManager<GameDataClass, PlayerDataClass>();
			APIManager.Instance.UseAPISettingsModel(_data.Game.APISettings);
		}

		protected virtual void InitPlayerData(string filename)
		{
			_data.Load(filename);
			_data.Player.AddModel<APIState>();

			APIState state = _data.Player.GetModel<APIState>();
			if (state != null)
			{
				APIManager.Instance.UseAPIStateModel(state);
			}
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
