using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public enum StoreItemType
	{
		CoinsPackSmall = 1,
		CoinsPackMedium = 2,
		CoinsPackBig = 3,
		RemoveAds = 4,
		None = 5,
	}
	
	public class StoreItem
	{
		#region Parameters
		private StoreItemType _type;
		private string _id;
		private string _price;
		#endregion
		
		#region Initialization
		public StoreItem()
		{
			_type = StoreItemType.None;
			_id = string.Empty;
			_price = "--";
		}
		
		public StoreItem(StoreItemType type, string id)
		{
			_type = type;
			_id = id;
			_price = "--";
		}
		#endregion
		
		#region Controls
		public void UpdatePrice(string newPrice)
		{
			_price = newPrice;
		}
		#endregion
		
		#region Getters
		public StoreItemType Type
		{
			get { return _type; }
		}
		
		public string ID
		{
			get { return _id; }
		}
		
		public string Price
		{
			get { return _price; }
		}
		#endregion
	}
}
