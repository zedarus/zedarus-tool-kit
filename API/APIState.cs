using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Player;
using Serialization;

namespace Zedarus.ToolKit.API
{
	public class APIState : PlayerDataModel
	{
		#region Properties
		[SerializeThis] private bool _adsRemoved;
		[SerializeThis] private int _interstitialsEvents;
		#endregion

		#region Init
		public APIState()
		{
			_adsRemoved = false;
			_interstitialsEvents = 0;
		}
		#endregion

		#region Controls
		public override void Reset()
		{
			// TODO: decide if we need to reset this data
			// Reset only properties here!
			//_adsEnabled = false;
			//_adsRemoved = false;
		}

		public override bool Merge(PlayerDataModel data)
		{
			// TODO: decide is this data should be merged on sycn
			if (data is APIState)
			{
				Debug.Log("Merging API state");
			}

			return false;
		}

		public void RegisterInterstitialsEvent()
		{
			_interstitialsEvents++;
		}
		
		public void ResetIntersititalsEvents()
		{
			_interstitialsEvents = 0;
		}
		
		public void RemoveAds()
		{
			_adsRemoved = true;
			// TODO: save
		}
		#endregion

		#region Getters

		public bool AdsRemoved
		{
			get { return _adsRemoved; }
		}
		
		public int InterstitialsEvents
		{
			get { return _interstitialsEvents; }
		}
		#endregion
	}
}
