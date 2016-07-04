using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class WalletTransaction : BasicTransaction
	{
		public WalletTransaction(int amount) : base(amount)
		{
		
		}
	}
}

