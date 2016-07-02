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
		[SerializeField]
		private List<int> _unlockedAchievements;

		[NonSerialized]
		private GameData _gameDataRef = null;
		[NonSerialized]
		private Func<int, object, bool> _customConditionDelegate = null;
		#endregion

		#region Events
		public event Action<AchievementData> AchievementUnlocked;
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
			_unlockedAchievements = new List<int>();
		}
		#endregion

		#region Getters

		#endregion

		#region Controls
		internal void SetGameDataReference(GameData data)
		{
			_gameDataRef = data;
		}

		public void SetCustomConditionDelegate(Func<int, object, bool> customConditionDelegate)
		{
			_customConditionDelegate = customConditionDelegate;
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
					case AchievementConditionData.ParameterType.CustomCondition:
						CheckAchivementsForCondition<T>(condition, parameterValue);
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

					CheckAchivementsForCondition<T>(condition, (T)parameters[conditionID]);
				}
			}
		}

		private void CheckAchivementsForCondition<T>(AchievementConditionData condition, T parameterValue)
		{
			foreach (AchievementData achievement in _gameDataRef.Achievements)
			{
				if (achievement.Enabled && achievement.ConditionID.Equals(condition.ID))
				{
					if (!IsAchievementUnlocked(achievement.ID))
					{
						switch (condition.ParamType)
						{
							case AchievementConditionData.ParameterType.Int:
								int paramCurrent = 0;
								int paramTarget = 0;
								if (int.TryParse(parameterValue.ToString(), out paramCurrent) && int.TryParse(achievement.ConditionParameter, out paramTarget))
								{
									if (paramCurrent >= paramTarget)
									{
										UnlockAchievement(achievement);
									}
								}
								break;
							case AchievementConditionData.ParameterType.Float:
								float paramCurrentFloat = 0;
								float paramTargetFloat = 0;
								if (float.TryParse(parameterValue.ToString(), out paramCurrentFloat) && float.TryParse(achievement.ConditionParameter, out paramTargetFloat))
								{
									if (paramCurrentFloat >= paramTargetFloat)
									{
										UnlockAchievement(achievement);
									}
								}
								break;
							case AchievementConditionData.ParameterType.CustomCondition:
								if (_customConditionDelegate != null)
								{
									if (_customConditionDelegate(achievement.ID, parameterValue))
									{
										UnlockAchievement(achievement);
									}
								}
								break;
						}
					}
				}
			}
		}

		private bool IsAchievementUnlocked(int achievementID)
		{
			return _unlockedAchievements.Contains(achievementID);
		}

		private void UnlockAchievement(AchievementData achievement)
		{
			if (!_unlockedAchievements.Contains(achievement.ID))
			{
				_unlockedAchievements.Add(achievement.ID);

				if (AchievementUnlocked != null)
				{
					AchievementUnlocked(achievement);
				}
			}
		}

		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{
			AchievementsTracker other = (AchievementsTracker)data;

			if (other != null)
			{
				foreach (int achivementID in other._unlockedAchievements)
				{
					if (!_unlockedAchievements.Contains(achivementID))
					{
						_unlockedAchievements.Add(achivementID);
					}
				}

				foreach (KeyValuePair<int, int> param in other._parametersInt)
				{
					if (_parametersInt.ContainsKey(param.Key))
					{
						if (_parametersInt[param.Key] < param.Value)
						{
							_parametersInt[param.Key] = param.Value;
						}
					}
					else
					{
						_parametersInt.Add(param.Key, param.Value);
					}
				}

				foreach (KeyValuePair<int, float> param in other._parametersFloat)
				{
					if (_parametersFloat.ContainsKey(param.Key))
					{
						if (_parametersFloat[param.Key] < param.Value)
						{
							_parametersFloat[param.Key] = param.Value;
						}
					}
					else
					{
						_parametersFloat.Add(param.Key, param.Value);
					}
				}

				return true; 
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Helpers
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
		#endregion
	}
}
