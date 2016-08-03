using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class AchievementData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Name")]
		private string _name = "Achievement";

		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Condition", foreignKeyForTable=typeof(AchievementConditionData))]
		private int _conditionID;

		[SerializeField]
		[DataField("Condition Parameters")]
		private string _conditionParameter = string.Empty;

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
			public const string Enabled = "_enabled";
			public const string ConditionID = "_conditionID";
			public const string ConditionParameter = "_conditionParameter";
			public const string AppleID = "_appleID";
			public const string GoogleID = "_googleID";
		}

		#region Initalization
		public AchievementData() : base() { }
		public AchievementData(int id) : base(id) { }
		#endregion

		#region Getters
		public string Name
		{
			get { return _name; }
		}

		public bool Enabled
		{
			get { return _enabled; }
		}

		public int ConditionID
		{
			get { return _conditionID; }
		}

		public string ConditionParameter
		{
			get { return _conditionParameter; }
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

