using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
		private APIManager _api;
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

			if (Data.Game != null)
			{
				if (_cachedRemoteData != null)
				{
					Data.Game.ApplyRemoteData(_cachedRemoteData);
					_cachedRemoteData = null;		
				}
			}

			API.RemoteData.RequestData();
			API.Store.RegisterProducts(ProductList);
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
			_data.Player.AddModel<APIState>();

			APIState state = _data.Player.GetModel<APIState>();
			if (state != null)
			{
				API.UseAPIStateModel(state);
			}
		}

		protected virtual void InitAPI()
		{
			API.RemoteData.DataReceived += OnRemoteDataReceived;
			API.Store.ProductPurchaseFinished += OnProductPurchaseFinished;
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

		protected virtual void OnProductPurchaseFinished(string productID, bool success)
		{
			if (Data.Game.APISettings.AdsEnabled && productID.Equals(Data.Game.APISettings.RemoveAdsIAPID) && success)
			{
				API.Ads.DisableAds();
				Data.Save();
			}
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
