﻿using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class APIState : IPlayerDataModel
	{
		#region Properties
		[SerializeField]
		private bool _adsEnabled;
		[SerializeField]
		private int _intertitialCounter;
		[SerializeField]
		private DateTime _promoDisplayDate;
		[SerializeField]
		private bool _syncEnabled;
		[SerializeField]
		private bool _firstSync;
		[SerializeField]
		private bool _askedSyncPermission;
		[OptionalField]
		private DateTime _freeAdsRemovalTimestampUTC;
		#endregion

		#region Init
		public APIState() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_firstSync = true;
			_syncEnabled = true;
			_askedSyncPermission = false;
			_adsEnabled = true;
			_intertitialCounter = 0;
			_promoDisplayDate = new DateTime(1986, 7, 21);
			_freeAdsRemovalTimestampUTC = new DateTime(1986, 7, 21);
		}
		#endregion

		#region Controls
		public void IncreaseInterstitialCounter()
		{
			_intertitialCounter++;
		}

		public void ResetInterstitialCounter()
		{
			_intertitialCounter = 0;
		}

		public void DisableAds()
		{
			_adsEnabled = false;
		}

		public void RegisterPromoDisplay()
		{
			_promoDisplayDate = DateTime.UtcNow;
		}

		public void RegisterFreeAdsRemoval()
		{
			_freeAdsRemovalTimestampUTC = DateTime.UtcNow;
		}

		public void ChangeSyncState(bool enabled)
		{
			_syncEnabled = enabled;
		}

		public void AskForSyncPermission()
		{
			_askedSyncPermission = true;
		}

		public void MarkAsFirstSync()
		{
			_firstSync = false;
		}
		#endregion

		#region Getters
		public bool AdsEnabled
		{
			get { return _adsEnabled; }
		}

		public int IntertitialCounter
		{
			get { return _intertitialCounter; }
		}

		public DateTime LastPromoDisplayDate
		{
			get { return _promoDisplayDate; }
		}

		public DateTime LastFreeAdsRemovalDate
		{
			get { return _freeAdsRemovalTimestampUTC; }
		}

		public bool SyncEnabled
		{
			get { return _syncEnabled; }
		}

		public bool AskedSyncPermission
		{
			get { return _askedSyncPermission; }
		}

		public bool FirstSync
		{
			get { return _firstSync; }
		}
		#endregion

		#region Controls
		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{ 
			APIState other = (APIState)data;
			if (other != null)
			{
				_adsEnabled = other.AdsEnabled;
				return true;
			}
			else
			{
				return true; 
			}
		}
		#endregion
	}
}
