using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class LeaderboardData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Name")]
		private string _name = "Leaderboard";

		[SerializeField]
		[DataField("Default")]
		private bool _default = false;

		[SerializeField]
		[DataField("Apple ID")]
		private string _appleID = "";

		[SerializeField]
		[DataField("Google Play ID")]
		private string _googleID = "";
		#endregion

		public struct Fields
		{
			public const string ID = "_id";
			public const string Name = "_name";
			public const string Default = "_default";
			public const string AppleID = "_appleID";
			public const string GoogleID = "_googleID";
		}

		#region Initalization
		public LeaderboardData() : base() { }
		public LeaderboardData(int id) : base(id) { }
		#endregion

		#region Getters
		public string Name
		{
			get { return _name; }
		}

		public bool Default
		{
			get { return _default; }
		}

		public string AppleID
		{
			get { return _appleID; }
		}

		public string GoogleID
		{
			get { return _googleID; }
		}

		public string CurrentPlatformID
		{
			get 
			{ 
				#if UNITY_IPHONE
				return AppleID;
				#elif UNITY_ANDROID
				return GoogleID;
				#else
				return null;
				#endif
			}
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
		get { return base.ListName + " " + Name; }
		}
		#endregion
		#endif
	}
}

