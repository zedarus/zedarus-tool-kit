using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using System.Collections.Generic;

namespace Zedarus.Toolkit.Data.New.Game
{
	[System.Serializable]
	public class GameDataModelBase : IGameDataModel
	{
		#region Properties
		[SerializeField][GameDataModelField("ID", locked = true)] private int _id;
		#endregion

		#region Initalization
		public GameDataModelBase() : this(0) { }

		public GameDataModelBase(int id)
		{
			_id = id;
		}
		#endregion

		#region Getters
		public int ID
		{
			get { return _id; }
		}
		#endregion

		#if UNITY_EDITOR
		public void RenderForm()
		{
			FieldInfo[] fields = GetFields(this);

			foreach (FieldInfo field in fields)
			{
				object[] attrs = field.GetCustomAttributes(typeof(GameDataModelField), true);
				foreach (object attr in attrs)
				{
					GameDataModelField fieldAttr = attr as GameDataModelField;
					if (fieldAttr != null)
					{
						RenderEditorForField(field, fieldAttr);
					}
				}
			}
		}

		public virtual string ListName { get { return "#" + ID.ToString(); } }

		public void CopyValuesFrom(IGameDataModel data)
		{
			FieldInfo[] fields = GetFields(data);

			foreach (FieldInfo field in fields)
			{
				object[] attrs = field.GetCustomAttributes(typeof(GameDataModelField), true);
				foreach (object attr in attrs)
				{
					GameDataModelField fieldAttr = attr as GameDataModelField;
					if (fieldAttr != null)
					{
						ReplaceValueForFieldInCurrentInstance(field, data);
					}
				}
			}
		}

		#region Helpers
		protected string RenderPrefabField(string label, string value, System.Type type, int previewWidth = 100, int previewHeight = 100)
		{
			EditorGUILayout.BeginHorizontal();

			value = EditorGUILayout.TextField(label, value);

			Object prefabRef = AssetDatabase.LoadAssetAtPath<Object>(string.Concat("Assets/Resources/", value, ".prefab"));
			prefabRef = EditorGUILayout.ObjectField(prefabRef, type, false, GUILayout.MaxWidth(150));
			if (prefabRef != null)
			{
				value = AssetDatabase.GetAssetPath(prefabRef).Replace("Assets/Resources/", "").Replace(".prefab", "");

				GUIContent content = new GUIContent(AssetPreview.GetAssetPreview(prefabRef));
				EditorGUILayout.LabelField(content, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));

				prefabRef = null;
			}
			EditorGUILayout.EndHorizontal();

			return value;
		}

		private void RenderEditorForField(FieldInfo field, GameDataModelField attribute)
		{
			if (attribute.locked)
				GUI.enabled = false;

			if (!attribute.autoRender)
				RenderUnhandledEditorField(field, attribute);
			else if (attribute.customFieldType != GameDataModelField.CustomFieldType.Default)
				RenderCustomEditorForField(field, attribute);
			else if (field.FieldType == typeof(string))
				RenderStringField(field, attribute);
			else if (field.FieldType == typeof(int))
				RenderIntField(field, attribute);
			else if (field.FieldType == typeof(bool))
				RenderBoolField(field, attribute);
			else
				RenderUnhandledEditorField(field, attribute);

			if (attribute.locked)
				GUI.enabled = true;
		}

		private void RenderCustomEditorForField(FieldInfo field, GameDataModelField attribute)
		{
			switch (attribute.customFieldType)
			{
			case GameDataModelField.CustomFieldType.Prefab:
				object value = field.GetValue(this);
				string currentValue = "";
				if (value != null)
					currentValue = value.ToString();
				currentValue = RenderPrefabField(attribute.EditorLabel, currentValue, attribute.customFieldTypeLimit);
				field.SetValue(this, currentValue);
				break;
			}
		}

		protected virtual void RenderUnhandledEditorField(FieldInfo field, GameDataModelField attribute)
		{
			
		}

		private void RenderStringField(FieldInfo field, GameDataModelField attribute)
		{
			object value = field.GetValue(this);
			string currentValue = "";
			if (value != null)
				currentValue = value.ToString();
			
			field.SetValue(this, EditorGUILayout.TextField(attribute.EditorLabel, currentValue));
		}

		private void RenderIntField(FieldInfo field, GameDataModelField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.IntField(attribute.EditorLabel, int.Parse(field.GetValue(this).ToString())));
		}

		private void RenderBoolField(FieldInfo field, GameDataModelField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.Toggle(attribute.EditorLabel, bool.Parse(field.GetValue(this).ToString())));
		}
		#endregion

		private void ReplaceValueForFieldInCurrentInstance(FieldInfo field, object target)
		{
			FieldInfo[] fields = GetFields(this);

			foreach (FieldInfo currentField in fields)
			{
				if (currentField.Equals(field))
				{
					currentField.SetValue(this, field.GetValue(target));
				}
			}
		}

		private FieldInfo[] GetFields(IGameDataModel target)
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
		#endif
	}
}

