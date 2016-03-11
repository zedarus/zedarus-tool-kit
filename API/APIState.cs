using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Player;
#if ZTK_DATA_SERIALIZATION
using Serialization;
#endif

namespace Zedarus.ToolKit.API
{
	public class APIState : IPlayerDataModel
	{
		#region Properties
		#if ZTK_DATA_SERIALIZATION
		[SerializeThis] private bool _adsRemoved;
		[SerializeThis] private int _interstitialsEvents;
		#else
		private bool _adsRemoved;
		private int _interstitialsEvents;
		#endif
		#endregion

		#region Init
		public APIState()
		{
			_adsRemoved = false;
			_interstitialsEvents = 0;
		}
		#endregion

		#region Controls
		public void Reset()
		{
			// TODO: decide if we need to reset this data
			// Reset only properties here!
			//_adsEnabled = false;
			//_adsRemoved = false;
		}

		public void SetDefaults(int currentBuild)
		{
			
		}

		public bool Merge(IPlayerDataModel data)
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
