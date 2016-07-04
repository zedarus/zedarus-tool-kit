using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class BasicTransaction
	{
		#region Properties
		[SerializeField]
		private string _uuid;
		[SerializeField]
		private int _amount;
		#endregion

		#region Init
		public BasicTransaction(int amount)
		{
			_uuid = Guid.NewGuid().ToString();
			_amount = amount;
		}
		#endregion

		#region Controls
		public void Merge(BasicTransaction otherTransaction)
		{
			_amount = otherTransaction.Amount;
		}
		#endregion

		#region Getters
		public string UUID
		{
			get { return _uuid; }
		}

		public int Amount
		{
			get 
			{ 
				return _amount;
			}
		}
		#endregion
	}
}

