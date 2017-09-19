using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class Wallet : IPlayerDataModel
	{
		#region Properties
		[OptionalField]
		private List<WalletTransaction> _transactions;

		[OptionalField]
		private int _initialBalance;

		[NonSerialized]
		private int _balanceCache = 0;
		[NonSerialized]
		private bool _cached = false;
		[NonSerialized]
		private int _balanceUpdatedEventID = 0;
		#endregion

		#region Init
		public Wallet() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_initialBalance = 0;
			_transactions = new List<WalletTransaction>();
		}

		public void SetBalanceUpdatedEvent(int eventID)
		{
			_balanceUpdatedEventID = eventID;
		}
		#endregion

		#region Getters
		public int Balance
		{
			get 
			{ 
				if (!_cached)
				{
					_balanceCache = _initialBalance;

					foreach (WalletTransaction transaction in _transactions)
					{
						_balanceCache += transaction.Amount;
					}

					if (_balanceCache < 0)
					{
						_balanceCache = 0;
					}

					_cached = true;
				}

				return _balanceCache; 
			}
		}
		#endregion

		#region Controls
		public void AddInitialBalance(int amount)
		{
			_initialBalance = amount;
			_cached = false;
			SendBalanceUpdatedEvet();
		}

		public void Deposit(int amount)
		{
			_cached = false;
			_transactions.Add(new WalletTransaction(amount));
			SendBalanceUpdatedEvet();
		}

		public bool Withdraw(int amount)
		{
			if (Balance >= amount)
			{
				_cached = false;
				_transactions.Add(new WalletTransaction(-amount));
				SendBalanceUpdatedEvet();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Reset() { }

		public bool Merge(IPlayerDataModel data) 
		{
			Wallet other = (Wallet)data;
			if (other != null)
			{
				for (int i = 0; i < other._transactions.Count; i++)
				{
					WalletTransaction otherTransaction = other._transactions[i];
					WalletTransaction sameTransactionInThisWallet = GetTransactionWithUUID(otherTransaction.UUID);
					if (sameTransactionInThisWallet == null)
					{
						_transactions.Add(otherTransaction);
					}
					else
					{
						sameTransactionInThisWallet.Merge(otherTransaction);
					}
				}

				_cached = false;
				SendBalanceUpdatedEvet();

				return true; 
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Helpers
		private WalletTransaction GetTransactionWithUUID(string uuid)
		{
			foreach (WalletTransaction transaction in _transactions)
			{
				if (transaction.Equals(uuid))
				{
					return transaction;
				}
			}

			return null;
		}

		private void SendBalanceUpdatedEvet()
		{
			if (_balanceUpdatedEventID > 0)
			{
				EventManager.SendEvent<int>(_balanceUpdatedEventID, Balance);
			}		
		}
		#endregion
	}
}
