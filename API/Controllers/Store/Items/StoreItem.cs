using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.API
{	
	public class StoreItem
	{
		#region Parameters
		private int _type;
		private string _id;
		private string _price;
		#endregion
		
		#region Initialization
		public StoreItem()
		{
			_type = 0;
			_id = string.Empty;
			_price = "--";
		}
		
		public StoreItem(int type, string id)
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
		public int Type
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
