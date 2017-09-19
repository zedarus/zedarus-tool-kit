using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[System.Serializable]
	public class IAPProductData : GameDataModel, IGameDataModel
	{

		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Free")]
		private bool _free = false;

		[SerializeField]
		[DataField("Order")]
		private int _order = 0;

		[SerializeField]
		[DataField("Unity ID")]
		private string _unitID;

		[SerializeField]
		[DataField("AppStore ID")]
		private string _appleID;

		[SerializeField]
		[DataField("GooglePlay ID")]
		private string _googlePlayID;
		#endregion

		#region Initalization
		public IAPProductData() : base() { }
		public IAPProductData(int id) : base(id) { }
		#endregion

		#region Getters
		public bool Enabled
		{
			get { return _enabled; }		
		}

		public bool Free
		{
			get { return _free; }
		}

		public virtual string PriceLocalised
		{
			get 
			{ 
				return "$0.00";
			}
		}

		public int Order
		{
			get { return _order; }
		}
			
		public string UnityID
		{
			get { return _unitID; }
		}

		public string AppleID
		{
			get { return _appleID; }
		}

		public string GoogleID
		{
			get { return _googlePlayID; }
		}
		#endregion
	}
}

