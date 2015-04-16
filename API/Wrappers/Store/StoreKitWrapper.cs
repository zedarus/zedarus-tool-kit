using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	#if API_STOREKIT_P31
	public class StoreKitWrapper : APIWrapper<StoreKitWrapper>, IStoreWrapperInterface 
	{
		#region Events
		public event Action<StoreItem> PurchaseProcessStarted;
		public event Action<StoreItem, bool> PurchaseProcessFinished;
		public event Action ProductsListReceivedFromServer;
		#endregion
		
		#region Parameters
		#if UNITY_IPHONE
		private StoreItem _itemToPurchase;
		#endif
		private Dictionary<string, string> _prices;
		private bool _pricesReceived;
		#endregion
		
		#region Setup
		protected override void Setup() 
		{
			_prices = new Dictionary<string, string>();
			_pricesReceived = false;
		}
		#endregion
		
		#region Controls
		public void PurchaseItem(StoreItem item) 
		{
			#if UNITY_IPHONE
			if (!StoreKitBinding.canMakePayments())
				return;
			
			_itemToPurchase = item;
			
			if (PurchaseProcessStarted != null)
				PurchaseProcessStarted(_itemToPurchase);
			
			StoreKitBinding.purchaseProduct(_itemToPurchase.ID, 1);
			#endif
		}
		
		public void RestorePurchases() 
		{
			#if UNITY_IPHONE
			StoreKitBinding.restoreCompletedTransactions();
			#endif
		}
		
		public void GetProductsListFromServer(List<StoreItem> products)
		{
			#if UNITY_IPHONE
			var productIdentifiers = new string[products.Count];
			for (int i = 0; i < products.Count; i++)
				productIdentifiers[i] = products[i].ID;
			
			StoreKitBinding.requestProductData( productIdentifiers );
			#endif
		}
		
		public string GetLocalisedPriceForItemWithID(string id)
		{
			if (_pricesReceived)
			{
				if (_prices.ContainsKey(id))
					return _prices[id];
				else
					return "--";
			}
			else
				return "--";
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			#if UNITY_IPHONE
			StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
			StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
			StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
			StoreKitManager.purchaseFailedEvent += purchaseFailed;
			StoreKitManager.productListReceivedEvent += productListReceivedEvent;
			StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
			StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailed;
			StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinished;
			StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
			#endif
		}
		
		protected override void RemoveEventListeners() 
		{
			#if UNITY_IPHONE
			StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
			StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
			StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
			StoreKitManager.purchaseFailedEvent -= purchaseFailed;
			StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
			StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
			StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailed;
			StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinished;
			StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
			#endif
		}
		#endregion

		#if UNITY_IPHONE
		#region Event Handlers
		private void productListReceivedEvent(List<StoreKitProduct> productList)
		{
			ZedLogger.Log("productListReceivedEvent. total products received: " + productList.Count);
			
			// print the products to the console
			foreach(StoreKitProduct product in productList)
			{
				ZedLogger.Log(product.ToString() + "\n");
				_prices.Add(product.productIdentifier, product.formattedPrice);
			}
			
			SendProductsListReceivedEvent();
		}
		
		private void productListRequestFailed(string error)
		{
			ZedLogger.Log("productListRequestFailed: " + error);
		}
		
		private void purchaseFailed(string error)
		{
			ZedLogger.Log("purchase failed with error: " + error);
			SendPurchaseProcessFinishedEvent(_itemToPurchase, false);
		}
		
		private void purchaseCancelled(string error)
		{
			ZedLogger.Log("purchase cancelled with error: " + error);
			SendPurchaseProcessFinishedEvent(_itemToPurchase, false);
		}
		
		private void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction)
		{
			ZedLogger.Log("productPurchaseAwaitingConfirmationEvent: " + transaction);
		}
		
		private void purchaseSuccessful(StoreKitTransaction transaction)
		{
			ZedLogger.Log("purchased product: " + transaction);

			StoreItemType type = StoreItemType.None;
			switch (transaction.productIdentifier)
			{
				case "CoinsPackSmall":
				case "CoinsPackSmallFree":
					type = StoreItemType.CoinsPackSmall;
					break;
				case "CoinsPackMedium":
				case "CoinsPackMediumFree":
					type = StoreItemType.CoinsPackMedium;
					break;
				case "CoinsPackBig":
				case "CoinsPackBigFree":
					type = StoreItemType.CoinsPackBig;
					break;
				case "RemoveAds":
					type = StoreItemType.RemoveAds;
					break;
			}

			StoreItem item = new StoreItem(type, transaction.productIdentifier);

			SendPurchaseProcessFinishedEvent(item, true);
		}
		
		private void restoreTransactionsFailed(string error)
		{
			ZedLogger.Log("restoreTransactionsFailed: " + error);
		}
		
		private void restoreTransactionsFinished()
		{
			ZedLogger.Log("restoreTransactionsFinished");
		}
		
		private void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads)
		{
			ZedLogger.Log("paymentQueueUpdatedDownloadsEvent: ");
			foreach(var dl in downloads)
				ZedLogger.Log(dl);
		}
		#endregion
		#endif
		
		#region Event Senders
		private void SendProductsListReceivedEvent()
		{
			_pricesReceived = true;
			
			if (ProductsListReceivedFromServer != null)
				ProductsListReceivedFromServer();
			
			SendInitializedEvent();
		}
		
		private void SendPurchaseProcessFinishedEvent(StoreItem item, bool result)
		{
			#if UNITY_IPHONE
			if (PurchaseProcessFinished != null)
				PurchaseProcessFinished(item, result);
			#endif
		}
		#endregion
	}
	#endif
}
