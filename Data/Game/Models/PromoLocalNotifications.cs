using UnityEngine;
using System;
using System.Collections;

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
		}
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
			// TODO: implement correct user info here for rewards, etc
			get { return null; }		
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

