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
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Extentions;

namespace Zedarus.ToolKit
{
	public class AppController<GameDataClass, PlayerDataClass> : SimpleSingleton<AppController<GameDataClass, PlayerDataClass>> 
		where GameDataClass : GameData where PlayerDataClass : PlayerData
	{
		#region Properties
		static private bool initialized = false;
		static private bool initializing = false;
		private DataManager<GameDataClass, PlayerDataClass> _data;
		private APIManager _api;
		private AudioController _audio;
		private LocalisationManager _localisation;
		private bool _postInit = false;
		private AppStateTracker _appStateTracker;
		private AppHelpers _helpers = null;
		private List<Extention> _extentions = new List<Extention>();
		#endregion

		#region Unity Methods
		public AppController()
		{
			if (!initialized && !initializing)
				Init();
		}
		#endregion

		#region Init
		private void Init()
		{
			initializing = true;

			if (Application.isPlaying)
			{
				GameObject tracker = new GameObject();
				_appStateTracker = tracker.AddComponent<AppStateTracker>();
				_appStateTracker.Init(OnAppBecomesActive, OnAppGoesToBackground);
			}

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

			Localisation.Init();

			InitLocalNotifications();
			InitExtentions();
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
			EventManager.AddListener<string>(IDs.Events.DisplayAdPlacement, OnDisplayAdPlacement);
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
			API.Sync.RequestSyncEnable += OnRequestSyncEnable;
			API.Promo.ProcessRewardFromLocalNotification += OnProcessRewardFromLocalNotification;
			API.Promo.ProcessRemoteUnlockFeature += OnProcessRemoteUnlockFeature;
			API.Promo.ProcessRemoteUnlockResource += OnProcessRemoteUnlockResource;
			API.Promo.ProcessRemoteUnlockParams += OnProcessRemoteUnlockParams;
		}

		protected virtual void InitExtentions()
		{
			
		}

		protected virtual bool DisplaySyncConfirmUI(System.Action syncConfirmedHandler, System.Action syncDeniedHandler)
		{
			return true;
		}

		private void InitLocalNotifications()
		{
			API.Promo.ClearLocalNotifications();

			if (Data.Game.APISettings.LocalNotificationsEnabled)
			{
				foreach (PromoLocalNotifications notif in Data.Game.LocalNotifications)
				{
					if (notif.Enabled)
					{
						string text = notif.Text;
						if (notif.UseLocalisation)
						{
							text = Localisation.Localise(notif.TextLocalisationID);
						}
						API.Promo.ScheduleLocalNotification(text, notif.Action, notif.Date, notif.Repeat, notif.UserInfo);
					}
				}
			}
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
			InitLocalNotifications();
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
			EventManager.SendEvent(IDs.Events.CloudSyncFinished);
		}

		private void OnRequestSyncEnable()
		{
			if (!DisplaySyncConfirmUI(OnSyncConfirmed, OnSyncDenied))
			{
				DelayedCall.Create(OnRequestSyncEnable, 0.5f, true, true, "Ask for sync delay");
			}
		}

		private void OnSyncConfirmed()
		{
			API.Sync.AllowSync();
			API.Sync.ApplyLoadedData();
		}

		private void OnSyncDenied()
		{
			API.Sync.DenySync();
		}

		protected virtual bool OnCheckCustomAchievementCondition(int achievement, object parameterValue)
		{
			return false;
		}

		protected virtual void OnProcessRewardFromLocalNotification(int rewardID)
		{
		
		}

		protected virtual void OnProcessRemoteUnlockFeature(string feature, string value)
		{
		}

		protected virtual void OnProcessRemoteUnlockResource(string resource, int quantity)
		{
		}

		protected virtual void OnProcessRemoteUnlockParams(Dictionary<string, string> parameters)
		{
		}

		protected virtual void OnAppBecomesActive()
		{
			InitLocalNotifications();
		}

		protected virtual void OnAppGoesToBackground()
		{
		
		}

		private void OnDisplayAdPlacement(string placement)
		{
			API.Ads.ShowIntersitital(placement, null);
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

		public LocalisationManager Localisation
		{
			get 
			{
				if (_localisation == null)
				{
					_localisation = new LocalisationManager();
				}

				return _localisation;
			}
		}

		public AppHelpers Helpers
		{
			get 
			{
				if (_helpers == null)
				{
					_helpers = new AppHelpers(Data.Game, API);
				}

				return _helpers;
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

		#region Extentions
		protected void AddExtention(Extention extention)
		{
			if (!_extentions.Contains(extention))
			{
				_extentions.Add(extention);
			}
		}

		public T GetExtention<T>() where T : Extention
		{
			// TODO: cache

			foreach (Extention extention in _extentions)
			{
				if (extention.GetType().Equals(typeof(T)))
				{
					return extention as T;
				}
			}

			return null;
		}
		#endregion
	}

	public class AppStateTracker : MonoBehaviour
	{
		private System.Action _appBecomesActiveCallback = null;
		private System.Action _appGoesToBackgroundCallback = null;

		public void Init(System.Action appBecomesActiveCallback, System.Action appGoesToBackgroundCallback)
		{
			_appBecomesActiveCallback = appBecomesActiveCallback;
			_appGoesToBackgroundCallback = appGoesToBackgroundCallback;
			gameObject.name = "App State Tracker";

			if (Application.isPlaying)
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause && _appGoesToBackgroundCallback != null)
			{
				_appGoesToBackgroundCallback();
			}
			else if (_appBecomesActiveCallback != null)
			{
				_appBecomesActiveCallback();
			}
		}
	}
}
