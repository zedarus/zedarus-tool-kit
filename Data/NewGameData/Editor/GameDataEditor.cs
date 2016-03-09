using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Zedarus.Toolkit.Data.New.Game
{
	public class GameDataEditor : EditorWindow
	{
		#region Settings
		private enum State
		{
			Blank = 0,
			Edit = 1,
			Add = 2,
		}

		private const string _windowUIPath = "Window/Zedarus Games/Game Data";
		#endregion

		#region Properties
		private State _state;

		private int _currentModelID = 0;
		private int _selectedModelDataIndex = -1;

		private GameDataBase _data = null;
		private IGameDataModel _model = null;

		private Vector2 _modelsViewScrollPos = Vector2.zero;
		private Vector2 _modelsListViewScrollPos = Vector2.zero;
		private Vector2 _editViewScrollPos = Vector2.zero;
		#endregion

		#region Initialization
		[MenuItem(_windowUIPath)]
		public static void Init()
		{
			GameDataEditor window = EditorWindow.GetWindow<GameDataEditor>();
			window.minSize = new Vector2(800, 400);
			window.Show();
		}
		#endregion

		#region Unity Methods
		private void OnEnable()
		{
			_state = State.Blank;
		}

		private void OnGUI()
		{
			RenderUI();
		}
		#endregion

		#region UI Rendering
		private void RenderUI()
		{
			EditorGUILayout.Space();
			RenderDataBase();

			if (DataLoaded)
			{
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
				RenderModelsView();
				RenderModelsListView();
				RenderEditArea();
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				_currentModelID = 0;
				_selectedModelDataIndex = -1;
				_state = State.Blank;
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
				GUI.FocusControl(null);
				_state = State.Blank;
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
				EditorGUILayout.LabelField(_data.GetTableName(_currentModelID));

				_modelsListViewScrollPos = EditorGUILayout.BeginScrollView(_modelsListViewScrollPos, "box", GUILayout.ExpandHeight(true));
				int modelIndex = _data.RenderModelsDataListView(_currentModelID);
				EditorGUILayout.EndScrollView();

				if (modelIndex > -1)
				{
					IGameDataModel selectedModel = _data.GetModelDataAt(_currentModelID, modelIndex);
					if (selectedModel != null)
					{
						GUI.FocusControl(null);
						_model = _data.CreateNewModel(_currentModelID);
						_model.CopyValuesFrom(selectedModel);
						_selectedModelDataIndex = modelIndex;
						_editViewScrollPos = Vector2.zero;
						_state = State.Edit;
					}
				}

				if (GUILayout.Button("New"))
				{
					GUI.FocusControl(null);
					_model = _data.CreateNewModel(_currentModelID);
					_editViewScrollPos = Vector2.zero;
					_state = State.Add;
				}

				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
			}
		}

		private void RenderEditArea()
		{
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
			EditorGUILayout.Space();
			EditorGUILayout.LabelField( _model != null ? _model.ListName : "" );
			//EditorGUILayout.Space();

			_editViewScrollPos = EditorGUILayout.BeginScrollView(_editViewScrollPos, "box", GUILayout.ExpandHeight(true));

			switch (_state)
			{
			case State.Add:
				RenderModelAdd();
				break;
			case State.Edit:
				RenderModelEdit();
				break;
			default:
				EditorGUILayout.EndScrollView();
				break;
			}


			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
		}

		private void RenderModelAdd()
		{
			if (_model != null)
			{
				//EditorGUILayout.LabelField("");
				//EditorGUILayout.Space();

				_model.RenderForm(false);
				//EditorGUILayout.Space();
				EditorGUILayout.EndScrollView();

				EditorGUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Add", GUILayout.Width(100)))
				{
					GUI.FocusControl(null);

					if (_data.AddModelData(_currentModelID, _model))
					{
						EditorUtility.SetDirty(_data);
						_model = null;
						_state = State.Blank;
					}
					else
					{
						EditorUtility.DisplayDialog("Error", "Model data with this ID already exists", "OK");
					}
				}
				else if (GUILayout.Button("Cancel", GUILayout.Width(100)))
				{
					GUI.FocusControl(null);
					_model = null;
					_state = State.Blank;
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		private void RenderModelEdit()
		{
			if (_model != null)
			{
				//EditorGUILayout.LabelField(_model.ListName);
				//EditorGUILayout.Space();

				_model.RenderForm(false);
				//EditorGUILayout.Space();
				EditorGUILayout.EndScrollView();

				EditorGUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Save", GUILayout.Width(100)))
				{
					GUI.FocusControl(null);
					IGameDataModel model = _data.GetModelDataAt(_currentModelID, _selectedModelDataIndex);

					bool idAlreadyInUse = model.ID != _model.ID && _data.IsModelIDAlreadyInUse(_currentModelID, _model.ID);

					if (!idAlreadyInUse)
					{
						if (model != null)
						{
							model.CopyValuesFrom(_model);
							EditorUtility.SetDirty(_data);
						}
						_model = null;
						_state = State.Blank;
					}
					else
					{
						EditorUtility.DisplayDialog("Error", "Model data with this ID already exists", "OK");
					}
				}
				else if (GUILayout.Button("Cancel", GUILayout.Width(100)))
				{
					GUI.FocusControl(null);
					_model = null;
					_state = State.Blank;
				}
				else if (GUILayout.Button("Delete", GUILayout.Width(100)))
				{
					GUI.FocusControl(null);

					if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete " + _model.ListName + "?", "Yes", "No"))
					{
						_data.RemoveModelDataAt(_currentModelID, _selectedModelDataIndex);
						EditorUtility.SetDirty(_data);
						_model = null;
						_state = State.Blank;
					}
				}

				EditorGUILayout.EndHorizontal();
			}
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
