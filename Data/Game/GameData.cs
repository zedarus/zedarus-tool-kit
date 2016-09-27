using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Zedarus.ToolKit.Data.Game
{
	public class GameData : ScriptableObject
	{
		#region Properties
		[SerializeField]
		[DataTable(1001, "Settings", typeof(SettingsData))]
		private SettingsData _settings;

		[SerializeField]
		[DataTable(1002, "API Settings", typeof(APISettingsData))]
		private APISettingsData _apiSettings;

		[SerializeField]
		[DataTable(1003, "Achievement Conditions", typeof(AchievementConditionData))]
		private List<AchievementConditionData> _achievementConditions;

		[SerializeField]
		[DataTable(1004, "Achievements", typeof(AchievementData))]
		private List<AchievementData> _achievements;

		[SerializeField]
		[DataTable(1005, "Leaderboards", typeof(LeaderboardData))]
		private List<LeaderboardData> _leaderboards;

		[SerializeField]
		[DataTable(1007, "Promo - Rewards", typeof(PromoReward))]
		private List<PromoReward> _promoRewards;

		[SerializeField]
		[DataTable(1008, "Promo - Local Notifications", typeof(PromoLocalNotifications))]
		private List<PromoLocalNotifications> _localNotifications;
		#endregion

		#region Settings
		public static string DATABASE_PATH
		{
			get
			{
				return "Assets/Resources/Data/GameData.asset";
			}
		}

		public static string DATABASE_LOCAL_PATH
		{
			get
			{
				return "Data/GameData";
			}
		}
		#endregion

		#region Unity Methods
		private void OnEnable()
		{
			Init();
		}
		#endregion

		#region Initialization
		// We need this method because since 5.3.0 unity no longer
		// supports JsonUtility.FromJson() for ScriptableObjects
		public void ApplyRemoteData(string json)
		{
			float t = Time.realtimeSinceStartup;
			JsonData jasonData = JsonMapper.ToObject(json);

			FieldInfo[] fields = GetFields(this);
			foreach (FieldInfo field in fields)
			{
				if (jasonData.Keys.Contains(field.Name))
				{
					if (field.FieldType.GetInterface("IList") != null)
					{
						DataTable dataField = GetTableAttributeForField(field);
						if (dataField != null)
						{
							IList list = field.GetValue(this) as IList;

							if (list != null)
							{
								for (int i = 0; i < jasonData[field.Name].Count; i++)
								{
									try
									{
										IGameDataModel model = JsonUtility.FromJson(jasonData[field.Name][i].ToJson(), dataField.Type) as IGameDataModel;
										if (model != null)
										{
											bool modelFound = false;
											foreach (IGameDataModel currentModel in list)
											{
												if (currentModel.ID.Equals(model.ID))
												{
													modelFound = true;
													currentModel.OverrideValuesFrom(jasonData[field.Name][i].ToJson());
													break;
												}
											}

											if (!modelFound)
											{
												list.Add(model);
											}

											#if UNITY_EDITOR
											model.SetDataReference(this);
											#endif
										}
									}
									catch (System.Exception e)
									{
										Debug.Log(e.ToString());
									}
								}
							}
						}
					}
					else
					{
						try
						{
							IGameDataModel currentModel = field.GetValue(this) as IGameDataModel;
							currentModel.OverrideValuesFrom(jasonData[field.Name].ToJson());
							#if UNITY_EDITOR
							currentModel.SetDataReference(this);
							#endif
						}
						catch (System.Exception e)
						{
							Debug.Log(e.ToString());
						}
					}
				}
			}

			Debug.Log("Merge Time Elapsed: " + (Time.realtimeSinceStartup - t));
		}

		protected virtual void Init()
		{
			#if UNITY_EDITOR
			LoadTables();
			#endif
		}
		#endregion

		#region Queries
		public GameDataQuery<T> Get<T>() where T : class, IGameDataModel 
		{
			FieldInfo[] fields = GetFields(this);
			List<T> values = new List<T>();

			foreach (FieldInfo field in fields)
			{
				DataTable table = GetTableAttributeForField(field);
				if (table != null && table.Type == typeof(T))
				{
					object value = field.GetValue(this);

					if (value.GetType().IsGenericType)
					{
						values.AddRange(value as List<T>);
					}
					else
					{
						values.Add(value as T);
					}
				}
			}

			GameDataQuery<T> query = new GameDataQuery<T>(values.ToArray());
			values.Clear();
			return query;
		}
		#endregion

		#region Getters
		public APISettingsData APISettings
		{
			get { return _apiSettings; }
		}

		public SettingsData Settings
		{
			get { return _settings; }
		}

		public List<AchievementConditionData> AchivementConditions
		{
			get { return _achievementConditions; }
		}

		public List<AchievementData> Achievements
		{
			get { return _achievements; }
		}

		public List<LeaderboardData> Leaderboards
		{
			get { return _leaderboards; }
		}

		public List<PromoLocalNotifications> LocalNotifications
		{
			get { return _localNotifications; }
		}

		public List<PromoReward> PromoRewards
		{
			get { return _promoRewards; }		
		}

		public LeaderboardData DefaultLeaderboard
		{
			get 
			{ 
				foreach (LeaderboardData leaderboard in Leaderboards)
				{
					if (leaderboard.Default)
					{
						return leaderboard;
					}
				}

				return null; 
			}
		}

		public AchievementConditionData GetAchievementCondition(int id)
		{
			return GetDataTableEntry<AchievementConditionData>(id, AchivementConditions);
		}

		public AchievementData GetAchievement(int id)
		{
			return GetDataTableEntry<AchievementData>(id, Achievements);
		}

		public LeaderboardData GetLeaderboard(int id)
		{
			return GetDataTableEntry<LeaderboardData>(id, Leaderboards);
		}

		public PromoLocalNotifications GetLocalNotification(int id)
		{
			return GetDataTableEntry<PromoLocalNotifications>(id, LocalNotifications);
		}

		public PromoReward GetPromoReward(int id)
		{
			return GetDataTableEntry<PromoReward>(id, PromoRewards);
		}

		protected T GetDataTableEntry<T>(int id, List<T> table) where T : IGameDataModel
		{
			foreach (IGameDataModel entry in table)
			{
				if (entry.ID.Equals(id))
					return (T)entry;
			}

			return default(T);
		}
		#endregion

		#region Helpers
		private FieldInfo[] GetFields(GameData target)
		{
			List<FieldInfo> fields = new List<FieldInfo>();

			fields.AddRange(target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance));

			System.Type baseType = target.GetType().BaseType;

			while (baseType != null)
			{
				fields.InsertRange(0, baseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
				baseType = baseType.BaseType;
			}

			return fields.ToArray();
		}

		private DataTable GetTableAttributeForField(FieldInfo field)
		{
			object[] attrs = field.GetCustomAttributes(typeof(DataTable), true);
			foreach (object attr in attrs)
			{
				DataTable fieldAttr = attr as DataTable;
				if (fieldAttr != null)
				{
					return fieldAttr;
				}
			}

			return null;
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		private Dictionary<int, FieldInfo> _tables = new Dictionary<int, FieldInfo>();
		private int _openModelID = 0;
		private int _openModelDataIndex = 0;

		public void RegisterOpenModelRequest<T>(int id)
		{
			System.Type targetType = typeof(T);
			bool match = false;

			foreach (KeyValuePair<int, FieldInfo> t in _tables)
			{
				if (t.Value.FieldType.Equals(targetType))	// single value fields
				{
					match = true;
				}
				else if (t.Value.FieldType.HasElementType)	// Treat arrays
				{
					match = true;
				}
				else if (t.Value.FieldType.IsGenericType)	// Treat lists and dictionaries
				{
					foreach (System.Type type in t.Value.FieldType.GetGenericArguments())
					{
						if (type.Equals(targetType))
						{
							match = true;
						}
					}
				}

				if (match)
				{
					_openModelID = t.Key;
				 	IList list = GetListForTable(_openModelID);
					if (list != null)
					{
						foreach (IGameDataModel modelData in list)
						{
							if (modelData != null && modelData.ID == id)
							{
								_openModelDataIndex = list.IndexOf(modelData);
							}
						}
					}
					break;
				}
			}
		}

		public bool CheckForOpenModelRequest()
		{
			return _openModelID > 0 && _openModelDataIndex >= 0;
		}

		public int GetOpenModelRequestID()
		{
			return _openModelID;
		}

		public int GetOpenModelRequestDataIndex()
		{
			return _openModelDataIndex;
		}

		public void ResetOpenModelRequest()
		{
			_openModelID = 0;
			_openModelDataIndex = 0;
		}

		public virtual T[] GetModels<T>()
		{
			FieldInfo[] fields = GetFields(this);
			System.Type targetType = typeof(T);
			List<T> models = new List<T>();

			foreach (FieldInfo field in fields)
			{
				if (field.FieldType.Equals(targetType))	// single value fields
				{
					models.Add((T)field.GetValue(this));
				}
				else if (field.FieldType.HasElementType)	// Treat arrays
				{
					if (field.FieldType.GetElementType().Equals(targetType))
					{
//						models.AddRange(field.GetValue(this) as IEnumerable);
					}
				}
				else if (field.FieldType.IsGenericType)	// Treat lists and dictionaries
				{
					foreach (System.Type type in field.FieldType.GetGenericArguments())
					{
						if (type.Equals(targetType))
						{
							models.AddRange(field.GetValue(this) as IEnumerable<T>);
						}
					}
				}
			}

			return models.ToArray();
		}

		private void LoadTables()
		{
			FieldInfo[] fields = GetFields(this);

			foreach (FieldInfo field in fields)
			{
				DataTable attr = GetTableAttributeForField(field);
				if (attr != null)
				{
					if (field.GetValue(this) == null)
					{
						field.SetValue(this, System.Activator.CreateInstance(field.FieldType));
					}

					_tables.Add(attr.ID, field);
				}
			}
		}

		private int GetNextModelID(int modelID)
		{
			int maxID = -1;

			if (_tables.ContainsKey(modelID))
			{
				IList list = GetListForTable(modelID);
				if (list != null)
				{
					foreach (IGameDataModel modelData in list)
					{
						if (modelData != null && modelData.ID > maxID)
							maxID = modelData.ID;
					}
				}
			}

			int newID = 1;

			if (maxID >= 0)
				newID = maxID + Random.Range(1, 32);

			return newID;
		}

		/// <summary>
		/// Adds the model data.
		/// </summary>
		/// <returns>Returns <code>false</code> if model data with this ID already exists.</returns>
		/// <param name="tableID">Model identifier.</param>
		/// <param name="modelData">Model data.</param>
		public bool AddModelData(int tableID, IGameDataModel modelData)
		{
			if (IsModelIDAlreadyInUse(tableID, modelData.ID))
				return false;
			
			IList list = GetListForTable(tableID);

			if (list != null)
			{
				list.Add(modelData);
				modelData.SetDataReference(this);
			}

			return true;
		}

		public bool IsModelIDAlreadyInUse(int tableID, int id)
		{
			IList list = GetListForTable(tableID);

			if (list != null)
			{
				foreach (IGameDataModel existingModelData in list)
				{
					if (existingModelData != null && existingModelData.ID == id)
						return true;
				}
			}

			return false;
		}

		public string GetTableName(int id)
		{
			if (_tables.ContainsKey(id))
			{
				DataTable table = GetTableAttributeForField(_tables[id]);
				if (table != null)
					return table.Name;
			}

			return "NONE";
		}

		private Dictionary<int, string> GetGameModelListNamesForID(int id, bool useFilters)
		{
			IList list = GetListForTable(id);

			if (list != null)
			{
				Dictionary<int, string> names = new Dictionary<int, string>();

				for (int i = 0; i < list.Count; i++)
				{
					IGameDataModel modelData = list[i] as IGameDataModel;
					if (modelData != null)
					{
						if (useFilters)
						{
							if (DoesModelPassFilters(id, modelData))
							{
								names.Add(i, modelData.ListName);
							}
						}
						else
						{
							names.Add(i, modelData.ListName);
						}
					}
				}

				return names;
			}
			else
				return null;
		}

		public int RenderModelsView()
		{
			int selectedModelID = 0;
			foreach (KeyValuePair<int, FieldInfo> table in _tables)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(GetTableName(table.Key), "box", GUILayout.ExpandWidth(true)))
				{
					selectedModelID = table.Key;
				}

				EditorGUILayout.EndHorizontal();
			}
			return selectedModelID;
		}

		public IGameDataModel CreateNewModel(int tableID)
		{
			if (_tables.ContainsKey(tableID))
			{
				DataTable table = GetTableAttributeForField(_tables[tableID]);
				if (table != null)
				{
					IGameDataModel model = System.Activator.CreateInstance(table.Type, GetNextModelID(tableID)) as IGameDataModel;
					model.SetDataReference(this);
					return model;
				}
			}

			return null;
		}

		public IGameDataModel GetModelDataAt(int tableID, int modelDataIndex)
		{
			IList list = GetListForTable(tableID);
			IGameDataModel model = null;

			if (list != null && modelDataIndex >= 0 && modelDataIndex < list.Count)
				model = list[modelDataIndex] as IGameDataModel;
			else if (list == null)
				model = GetSingleModelForTable(tableID);
			else
				model = null;

			if (model != null)
			{
				model.SetDataReference(this);
			}

			return model;
		}

		public void RemoveModelDataAt(int tableID, int modelDataIndex)
		{
			RemoteItemFromList(GetListForTable(tableID), modelDataIndex);
		}

		public bool IsModelDataAList(int modelID)
		{
			return GetGameModelListNamesForID(modelID, false) != null;
		}

		public int RenderModelsDataListView(int modelID)
		{
			int selectedModelDataIndex = -1;
			Dictionary<int, string> modelsData = GetGameModelListNamesForID(modelID, true);

			if (modelsData != null)
			{
				foreach (KeyValuePair<int, string> m in modelsData)
				{
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button(m.Value, "box", GUILayout.ExpandWidth(true)))
					{
						selectedModelDataIndex = m.Key;
					}

					EditorGUILayout.EndHorizontal();
				}
			}
			return selectedModelDataIndex;
		}

		private IList GetListForTable(int tableID)
		{
			if (_tables.ContainsKey(tableID))
			{
				FieldInfo field = _tables[tableID];
				return field.GetValue(this) as IList;
			}
			else
				return null;
		}

		private IGameDataModel GetSingleModelForTable(int tableID)
		{
			if (_tables.ContainsKey(tableID))
			{
				FieldInfo field = _tables[tableID];
				IGameDataModel model = field.GetValue(this) as IGameDataModel;
				model.SetDataReference(this);
				return model;
			}
			else
				return null;
		}

		private void RemoteItemFromList(IList list, int index)
		{
			if (list != null && index >= 0 && index < list.Count)
				list.RemoveAt(index);
		}
		#endregion

		#region Filters
		private Dictionary<int, List<GameDataModelFilter>> _filters = new Dictionary<int, List<GameDataModelFilter>>();

		// TODO: remember filters for each model and switch between them
		public void AddFilter(int modelID)
		{
			if (!_filters.ContainsKey(modelID))
			{
				_filters.Add(modelID, new List<GameDataModelFilter>());
			}

			_filters[modelID].Add(new GameDataModelFilter());
		}

		private bool IsFieldTypeSupportedForFilters(System.Type type)
		{
			return type == typeof(string) ||
				type == typeof(int) ||
				type == typeof(float) ||
				type == typeof(bool);
		}

		private bool DoesModelPassFilters(int modelID, IGameDataModel modelData)
		{
			List<GameDataModelFilter> filters = null;

			if (_filters.ContainsKey(modelID))
			{
				filters = _filters[modelID];
			}
			else
			{
				return true;
			}

			FieldInfo[] fields = GameDataModel.GetFields(modelData.GetType());
			object[] attrs = null;
			foreach (FieldInfo field in fields)
			{
				attrs = field.GetCustomAttributes(typeof(DataField), true);
				foreach (object attr in attrs)
				{
					DataField fieldAttr = attr as DataField;
					if (fieldAttr != null && fieldAttr.useForFiltering)
					{
						object value = field.GetValue(modelData);

						foreach (GameDataModelFilter filter in filters)
						{
							if (filter.PropertyName != null && filter.PropertyName.Equals(field.Name))
							{
								if (field.FieldType == typeof(string))
								{
									if (!filter.CompareToString(value))
									{
										return false;
									}
								}
								else if (field.FieldType == typeof(int))
								{
									if (!filter.CompareToInt(value))
									{
										return false;
									}
								}
								else if (field.FieldType == typeof(float))
								{
									if (!filter.CompareToFloat(value))
									{
										return false;
									}
								}
								else if (field.FieldType == typeof(bool))
								{
									if (!filter.CompareToBool(value))
									{
										return false;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		private bool RenderFilter(int modelID, GameDataModelFilter filter)
		{
			FieldInfo f = _tables[modelID];

			System.Type type = null;

			if (f.FieldType.IsArray)
			{
				type = f.FieldType.GetElementType();
			}
			else if (f.FieldType.IsGenericType)
			{
				type = f.FieldType.GetGenericArguments()[0];
			}
			else
			{
				type = f.FieldType;
			}

			List<FieldInfo> selectedFields = new List<FieldInfo>();
			List<string> selectedFieldsNames = new List<string>();
			int selectionIndex = 0;

			FieldInfo[] fields = GameDataModel.GetFields(type);
			object[] attrs = null;
			foreach (FieldInfo field in fields)
			{
				if (IsFieldTypeSupportedForFilters(field.FieldType))
				{
					attrs = field.GetCustomAttributes(typeof(DataField), true);
					foreach (object attr in attrs)
					{
						DataField fieldAttr = attr as DataField;
						if (fieldAttr != null && fieldAttr.useForFiltering)
						{
							selectedFields.Add(field);
							selectedFieldsNames.Add(fieldAttr.EditorLabel);

							if (field.Name.Equals(filter.PropertyName))
							{
								selectionIndex = selectedFieldsNames.Count - 1;
							}
						}
					}
				}
			}

			EditorGUILayout.BeginHorizontal();

			selectionIndex = EditorGUILayout.Popup(selectionIndex, selectedFieldsNames.ToArray());
			filter.FilterValue = EditorGUILayout.TextField(filter.FilterValue);

			if (GUILayout.Button("-"))
			{
				return true;
			}

			EditorGUILayout.EndHorizontal();

			filter.PropertyName = selectedFields[selectionIndex].Name;

			return false;
		}

		public void RenderFilters(int modelID)
		{
			bool hasFilters = false;
			int removeFilter = -1;

			if (_filters.ContainsKey(modelID))
			{
				hasFilters = _filters[modelID].Count > 0;

				if (hasFilters)
				{
					EditorGUILayout.LabelField("Filters:");
				}
				
				for (int i = 0; i < _filters[modelID].Count; i++)
				{
					if (RenderFilter(modelID, _filters[modelID][i]))
					{
						removeFilter = i;
					}
				}
			}

			if (removeFilter >= 0)
			{
				_filters[modelID].RemoveAt(removeFilter);
			}

			if (GUILayout.Button("Add filter"))
			{
				AddFilter(modelID);
			}

			if (hasFilters)
			{
				EditorGUILayout.Space();
			}
		}
		#endregion
		#endif
	}


}
