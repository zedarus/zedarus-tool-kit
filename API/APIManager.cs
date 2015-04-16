using UnityEngine;
using System.Collections;
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
		#region Properties
		private APISettings _settings;
		private StoreController _storeController;
		private SocialController _socialController;
		private ScoreController _scoreController;
		private SyncController _syncController;
		private AnalyticsController _analyticsController;
		private InterstitialsAdsController _interstitialsAdsController;
		private BannerAdsController _bannersAdsController;
		#endregion
		
		#region Initalization
		private APIManager()
		{
			_settings = new APISettings();

			_storeController = new StoreController();
			_socialController = new SocialController();
			_scoreController = new ScoreController();
			_syncController = new SyncController();
			_analyticsController = new AnalyticsController();
			_interstitialsAdsController = new InterstitialsAdsController();
			_bannersAdsController = new BannerAdsController();
			
			_storeController.Initialized += OnControllerInitialized;
			_socialController.Initialized += OnControllerInitialized;
			_scoreController.Initialized += OnControllerInitialized;
			_syncController.Initialized += OnControllerInitialized;
			_analyticsController.Initialized += OnControllerInitialized;
			_interstitialsAdsController.Initialized += OnControllerInitialized;
			_bannersAdsController.Initialized += OnControllerInitialized;
		}
		#endregion
		
		#region Controls
		public void Init()
		{
			_storeController.Init();
			_socialController.Init();
			_scoreController.Init();
			_syncController.Init();
			_analyticsController.Init();
			_interstitialsAdsController.Init();
			_bannersAdsController.Init();
		}
		#endregion
		
		#region Event Handlers
		private void OnControllerInitialized()
		{

		}
		#endregion
		
		#region Getters
		public APISettings Settings
		{
			get { return _settings; }
		}

		public StoreController Store
		{
			get { return _storeController; }
		}
		
		public SocialController Social
		{
			get { return _socialController; }
		}
		
		public ScoreController Score
		{
			get { return _scoreController; }	
		}
		
		public SyncController Sync
		{
			get { return _syncController; }
		}
		
		public AnalyticsController Analytics
		{
			get { return _analyticsController; }
		}

		public InterstitialsAdsController InterstitialsAds
		{
			get { return _interstitialsAdsController; }
		}

		public BannerAdsController BannerAds
		{
			get { return _bannersAdsController; }
		}
		#endregion
	}
}

