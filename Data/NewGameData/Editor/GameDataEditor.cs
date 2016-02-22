using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Zedarus.Toolkit.Data.New.Game
{
	public class GameDataEditor : EditorWindow
	{
		private enum State
		{
			BLANK,
			EDIT,
			ADD
		}

		private State state;

		private const string _windowUIPath = "Window/Zedarus Games/Game Data";
		private int _currentModelID = 0;
		private int _selectedModelDataIndex = -1;

		private GameDataBase _data = null;
		private IGameDataModel _newModel = null;

		private Vector2 _scrollPos;
		private Vector2 _modelsViewScrollPos = Vector2.zero;
		private Vector2 _modelsListViewScrollPos = Vector2.zero;

		//private EnemyData _enemyContainer = new EnemyData();

		[MenuItem(_windowUIPath)]
		public static void Init()
		{
			GameDataEditor window = EditorWindow.GetWindow<GameDataEditor>();
			window.minSize = new Vector2(800, 400);
			window.Show();
		}

		#region Unity Methods
		private void OnEnable()
		{
			state = State.BLANK;
		}

		private void OnGUI()
		{
			RenderUI();
		}
		#endregion

		#region UI Rendering
		private void RenderUI()
		{
			RenderDataBase();

			if (DataLoaded)
			{
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
				RenderModelsView();
				RenderModelsListView();
				RenderEditArea();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void RenderDataBase()
		{
			EditorGUILayout.BeginVertical();
			if (!DataLoaded) EditorGUILayout.LabelField("No database found. Please drag a database instance reference from Project view here");
			_data = EditorGUILayout.ObjectField("Database", _data, typeof(GameDataBase), false) as GameDataBase;
			EditorGUILayout.EndVertical();
		}

		private void RenderModelsView()
		{
			EditorGUILayout.BeginVertical(GUILayout.Width(250));
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Tables");

			_modelsViewScrollPos = EditorGUILayout.BeginScrollView(_modelsViewScrollPos, "box", GUILayout.ExpandHeight(true));
			int modelID = _data.RenderModelsView();

			if (modelID > 0 && modelID != _currentModelID)
			{
				_currentModelID = modelID;
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
		}

		private void RenderModelsListView()
		{
			if (_currentModelID > 0)
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(250));
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(_data.GetModelName(_currentModelID));

				_modelsListViewScrollPos = EditorGUILayout.BeginScrollView(_modelsListViewScrollPos, "box", GUILayout.ExpandHeight(true));
				int modelIndex = _data.RenderModelsDataListView(_currentModelID);
				EditorGUILayout.EndScrollView();

				if (modelIndex > -1)
				{
					IGameDataModel selectedModel = _data.GetModelDataAt(_currentModelID, modelIndex);
					if (selectedModel != null)
					{
						_newModel = _data.CreateNewModel(_currentModelID);
						_newModel.CopyValuesFrom(selectedModel);
						_selectedModelDataIndex = modelIndex;
						state = State.EDIT;
					}
				}

				if (GUILayout.Button("New"))
				{
					_newModel = _data.CreateNewModel(_currentModelID);
					state = State.ADD;
				}

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
			}
		}

		private void RenderEditArea()
		{
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			EditorGUILayout.Space();

			if (state == State.ADD)
			{
				if (_newModel != null)
				{
					if (_newModel.RenderForm("Add"))
					{
						_data.AddModelData(_currentModelID, _newModel);
						GUI.FocusControl(null);
						EditorUtility.SetDirty(_data);
						_newModel = null;
						state = State.BLANK;
					}
				}
			}
			else if (state == State.EDIT)
			{
				if (_newModel != null)
				{
					if (_newModel.RenderForm("Save"))
					{
						GUI.FocusControl(null);
						IGameDataModel model = _data.GetModelDataAt(_currentModelID, _selectedModelDataIndex);
						if (model != null)
						{
							model.CopyValuesFrom(_newModel);
							EditorUtility.SetDirty(_data);
						}
						_newModel = null;
						state = State.BLANK;
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Helpers
		private bool DataLoaded 
		{
			get { return _data != null; }
		}
		#endregion
	}
}
