using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class PromoLocalNotifications : GameDataModel, IGameDataModel
	{
		#region Settings
		public enum RepeatInterval
		{
			None = 0,
			Daily = 1,
			Weekly = 2,
			Monthly = 3,
			Minute = 5,
			Hourly = 4,
		}

		public const string REWARD_ID = "reward_id";
		#endregion

		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Text")]
		private string _text = "Hello";

		[SerializeField]
		[DataField("Action Name")]
		private string _action = string.Empty;

		[SerializeField]
		[DataField("Minutes From Now")]
		private int _minutesFromNow = 1440;

		[SerializeField]
		[DataField("Use localisation")]
		private bool _useLocalisation = false;

		[SerializeField]
		[DataField("Text Loc ID")]
		private string _textLocID = string.Empty;

		[SerializeField]
		[DataField("Repeat Interval")]
		private RepeatInterval _repeat = RepeatInterval.None;

		[SerializeField]
		[DataField("Reward", foreignKeyForTable=typeof(PromoReward))]
		private int _rewardID;
		#endregion

		#region Initalization
		public PromoLocalNotifications() : base() { }
		public PromoLocalNotifications(int id) : base(id) { }
		#endregion

		#region Getters
		public bool Enabled
		{
			get 
			{
				return _enabled; 
			}
		}

		public string Text
		{
			get 
			{
				return _text;
			}
		}

		public string Action
		{
			get { return _action; }
		}

		public bool UseLocalisation
		{
			get { return _useLocalisation; }
		}

		public string TextLocalisationID
		{
			get { return _textLocID; }		
		}

		public DateTime Date
		{
			get { return DateTime.Now.AddMinutes(_minutesFromNow); }
		}

		public RepeatInterval Repeat
		{
			get { return _repeat; }
		}

		public IDictionary UserInfo
		{
			get { return new Dictionary<string, int>() { { REWARD_ID, _rewardID } }; }		
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return base.ListName + " " + Text; }
		}
		#endregion
		#endif
	}
}

