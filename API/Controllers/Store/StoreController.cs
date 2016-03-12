using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class StoreController : APIController
	{
		#region Parameters
		private List<StoreItem> _items;
		private Queue<StoreItemType> _queuedPurchases;
		#endregion
		
		#region Events
		// TODO: do we really need these events?
		//public event Action<StoreItem> PurchaseProcessStarted;
		//public event Action<StoreItem, bool> PurchaseProcessFinished;
		public event Action OnDisableAds;
		#endregion
		
		#region Initialization
		protected override void Setup() 
		{
			_items = new List<StoreItem>();
			_queuedPurchases = new Queue<StoreItemType>();

			// TODO: move this to settings class
			_items.Add(new StoreItem(StoreItemType.CoinsPackSmall, "CoinsPackSmall", "KubikoCoinsPackSmall", "com.zedarus.kubiko.coins_pack_small"));
			_items.Add(new StoreItem(StoreItemType.CoinsPackMedium, "CoinsPackMedium", "KubikoCoinsPackMedium", "com.zedarus.kubiko.coins_pack_medium"));
			_items.Add(new StoreItem(StoreItemType.CoinsPackBig, "CoinsPackBig", "KubikoCoinsPackBig", "com.zedarus.kubiko.coins_pack_big"));
			_items.Add(new StoreItem(StoreItemType.RemoveAds, "RemoveAds", "KubikoRemoveAds", "com.zedarus.kubiko.remove_ads"));
		}	
		
		protected override void CompleteInitialization()
		{
			base.CompleteInitialization();
			CheckQueue();
		}
		#endregion
		
		#region Wrappers Initialization
		protected override void InitWrappers() 
		{
			base.InitWrappers();
			foreach (IStoreWrapperInterface wrapper in Wrappers)
			{
				wrapper.GetProductsListFromServer(_items);
			}
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.IAPs.Unity:
					return UnityStoreWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls
		public void PurchaseItem(StoreItemType itemType)
		{
			if (!IsInitialized)
			{
				_queuedPurchases.Enqueue(itemType);
				ZedLogger.LogWarning("Store controller is not yet initized, so purchase is queued");
				return;	
			}
			
			StoreItem item = GetItemWithType(itemType);
			if (item != null)
			{
				if (Wrapper != null) Wrapper.PurchaseItem(item);
			} else
				ZedLogger.LogWarning("Item with type \"" + itemType + "\" not found in the store.");
		}

		public void RestorePurchases() 
		{
			if (Wrapper != null) Wrapper.RestorePurchases();
		}
		
		public string GetPriceForItem(StoreItemType itemType)
		{
			StoreItem item = GetItemWithType(itemType);
			if (item != null)
				return item.FormattedPrice;
			else
				return "--";
		}
		
		public void ShowCoinsShop(LogScreens location, LogButtons button)
		{
			/*if (GlobalSettings.Instance.IAPEnabled)
				PopupManager.Instance.ShowPopupCoinsShop(OnCoinsShopPopupBuyPack, null, location, button);*/
		}

		public void ShowNotEnoughCoinsPopup()
		{
			/*if (GlobalSettings.Instance.IAPEnabled)
				PopupManager.Instance.ShowNotEnoughCoinsPopup(null, null);*/
		}

		public void SimulateRemoveAdsPurchase()
		{
			// TODO: PlayerDataManager.Instance.DisableAds();
			APIManager.Instance.Ads.DisableAds();
			//APIManager.Instance.BannerAds.DisableAds();
			if (OnDisableAds != null)
				OnDisableAds();
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();
			
			foreach (IStoreWrapperInterface wrapper in Wrappers)
			{
				wrapper.PurchaseProcessStarted += OnPurchaseStarted;
				wrapper.PurchaseProcessFinished += OnPurchaseFinished;
				wrapper.ProductsListReceivedFromServer += OnProductsListReceivedFromServer;
			}
		}
		
		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();
			
			foreach (IStoreWrapperInterface wrapper in Wrappers)
			{
				wrapper.PurchaseProcessStarted -= OnPurchaseStarted;
				wrapper.PurchaseProcessFinished -= OnPurchaseFinished;
				wrapper.ProductsListReceivedFromServer -= OnProductsListReceivedFromServer;
			}
		}
		#endregion
		
		#region Event Handlers
		private void OnPurchaseStarted(string itemID)
		{
			//if (PurchaseProcessStarted != null)
			//	PurchaseProcessStarted(item);
			
			// TODO: PopupManager.Instance.ShowProcessingPopup();
		}
		
		private void OnPurchaseFinished(string itemID, bool success)
		{
			//if (PurchaseProcessFinished != null)
			//	PurchaseProcessFinished(item, success);

			StoreItem item = GetItemWithID(itemID);
			// TODO: PopupManager.Instance.ShowTransactionResultPopup(null, success);

			if (success)
			{
				if (item.Type == StoreItemType.RemoveAds)
				{
					SimulateRemoveAdsPurchase();
				}
				//else
					// TODO: PlayerDataManager.Instance.Wallet.AddCoins(GameDataManager.Instance.GetCoinsForPurchase(item.Type), true);
			}
			
			//APIManager.Instance.Analytics.LogPurchase(success, item.ID, item.Price, item.Currency);
			
			CheckQueue();
		}
		
		private void OnProductsListReceivedFromServer()
		{
			if (Wrapper != null)
			{
				for (int i = 0; i < _items.Count; i++)
				{
					decimal price = Wrapper.GetDecimalPriceForItemWithID(_items[i].ID);
					string formattedPrice = Wrapper.GetLocalisedPriceForItemWithID(_items[i].ID);
					string currency = Wrapper.GetCurrencyIDForItemWithID(_items[i].ID);
					_items[i].UpdatePrice(price, formattedPrice, currency);
				}
			}
		}
		
		private void OnCoinsShopPopupBuyPack(StoreItemType itemType)
		{
			PurchaseItem(itemType);
		}
		#endregion
		
		#region Helpers
		private StoreItem GetItemWithType(StoreItemType itemType)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].Type == itemType)
					return _items[i];
			}
			
			return null;
		}

		private StoreItem GetItemWithID(string id)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].ID.Equals(id))
					return _items[i];
			}

			return null;
		}
		#endregion
		
		#region Getters
		protected IStoreWrapperInterface Wrapper
		{
			get { return (IStoreWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
		
		#region Purchases Queue
		private void CheckQueue()
		{
			if (_queuedPurchases.Count > 0)
			{
				StoreItemType itemType = _queuedPurchases.Dequeue();
				PurchaseItem(itemType);
			}
		}
		#endregion
	}
}
