using UnityEngine;
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
		#endregion

		#region Init
		public APIState() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_adsEnabled = true;
			_intertitialCounter = 0;
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
