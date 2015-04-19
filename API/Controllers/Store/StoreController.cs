using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class StoreController : APIController
	{
		#region Parameters
		private List<StoreItem> _items;
		private Queue<int> _queuedPurchases;
		#endregion
		
		#region Events
		// TODO: do we really need these events?
		//public event Action<StoreItem> PurchaseProcessStarted;
		//public event Action<StoreItem, bool> PurchaseProcessFinished;
		public event Action OnDisableAds;
		#endregion
		
		#region Initialization
		public StoreController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}

		protected override void Setup() 
		{
			_items = new List<StoreItem>();
			_queuedPurchases = new Queue<int>();
			
			foreach (KeyValuePair<int, string> storeItem in APIManager.Instance.Settings.StoreItems)
			{
				_items.Add(new StoreItem(storeItem.Key, storeItem.Value));
			}
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
				wrapper.GetProductsListFromServer(_items);
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Generic:
					return GenericStoreWrapper.Instance;
				#if API_STOREKIT_P31
				case APIs.AppleStoreKit:
					return StoreKitWrapper.Instance;
				#endif
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void PurchaseItem(int itemType)
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
		
		public string GetPriceForItem(int itemType)
		{
			StoreItem item = GetItemWithType(itemType);
			if (item != null)
				return item.Price;
			else
				return "--";
		}
		
		public void ShowCoinsShop()
		{
			// TODO: use events to show popup
			//if (GlobalSettings.Instance.IAPEnabled)
			//	PopupManager.Instance.ShowPopupCoinsShop(OnCoinsShopPopupBuyPack, null);
		}

		public void ShowNotEnoughCoinsPopup()
		{
			// TODO: use events to show popup
			//if (GlobalSettings.Instance.IAPEnabled)
			//	PopupManager.Instance.ShowNotEnoughCoinsPopup(null, null);
		}

		public void SimulateRemoveAdsPurchase()
		{
			APIManager.Instance.State.RemoveAds();
			APIManager.Instance.BannerAds.DisableAds();
			DataManager.Instance.Save();
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
		private void OnPurchaseStarted(StoreItem item)
		{
			//if (PurchaseProcessStarted != null)
			//	PurchaseProcessStarted(item);

			// TODO: use events to show popup
			//PopupManager.Instance.ShowProcessingPopup();
		}
		
		private void OnPurchaseFinished(StoreItem item, bool success)
		{
			//if (PurchaseProcessFinished != null)
			//	PurchaseProcessFinished(item, success);

			// TOOD: use events to show popup
			//PopupManager.Instance.ShowTransactionResultPopup(null, success);

			if (success)
			{
				if (item.Type == StoreItemType.RemoveAds)
					SimulateRemoveAdsPurchase();
				else
					EventManager.SendEvent(APIEvents.CurrencyPurchaseSuccess, item.Type);
			}
			
			APIManager.Instance.Analytics.LogPurchase(success, item.ID);
			
			CheckQueue();
		}
		
		private void OnProductsListReceivedFromServer()
		{
			if (Wrapper != null)
			{
				for (int i = 0; i < _items.Count; i++)
				{
					string price = Wrapper.GetLocalisedPriceForItemWithID(_items[i].ID);
					_items[i].UpdatePrice(price);
				}
			}
		}
		
		private void OnCoinsShopPopupBuyPack(int itemType)
		{
			PurchaseItem(itemType);
		}
		#endregion
		
		#region Helpers
		private StoreItem GetItemWithType(int itemType)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].Type == itemType)
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
				int itemType = _queuedPurchases.Dequeue();
				PurchaseItem(itemType);
			}
		}
		#endregion
	}
}
