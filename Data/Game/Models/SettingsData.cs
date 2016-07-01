using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class SettingsData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Version")]
		private string _version = "0.0.0";

		[SerializeField]
		[DataField("Build")]
		private int _build = 0;

		[SerializeField]
		[DataField("AppStore link")]
		private string _appstoreLink = "";

		[SerializeField]
		[DataField("Google Play link")]
		private string _googlePlayLink = "";

		[SerializeField]
		[DataField("AppStore link (short)")]
		private string _appstoreLinkShort = "";

		[SerializeField]
		[DataField("Google Play link (short)")]
		private string _googlePlayLinkShort = "";
		#endregion

		#region Initalization
		public SettingsData() : base() { }
		public SettingsData(int id) : base(id) { }
		#endregion

		#region Controls
		#if UNITY_EDITOR
		public void SetVersion(string version)
		{
			_version = version;
		}

		public void SetBuild(string buildString)
		{
			int build = 0;
			int.TryParse(buildString, out build);
			_build = build;
		}
		#endif
		#endregion

		#region Getters
		public string Version
		{
			get { return _version; }
		}

		public int Build
		{
			get { return _build; }
		}

		public string GetCurrentPlatformStoreLink(bool shortVersion)
		{
			#if UNITY_IPHONE
			return GetAppStoreLink(shortVersion);
			#elif UNITY_ANDROID
			return GetGooglePlayLink(shortVersion);
			#else
			return null;
			#endif
		}

		public string GetAppStoreLink(bool shortVersion)
		{
			if (shortVersion)
				return _appstoreLinkShort;
			else
				return _appstoreLink;
		}

		public string GetGooglePlayLink(bool shortVersion)
		{
			if (shortVersion)
				return _googlePlayLinkShort;
			else
			return _googlePlayLink;
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return "Settings"; }
		}
		#endregion
		#endif
	}
}

