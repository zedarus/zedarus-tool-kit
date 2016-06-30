using UnityEngine;
#if API_IAP_UNITY
using UnityEngine.Purchasing;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class UnityStoreWrapper : APIWrapper<UnityStoreWrapper>, IStoreWrapperInterface 
	#if API_IAP_UNITY
	, IStoreListener
	#endif
	{
		#region Events
		public event Action<string> PurchaseProcessStarted;
		public event Action<string, bool> PurchaseProcessFinished;
		public event Action ProductsListReceivedFromServer;
		#endregion

		#region Parameters
		private Dictionary<string, string> _formattedPrices;
		private Dictionary<string, string> _currencies;
		private Dictionary<string, decimal> _prices;
		private bool _pricesReceived;

		#if API_IAP_UNITY
		private IStoreController _controller;
		private IExtensionProvider _extensions;
		#endif
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings)
		{
			_formattedPrices = new Dictionary<string, string>();
			_currencies = new Dictionary<string, string>();
			_prices = new Dictionary<string, decimal>();
			_pricesReceived = false;
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion

		#region Controls
		public void Purchase(StoreProduct product)
		{
			if (PurchaseProcessStarted != null)
				PurchaseProcessStarted(product.ID);

			#if API_IAP_UNITY
			_controller.InitiatePurchase(product.ID);
			#endif
		}

		public void RestorePurchases()
		{
			#if API_IAP_UNITY
			_extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
				if (result) {
					// This does not mean anything was restored,
					// merely that the restoration process succeeded.
				} else {
					// Restoration failed.
				}
			});
			#endif
		}

		public void GetProductsListFromServer(StoreProduct[] products)
		{
			#if API_IAP_UNITY
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			ProductType productType;
			foreach (StoreProduct product in products)
			{
				productType = product.Consumable ? ProductType.Consumable : ProductType.NonConsumable;

				builder.AddProduct(product.ID, productType, new IDs
				{
					{ product.GooglePlayID, GooglePlay.Name },
					{ product.AppStoreID, AppleAppStore.Name }
				});
			}

			UnityPurchasing.Initialize(this, builder);
			#endif
		}

		public string GetLocalisedPriceForProductWithID(string id)
		{
			if (_pricesReceived)
			{
				if (_formattedPrices.ContainsKey(id))
					return _formattedPrices [id];
				else
					return "--";
			} else
				return "--";
		}

		public string GetCurrencyIDForProductWithID(string id)
		{
			if (_pricesReceived)
			{
				if (_currencies.ContainsKey(id))
					return _currencies [id];
				else
					return "USD";
			} else
				return "USD";
		}

		public decimal GetDecimalPriceForProductWithID(string id)
		{
			if (_pricesReceived)
			{
				if (_prices.ContainsKey(id))
					return _prices[id];
				else
					return 0.0m;
			} else
				return 0.0m;
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion

		#if API_IAP_UNITY
		#region Event Handlers
		/// <summary>
		/// Called when Unity IAP is ready to make purchases.
		/// </summary>
		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			_controller = controller;
			_extensions = extensions;

			foreach (var product in controller.products.all)
			{
				_formattedPrices.Add(product.definition.id, product.metadata.localizedPriceString);
				_currencies.Add(product.definition.id, product.metadata.isoCurrencyCode);
				_prices.Add(product.definition.id, product.metadata.localizedPrice);
			}

			SendProductsListReceivedEvent();
		}

		/// <summary>
		/// Called when Unity IAP encounters an unrecoverable initialization error.
		///
		/// Note that this will not be called if Internet is unavailable; Unity IAP
		/// will attempt initialization until it becomes available.
		/// </summary>
		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.Log("Unity IAP initialization failed: " + error);
		}

		/// <summary>
		/// Called when a purchase completes.
		///
		/// May be called at any time after OnInitialized().
		/// </summary>
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			SendPurchaseProcessFinishedEvent(e.purchasedProduct.definition.id, true);
			return PurchaseProcessingResult.Complete;
		}

		/// <summary>
		/// Called when a purchase fails.
		/// </summary>
		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			SendPurchaseProcessFinishedEvent(i.definition.id, false);
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

		private void SendPurchaseProcessFinishedEvent(string id, bool result)
		{
			if (PurchaseProcessFinished != null)
				PurchaseProcessFinished(id, result);
		}
		#endregion
	}
}
