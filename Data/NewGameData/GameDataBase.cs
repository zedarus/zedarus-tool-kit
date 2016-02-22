using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.Toolkit.Data.New.Game
{
	public class GameDataBase : ScriptableObject
	{
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
			
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		private Dictionary<int, string> _models = new Dictionary<int, string>();

		protected void RegisterModel(int id, string name)
		{
			if (!_models.ContainsKey(id))
			{
				_models.Add(id, name);
			}
			else
				Debug.LogError("Model with this ID was already registered");
		}

		public void AddModelData(int modelID, IGameDataModel modelData)
		{
			IList list = GetListForModel(modelID);
			if (list != null)
				list.Add(modelData);
		}

		public virtual string GetModelName(int id)
		{
			if (_models.ContainsKey(id))
				return _models[id];
			else
				return "NONE";
		}

		protected List<string> GetGameModelListNamesForID(int id)
		{
			List<string> names = new List<string>();
			IList list = GetListForModel(id);

			if (list != null)
			{
				foreach (IGameDataModel modelData in GetListForModel(id))
				{
					if (modelData != null)
						names.Add(modelData.ListName);
				}
			}

			return names;
		}

		public int RenderModelsView()
		{
			int selectedModelID = 0;
			foreach (KeyValuePair<int, string> model in _models)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(model.Value, "box", GUILayout.ExpandWidth(true)))
				{
					selectedModelID = model.Key;
				}

				EditorGUILayout.EndHorizontal();
			}
			return selectedModelID;
		}

		public virtual IGameDataModel CreateNewModel(int modelID)
		{
			return null;
		}

		public IGameDataModel GetModelDataAt(int modelID, int modelDataIndex)
		{
			IList list = GetListForModel(modelID);
			if (list != null && modelDataIndex >= 0 && modelDataIndex < list.Count)
				return list[modelDataIndex] as IGameDataModel;
			else
				return null;
		}

		public void RemoveModelDataAt(int modelID, int modelDataIndex)
		{
			RemoteItemFromList(GetListForModel(modelID), modelDataIndex);
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

		protected virtual IList GetListForModel(int modelID)
		{
			return null;
		}

		private void RemoteItemFromList(IList list, int index)
		{
			if (list != null && index >= 0 && index < list.Count)
				list.RemoveAt(index);
		}
		#endregion
		#endif
	}
}
