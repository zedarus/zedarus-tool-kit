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
		[DataField("Apple ID")]
		private string _appleID = "";

		[SerializeField]
		[DataField("Google Play ID")]
		private string _googleID = "";
		#endregion

		#region Initalization
		public AchievementData() : base() { }
		public AchievementData(int id) : base(id) { }
		#endregion

		#region Getters
		public string Name
		{
			get { return _name; }
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

