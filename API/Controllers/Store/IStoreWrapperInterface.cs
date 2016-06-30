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
		void Purchase(StoreProduct product);
		void RestorePurchases();
		void GetProductsListFromServer(StoreProduct[] products);
		string GetLocalisedPriceForProductWithID(string id);
		string GetCurrencyIDForProductWithID(string id);
		decimal GetDecimalPriceForProductWithID(string id);
		#endregion
	}
}
