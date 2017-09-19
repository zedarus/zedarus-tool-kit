using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class AchievementConditionData : GameDataModel, IGameDataModel
	{
		#region Settings
		public enum ParameterType
		{
			Int = 1,
			Float = 2,
			CustomCondition = 99,
		}

		public enum ParameterUpdatePolicy
		{
			Override = 1,
			Add = 2,
		}
		#endregion

		#region Properties
		[SerializeField]
		[DataField("Name")]
		private string _name = "Condition";

		[SerializeField]
		[DataField("Parameter Type")]
		private ParameterType _parameterType = ParameterType.Int;

		[SerializeField]
		[DataField("Parameter Update Policy")]
		private ParameterUpdatePolicy _parameterUpdatePolicy = ParameterUpdatePolicy.Add;
		#endregion

		#region Initalization
		public AchievementConditionData() : base() {}
		public AchievementConditionData(int id) : base(id) {}
		#endregion

		#region Getters
		public string Name
		{
			get { return _name; }
		}

		public ParameterType ParamType
		{
			get { return _parameterType; }
		}

		public ParameterUpdatePolicy ParamUpdatePolicy
		{
			get { return _parameterUpdatePolicy; }
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

