﻿using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class RateMePopupData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Events Delay")]
		private int _eventsDelay = 5;

		[SerializeField]
		[DataField("Display Once")]
		private bool _displayOnce = true;

		[SerializeField]
		[DataField("Give Reward First")]
		private bool _rewardFirst = true;

		[SerializeField]
		[DataField("Reward Amount")]
		private int _reward = 100;

		[SerializeField]
		[DataField("AppStore Rating URL")]
		private string _appleURL = ""; 

		[SerializeField]
		[DataField("Google Play Rating URL")]
		private string _googleURL = ""; 
		#endregion

		#region Initalization
		public RateMePopupData() : base() {}
		public RateMePopupData(int id) : base(id) {}
		#endregion

		#region Getters
		public bool Enabled
		{
			get { return _enabled; }
		}

		public int EventsDelay
		{
			get { return _eventsDelay; }
		}

		public bool DisplayOnce
		{
			get { return _displayOnce; }
		}

		public bool RewardFirst
		{
			get { return _rewardFirst; }
		}

		public int Reward
		{
			get { return _reward; }
		}

		public string CurrentPlatformURL
		{
			get
			{
				#if UNITY_IOS
				return _appleURL;
				#elif UNITY_ANDROID
				return _googleURL;
				#else
				return "http://www.bladesliders.com";
				#endif
			}
		}
		#endregion
	}
}

