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

