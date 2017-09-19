using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.API
{	
	public class StoreProduct
	{
		#region Parameters
		private string _id;
		private string _appStoreID;
		private string _googlePlayID;
		private string _formattedPrice;
		private string _currency;
		private decimal _price;
		private bool _consumable = true;
		#endregion

		#region Initialization
		public StoreProduct()
		{
			_id = null;
			_formattedPrice = "--";
			_price = 0.0m;
			_currency = "USD";
		}
		
		public StoreProduct(string id, string appStoreID, string googlePlayID, bool consumable)
		{
			_id = id;
			_appStoreID = appStoreID;
			_googlePlayID = googlePlayID;
			_formattedPrice = "--";
			_price = 0.0m;
			_currency = "USD";
			_consumable = consumable;
		}
		#endregion
		
		#region Controls
		public void UpdatePrice(decimal price, string formattedPrice, string currency)
		{
			_price = price;
			_formattedPrice = formattedPrice;
			_currency = currency;
		}
		#endregion

		#region Getters
		public string ID
		{
			get { return _id; }
		}

		public string AppStoreID
		{
			get { return _appStoreID; }
		}

		public string GooglePlayID
		{
			get { return _googlePlayID; }
		}
		
		public string FormattedPrice
		{
			get { return _formattedPrice; }
		}

		public string Currency
		{
			get { return _currency; }
		}

		public decimal Price
		{
			get { return _price; }	   
		}

		public bool Consumable
		{
			get { return _consumable; }		
		}
		#endregion
	}
}
