using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Helpers;

namespace Zedarus.ToolKit.API
{
	public class GenericStoreWrapper : APIWrapper<GenericStoreWrapper>, IStoreWrapperInterface 
	{
		#region Events
		public event Action<StoreItem> PurchaseProcessStarted;
		public event Action<StoreItem, bool> PurchaseProcessFinished;
		public event Action ProductsListReceivedFromServer;
		#endregion
		
		#region Parameters
		private StoreItem _itemToPurchase;
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
		
		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
		
		#region Controls
		public void PurchaseItem(StoreItem item) 
		{
			_itemToPurchase = item;
			
			if (PurchaseProcessStarted != null)
				PurchaseProcessStarted(_itemToPurchase);
			
			SimpleTimer timer = SimpleTimer.CreateTimer();
			timer.OnTimerEnd += SendPurchaseProcessFinishedEvent;
			timer.StartTimer(UnityEngine.Random.Range(2f, 4f));
		}
		
		public void RestorePurchases() {}
		
		public void GetProductsListFromServer(List<StoreItem> products)
		{
			foreach (StoreItem item in products)
				_prices.Add(item.ID, string.Format("${0:n2}", UnityEngine.Random.Range(0.99f, 19.99f)));
			
			SimpleTimer timer = SimpleTimer.CreateTimer();
			timer.OnTimerEnd += SendProductsListReceivedEvent;
			timer.StartTimer(UnityEngine.Random.Range(3f, 5f));
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
		
		#region Event Senders
		private void SendProductsListReceivedEvent()
		{
			_pricesReceived = true;
			
			if (ProductsListReceivedFromServer != null)
				ProductsListReceivedFromServer();
			
			SendInitializedEvent();
		}
		
		private void SendPurchaseProcessFinishedEvent()
		{
			if (PurchaseProcessFinished != null)
				PurchaseProcessFinished(_itemToPurchase, (UnityEngine.Random.Range(0, 2) > 0) ? true : false);
		}
		#endregion
	}
}
