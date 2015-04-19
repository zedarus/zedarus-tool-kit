using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public enum APIs
	{
		Generic,
		AppleStoreKit,
		AppleGameCenter,
		AppleICloud,
		GoogleCheckout,
		GoogleGameServices,
		Facebook,
		Twitter,
		Email,
		Flurry,
		Chartboost,
		AdMob,
		AppleiAds,
		PlayHaven,
		None
	}
	
	public class APIManager : SimpleSingleton<APIManager>
	{
		#region Enums
		public enum Controllers
		{
			Store,
			Social,
			Score,
			Sync,
			Analytics,
			AdsInterstitials,
			AdsBanners
		}
		#endregion

		#region Properties
		private bool _initStarted;
		private bool _initialized;
		private int _initControllersCount;
		private APISettings _settings;
		private APIState _state;
		private Dictionary<Controllers, APIController> _controllers;
		#endregion
		
		#region Initalization
		private APIManager()
		{
			APIEvents.Register();
			_initStarted = false;
			_initialized = false;
			_initControllersCount = 0;
			_settings = new APISettings();
			_state = new APIState();
			_controllers = new Dictionary<Controllers, APIController>();
		}
		#endregion
		
		#region Controls
		public void Init()
		{
			if (!_initialized)
			{
				if (!_initStarted)
				{
					_initControllersCount = 0;
					_initStarted = true;
					foreach (KeyValuePair<Controllers, APIController> controller in _controllers)
					{
						controller.Value.Initialized += OnControllerInitialized;
						controller.Value.Init();
					}
				} else
					Debug.Log("API manager initalization is already in progress");
			} else
				Debug.Log("API manager already initialized");
		}

		public void AddController(Controllers type, APIController controller)
		{
			if (_initialized || _initStarted)
			{
				Debug.Log("Can't add new API controllers after initialization was started or was finished");
				return;
			}

			if (_controllers.ContainsKey(type))
				Debug.Log("Already has controller for this type: " + type);
			else
				_controllers.Add(type, controller);
		}

		public void UseState(APIState state)
		{
			if (state == null)
			{
				Debug.Log("Can't use new APIState because it's null");
				return;
			}

			_state = state;
		}
		#endregion
		
		#region Event Handlers
		private void OnControllerInitialized()
		{
			_initControllersCount++;
			if (_initControllersCount >= _controllers.Count)
				_initialized = true;
		}
		#endregion
		
		#region Getters
		public APISettings Settings
		{
			get { return _settings; }
		}

		public APIState State
		{
			get { return _state; }
		}

		public StoreController Store
		{
			get { return GetControllerForType<StoreController>(Controllers.Store); }
		}
		
		public SocialController Social
		{
			get { return GetControllerForType<SocialController>(Controllers.Social); }
		}
		
		public ScoreController Score
		{
			get { return GetControllerForType<ScoreController>(Controllers.Score); }	
		}
		
		public SyncController Sync
		{
			get { return GetControllerForType<SyncController>(Controllers.Sync); }
		}
		
		public AnalyticsController Analytics
		{
			get { return GetControllerForType<AnalyticsController>(Controllers.Analytics); }
		}

		public InterstitialsAdsController InterstitialsAds
		{
			get { return GetControllerForType<InterstitialsAdsController>(Controllers.AdsInterstitials); }
		}

		public BannerAdsController BannerAds
		{
			get { return GetControllerForType<BannerAdsController>(Controllers.AdsBanners); }
		}

		public T Controller<T>(Controllers type) where T : APIController
		{
			return GetControllerForType<T>(type);
		}

		private T GetControllerForType<T>(Controllers type) where T : APIController
		{
			if (_controllers.ContainsKey(type))
				return _controllers[type] as T;
			else
				return null;
		}
		#endregion
	}
}

