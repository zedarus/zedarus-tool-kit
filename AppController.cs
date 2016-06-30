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
		private bool _postInit = false;
		private string _cachedRemoteData = null;
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

		public void PostInit()
		{
			if (!_postInit)
			{
				OnPostInit();
				_postInit = true;
			}
		}

		protected virtual void OnPostInit()
		{
			if (Data.Player != null)
			{
				Data.Player.PostInit();
			}

			if (Data.Game != null)
			{
				if (_cachedRemoteData != null)
				{
					Data.Game.ApplyRemoteData(_cachedRemoteData);
					_cachedRemoteData = null;		
				}
			}

			APIManager.Instance.RemoteData.RequestData();
			APIManager.Instance.Store.RegisterProducts(ProductList);
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
			APIManager.Instance.RemoteData.DataReceived += OnRemoteDataReceived;
		}

		protected virtual StoreProduct[] ProductList
		{
			get { return null; }
		}
		#endregion

		#region Event Handlers
		private void OnRemoteDataReceived(string data)
		{
			if (initialized && _postInit)
			{
				Data.Game.ApplyRemoteData(data);
			}
			else
			{
				_cachedRemoteData = data;
			}
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
