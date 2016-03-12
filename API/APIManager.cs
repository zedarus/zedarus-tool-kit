using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public enum APIs
	{
		AppleStoreKit,
		AppleGameCenter,
		AppleICloud,
		GoogleCheckout,
		GoogleGameServices,
		Facebook,
		Twitter,
		Email,
		HeyZap,
		UnityAnalytics,
		ComboIAPs,
		GoogleStoreKit,
		UnityIAPs,
		None,
	}
	
	public class APIManager : SimpleSingleton<APIManager>
	{
		#region Parameter
		private StoreController _storeController;
		private SocialController _socialController;
		private ScoreController _scoreController;
		private SyncController _syncController;
		private AnalyticsController _analyticsController;
		private MediationAdsController _mediationAdsController;
		#endregion

		#region Properties
		private bool _firstTierInitialized = false;
		private bool _secondTierInitialized = false;
		#endregion
		
		#region Initalization
		private APIManager()
		{
			_storeController = new StoreController();
			_socialController = new SocialController();
			_scoreController = new ScoreController();
			_syncController = new SyncController();
			_analyticsController = new AnalyticsController();
			_mediationAdsController = new MediationAdsController();
			
			_storeController.Initialized += OnControllerInitialized;
			_socialController.Initialized += OnControllerInitialized;
			_scoreController.Initialized += OnControllerInitialized;
			_syncController.Initialized += OnControllerInitialized;
			_analyticsController.Initialized += OnControllerInitialized;
			_mediationAdsController.Initialized += OnControllerInitialized;
		}
		#endregion
		
		#region Controls
		public void InitFirstTier()
		{
			if (!_firstTierInitialized)
			{
				_firstTierInitialized = true;
				// We split initialization because of poor performance on Android
				#if UNITY_ANDROID
				_socialController.Init();
				_scoreController.Init();
				#else
				_storeController.Init();
				_socialController.Init();
				_scoreController.Init();
				_syncController.Init();
				_analyticsController.Init();
				_mediationAdsController.Init();
				#endif
			}
		}

		public void InitSecondTier()
		{
			if (!_secondTierInitialized)
			{
				_secondTierInitialized = true;
				#if UNITY_ANDROID
				_storeController.Init();
				_syncController.Init();
				_analyticsController.Init();
				_interstitialsAdsController.Init();
				_bannersAdsController.Init();
				_mediationAdsController.Init();
				#else
				#endif
			}
		}
		#endregion
		
		#region Event Handlers
		private void OnControllerInitialized()
		{
			//ZedLogger.Log("Controller initialized");
		}
		#endregion
		
		#region Getters
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

		/*public InterstitialsAdsController InterstitialsAds
		{
			get { return _interstitialsAdsController; }
		}

		public BannerAdsController BannerAds
		{
			get { return _bannersAdsController; }
		}*/

		public MediationAdsController AdsMediation
		{
			get { return _mediationAdsController; }
		}
		#endregion
	}
}

