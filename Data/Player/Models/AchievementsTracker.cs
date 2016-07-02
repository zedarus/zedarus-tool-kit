using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class AchievementsTracker : IPlayerDataModel
	{
		#region Properties
		[SerializeField]
		private Dictionary<int, int> _parametersInt;
		[SerializeField]
		private Dictionary<int, float> _parametersFloat;

		[NonSerialized]
		private GameData _gameDataRef = null;
		#endregion

		#region Init
		public AchievementsTracker() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_parametersInt = new Dictionary<int, int>();
			_parametersFloat = new Dictionary<int, float>();
		}
		#endregion

		#region Getters

		#endregion

		#region Controls
		internal void SetGameDataReference(GameData data)
		{
			_gameDataRef = data;
		}

		public void UpdateConditionParameter<T>(int conditionID, T parameterValue)
		{
			AchievementConditionData condition = _gameDataRef.GetAchievementCondition(conditionID);

			if (condition != null)
			{
				IDictionary parameters = null;

				switch (condition.ParamType)
				{
					case AchievementConditionData.ParameterType.Int:
						parameters = _parametersInt;
						break;
					case AchievementConditionData.ParameterType.Float:
						parameters = _parametersFloat;
						break;
				}

				if (parameters != null)
				{
					if (parameters.Contains(conditionID))
					{
						switch (condition.ParamUpdatePolicy)
						{
							case AchievementConditionData.ParameterUpdatePolicy.Add:
								parameters[conditionID] = Sum(parameters[conditionID], parameterValue);
								break;
							case AchievementConditionData.ParameterUpdatePolicy.Override:
								parameters[conditionID] = parameterValue;
								break;
						}
					}
					else
					{
						parameters.Add(conditionID, parameterValue);
					}

					Debug.Log(string.Format("Condition {0} has new parameter value {1}", condition.Name, parameters[conditionID]));
				}
			}
		}

		private object Sum(object a, object b)
		{
			object c = a;
			if (a is int && b is int)
			{
				c = (int)a + (int)b;
			}
			else if (a is float && b is float)
			{
				c = (float)a + (float)b;
			}
			return c;
		}

		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{
			AchievementsTracker other = (AchievementsTracker)data;

			if (other != null)
			{


				return true; 
			}
			else
			{
				return false;
			}
		}
		#endregion

//		#region Helpers
//		private void CheckAchievementsForCondition(AchievementConditionData condition)
//		{
//
//		}
//		#endregion
	}
}
