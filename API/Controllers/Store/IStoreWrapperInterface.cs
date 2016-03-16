using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IStoreWrapperInterface
	{
		#region Events
		event Action<string> PurchaseProcessStarted;
		event Action<string, bool> PurchaseProcessFinished;
		event Action ProductsListReceivedFromServer;
		#endregion
		
		#region Controls
		void PurchaseItem(StoreItem item);
		void RestorePurchases();
		void GetProductsListFromServer(List<StoreItem> products);
		string GetLocalisedPriceForItemWithID(string id);
		string GetCurrencyIDForItemWithID(string id);
		decimal GetDecimalPriceForItemWithID(string id);
		#endregion
	}
}
