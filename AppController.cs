using UnityEngine;
#if API_CRASH_UNITY
using UnityEngine.CrashLog;
#endif
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Audio;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit
{
	public class AppController<GameDataClass, PlayerDataClass> : SimpleSingleton<AppController<GameDataClass, PlayerDataClass>> 
		where GameDataClass : GameData where PlayerDataClass : PlayerData
	{
		#region Properties
		static private bool initialized = false;
		private DataManager<GameDataClass, PlayerDataClass> _data;
		private APIManager _api;
		private AudioController _audio;
		private bool _postInit = false;
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
			InitCrashReporting();
			InitEvents();

			InitGameData();
			InitPlayerData("default.dat");

			// It's important to initalize APIs after data because:
			// - they rely on some data values heavily
			// - remote game data is loaded through APIs, so we need to make sure to load local data first
			InitAPI();
			API.Init();
			
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

			API.RemoteData.RequestData();
			API.Store.RegisterProducts(ProductList);
			API.Sync.Sync();

			if (Data.Player != null && Data.Player.AchievementsTracker != null)
			{
				Data.Player.AchievementsTracker.RestoreAchievements();
			}

			Audio.Init(Data.Player);

			// TODO: pass language saved in player data here
			LocalisationManager.Instance.Init();
		}

		protected virtual void InitCrashReporting()
		{
			#if API_CRASH_UNITY
			CrashReporting.Init(Application.cloudProjectId);
			#endif
		}

		protected virtual void InitEvents()
		{
			IDs.Init();
		}

		protected virtual void InitGameData()
		{
			_data = new DataManager<GameDataClass, PlayerDataClass>();
			_data.LoadGameData();
			API.UseAPISettingsModel(_data.Game.APISettings);
		}

		protected virtual void InitPlayerData(string filename)
		{
			_data.LoadPlayerData(filename);

			APIState state = _data.Player.GetModel<APIState>();
			if (state != null)
			{
				API.UseAPIStateModel(state);
			}

			_data.Player.AchievementsTracker.SetCustomConditionDelegate(OnCheckCustomAchievementCondition);
		}

		protected virtual void InitAPI()
		{
			Data.PlayerDataSaved += OnPlayerDataSaved;
			API.RemoteData.DataReceived += OnRemoteDataReceived;
			API.Store.ProductPurchaseFinished += OnProductPurchaseFinished;
			API.Sync.SyncFinished += OnSyncFinished;
		}
		#endregion

		#region Controls
		public void PurchaseAdsRemoval()
		{
			if (Data.Game.APISettings.AdsEnabled)
			{
				API.Store.Purchase(Data.Game.APISettings.RemoveAdsIAPID, null);
			}
		}
		#endregion

		#region Event Handlers
		protected void OnRemoteDataReceived(string data)
		{
			EventManager.SendEvent<string>(IDs.Events.RemoteDataReceived, data);
		}

		protected virtual void OnProductPurchaseFinished(string productID, bool success)
		{
			if (Data.Game.APISettings.AdsEnabled && productID.Equals(Data.Game.APISettings.RemoveAdsIAPID) && success)
			{
				API.Ads.DisableAds();
				Data.Save(true);
			}
		}

		private void OnPlayerDataSaved(bool sync)
		{
			if (sync)
			{
				API.Sync.SaveData(PlayerData.Serialize<PlayerDataClass>(Data.Player));
			}
		}

		private void OnSyncFinished(byte[] data)
		{
			Data.Player.MergeData(PlayerData.Deserialize<PlayerDataClass>(data));
			Data.Save(false);
		}

		protected virtual bool OnCheckCustomAchievementCondition(int achievement, object parameterValue)
		{
			return false;
		}
		#endregion

		#region Getters
		public DataManager<GameDataClass, PlayerDataClass> Data
		{
			get { return _data; }
		}

		public APIManager API
		{
			get 
			{ 
				if (_api == null)
				{
					_api = new APIManager();
				}

				return _api; 
			}
		}

		public AudioController Audio
		{
			get 
			{
				if (_audio == null)
				{
					_audio = new AudioController();
				}

				return _audio;
			}
		}

		protected virtual StoreProduct[] ProductList
		{
			get 
			{ 
				if (Data.Game.APISettings.AdsEnabled)
				{
					return new StoreProduct[] { new StoreProduct(
						Data.Game.APISettings.RemoveAdsIAPID, 
						Data.Game.APISettings.RemoveAdsIAPAppleID, 
						Data.Game.APISettings.RemoveAdsIAPGoogleID, false)
					};
				}
				else
					return null; 
			}
		}
		#endregion
	}
}
