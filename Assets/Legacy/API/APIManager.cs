using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;

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
			public const int Native = 1;
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

		public struct RemoteData
		{
			public const int HeyZap = 1;
		}

		public struct Promo
		{
			public const int Batch = 1;
		}

		public const int None = 0;
	}
	
	public class APIManager
	{
		#region Properties
		private StoreController _storeController;
		private ShareController _shareController;
		private ScoreController _scoreController;
		private SyncController _syncController;
		private AnalyticsController _analyticsController;
		private AdsController _adsController;
		private RemoteDataController _remoteDataController;
		private PromoController _promoController;

		private APIState _stateModelRef = null;
		private APISettingsData _settingsModelRef = null;
		#endregion
		
		#region Initalization
		public APIManager()
		{
			_storeController = new StoreController();
			_shareController = new ShareController();
			_scoreController = new ScoreController();
			_syncController = new SyncController();
			_analyticsController = new AnalyticsController();
			_adsController = new AdsController();
			_remoteDataController = new RemoteDataController();
			_promoController = new PromoController();
			
			_storeController.Initialized += OnControllerInitialized;
			_shareController.Initialized += OnControllerInitialized;
			_scoreController.Initialized += OnControllerInitialized;
			_syncController.Initialized += OnControllerInitialized;
			_analyticsController.Initialized += OnControllerInitialized;
			_adsController.Initialized += OnControllerInitialized;
			_remoteDataController.Initialized += OnControllerInitialized;
			_promoController.Initialized += OnControllerInitialized;
		}
		#endregion
		
		#region Controls
		public void Init()
		{
			_storeController.Init(this);
			_shareController.Init(this);
			_scoreController.Init(this);
			_syncController.Init(this);
			_analyticsController.Init(this);
			_adsController.Init(this);
			_remoteDataController.Init(this);
			_promoController.Init(this);
		}

		public void UseAPISettingsModel(APISettingsData settingsModel)
		{
			_settingsModelRef = settingsModel;
		}

		public void UseAPIStateModel(APIState stateModel)
		{
			_stateModelRef = stateModel;
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
		
		public ShareController Share
		{
			get { return _shareController; }
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

		public AdsController Ads
		{
			get { return _adsController; }
		}

		public RemoteDataController RemoteData
		{
			get { return _remoteDataController; }
		}

		public PromoController Promo
		{
			get { return _promoController; }
		}

		internal APIState State
		{
			get { return _stateModelRef; }
		}

		internal APISettingsData Settings
		{
			get { return _settingsModelRef; }
		}
		#endregion
	}
}

