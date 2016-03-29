using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Data.Game;
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
		}
		#endregion

		#region Init
		private void Init()
		{
			InitEvents();

			InitGameData();
			InitPlayerData("default.dat");

			// It's important to initalize APIs after data because:
			// - they rely on some data values heavily
			// - remote game data is loaded through APIs, so we need to make sure to load local data first
			InitAPI();
			APIManager.Instance.Init();
			
			initialized = true;
		}

		protected virtual void InitEvents()
		{
			IDs.Init();
		}

		protected virtual void InitGameData()
		{
			_data = new DataManager<GameDataClass, PlayerDataClass>();
			_data.LoadGameData();
			APIManager.Instance.UseAPISettingsModel(_data.Game.APISettings);
		}

		protected virtual void InitPlayerData(string filename)
		{
			_data.LoadPlayerData(filename);
			_data.Player.AddModel<APIState>();

			APIState state = _data.Player.GetModel<APIState>();
			if (state != null)
			{
				APIManager.Instance.UseAPIStateModel(state);
			}
		}

		protected virtual void InitAPI()
		{

		}

		public virtual void Launch()
		{
			if (Data.Player != null)
				Data.Player.PostInit();
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
