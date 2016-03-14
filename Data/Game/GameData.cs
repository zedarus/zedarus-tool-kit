using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Data.Game
{
	public class GameData : ScriptableObject
	{
		#region Properties
		[SerializeField] private int _modelsIDCoutner = 0;

		[SerializeField]
		[DataTable(1001, "API Settings", typeof(APISettingsData))]
		private APISettingsData _apiSettings;
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
		protected virtual void Init()
		{
			#if UNITY_EDITOR
			LoadTables();
			#endif
		}
		#endregion

		#region Getters
		public APISettingsData APISettings
		{
			get { return _apiSettings; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		private Dictionary<int, FieldInfo> _tables = new Dictionary<int, FieldInfo>();

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

		private int NextModelID
		{
			get
			{
				int maxID = 0;
				_modelsIDCoutner = 0;

				foreach (KeyValuePair<int, FieldInfo> table in _tables)
				{
					IList list = GetListForTable(table.Key);

					if (list != null)
					{
						foreach (IGameDataModel modelData in list)
						{
							if (modelData != null && modelData.ID > maxID)
								maxID = modelData.ID;
						}
					}
				}

				if (_modelsIDCoutner < maxID)
					_modelsIDCoutner = maxID;

				return ++_modelsIDCoutner;
			}
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
				list.Add(modelData);

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

		private List<string> GetGameModelListNamesForID(int id)
		{
			IList list = GetListForTable(id);

			if (list != null)
			{
				List<string> names = new List<string>();

				foreach (IGameDataModel modelData in list)
				{
					if (modelData != null)
						names.Add(modelData.ListName);
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
					return System.Activator.CreateInstance(table.Type, NextModelID) as IGameDataModel;
				}
			}

			return null;
		}

		public IGameDataModel GetModelDataAt(int tableID, int modelDataIndex)
		{
			IList list = GetListForTable(tableID);
			if (list != null && modelDataIndex >= 0 && modelDataIndex < list.Count)
				return list[modelDataIndex] as IGameDataModel;
			else if (list == null)
				return GetSingleModelForTable(tableID);
			else
				return null;
		}

		public void RemoveModelDataAt(int tableID, int modelDataIndex)
		{
			RemoteItemFromList(GetListForTable(tableID), modelDataIndex);
		}

		public bool IsModelDataAList(int modelID)
		{
			return GetGameModelListNamesForID(modelID) != null;
		}

		public int RenderModelsDataListView(int modelID)
		{
			int selectedModelDataIndex = -1;
			List<string> modelsData = GetGameModelListNamesForID(modelID);
			if (modelsData != null)
			{
				for (int i = 0; i < modelsData.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button(modelsData[i], "box", GUILayout.ExpandWidth(true)))
					{
						selectedModelDataIndex = i;
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
				return field.GetValue(this) as IGameDataModel;
			}
			else
				return null;
		}

		private void RemoteItemFromList(IList list, int index)
		{
			if (list != null && index >= 0 && index < list.Count)
				list.RemoveAt(index);
		}

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
		#endif
	}


}
