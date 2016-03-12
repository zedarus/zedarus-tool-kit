using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public struct APIs
	{
		public struct Ads
		{
			public const int HeyZap = 1;
		}

		public struct Score
		{
			public const int GameCenter = 1;
			public const int GooglePlayPlayServices = 2;
		}

		public struct Sharing
		{
			public const int Facebook = 1;
			public const int Twitter = 2;
			public const int Email = 3;
		}

		public struct Analytics
		{
			public const int Unity = 1;
		}

		public struct IAPs
		{
			public const int Unity = 2;
		}

		public struct Sync
		{
			public const int iCloud = 1;
			public const int GooglePlayGameServices = 2;
		}

		public const int None = 0;
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
		public void Init()
		{
			_storeController.Init();
			_socialController.Init();
			_scoreController.Init();
			_syncController.Init();
			_analyticsController.Init();
			_mediationAdsController.Init();
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

		public MediationAdsController Ads
		{
			get { return _mediationAdsController; }
		}
		#endregion
	}
}

