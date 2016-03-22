using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Collections.Generic;
using LitJson;

namespace Zedarus.ToolKit.Data.Game
{
	[System.Serializable]
	public class GameDataModel : IGameDataModel
	{
		#region Properties
		[SerializeField][DataField("ID", locked = true, renderWhenIncluded = false)] private int _id;
		#endregion

		#region Initalization
		public GameDataModel() : this(0) { }

		public GameDataModel(int id)
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
		#region Helpers - Editor
		public void RenderForm(bool included)
		{
			FieldInfo[] fields = GetFields(this);
			object[] attrs = null;

			int fieldCount = 0;

			foreach (FieldInfo field in fields)
			{
				attrs = field.GetCustomAttributes(typeof(DataGroup), true);
				foreach (object attr in attrs)
				{
					DataGroup fieldAttr = attr as DataGroup;
					if (fieldAttr != null)
					{
						if (fieldCount > 0) EditorGUILayout.Space();
						EditorGUILayout.LabelField(fieldAttr.Title, EditorStyles.boldLabel);
					}
				}

				attrs = field.GetCustomAttributes(typeof(DataField), true);
				foreach (object attr in attrs)
				{
					DataField fieldAttr = attr as DataField;
					if (fieldAttr != null)
					{
						RenderEditorForField(field, fieldAttr, fieldCount, included);
					}
				}

				fieldCount++;
			}
		}

		public virtual string ListName { get { return "#" + ID.ToString(); } }

		protected string RenderPrefabField(string label, string value, System.Type type, bool includePreview, int previewWidth = 100, int previewHeight = 100)
		{
			EditorGUILayout.BeginHorizontal();

			value = EditorGUILayout.TextField(label, value);

			Object prefabRef = AssetDatabase.LoadAssetAtPath<Object>(string.Concat("Assets/Resources/", value, ".prefab"));
			prefabRef = EditorGUILayout.ObjectField(prefabRef, type, false, GUILayout.MaxWidth(150));
			if (prefabRef != null)
			{
				value = AssetDatabase.GetAssetPath(prefabRef).Replace("Assets/Resources/", "").Replace(".prefab", "");

				if (includePreview)
				{
					GUIContent content = new GUIContent(AssetPreview.GetAssetPreview(prefabRef));
					EditorGUILayout.LabelField(content, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
				}

				prefabRef = null;
			}
			EditorGUILayout.EndHorizontal();

			return value;
		}

		protected void RenderPrefabPreview(string path, int width, int height)
		{
			if (path != null)
			{
				Object prefabRef = AssetDatabase.LoadAssetAtPath<Object>(string.Concat("Assets/Resources/", path, ".prefab"));
				GUIContent content = new GUIContent(AssetPreview.GetAssetPreview(prefabRef));
				EditorGUILayout.LabelField(content, GUILayout.Width(width), GUILayout.Height(height));
			}
		}

		private void RenderEditorForField(FieldInfo field, DataField attribute, int fieldCount, bool included)
		{
			if (included && !attribute.renderWhenIncluded)
				return;

			if (attribute.locked)
				GUI.enabled = false;

			if (!attribute.autoRender)
				RenderUnhandledEditorField(field, attribute);
			else if (attribute.customFieldType != DataField.CustomFieldType.Default)
				RenderCustomEditorForField(field, attribute);
			else if (field.FieldType == typeof(string))
				RenderStringField(field, attribute);
			else if (field.FieldType == typeof(int))
				RenderIntField(field, attribute);
			else if (field.FieldType == typeof(float))
				RenderFloatField(field, attribute);
			else if (field.FieldType == typeof(bool))
				RenderBoolField(field, attribute);
			else if (field.FieldType.IsEnum)
				RenderEnumField(field, attribute);
			else if (field.FieldType.GetInterface(typeof(IGameDataModel).Name) != null)
				RenderIGameDataModelField(field, attribute, fieldCount);
			else
				RenderUnhandledEditorField(field, attribute);
			
			ValidateField(field);

			if (attribute.locked)
				GUI.enabled = true;
		}

		private void RenderIGameDataModelField(FieldInfo field, DataField attribute, int fieldCount)
		{
			if (fieldCount > 0) EditorGUILayout.Space();

			//EditorGUILayout.Foldout(true, "hello");

			EditorGUILayout.LabelField(attribute.EditorLabel, EditorStyles.boldLabel);

			IGameDataModel model = field.GetValue(this) as IGameDataModel;
			model.RenderForm(true);

			//EditorGUILayout.EndToggleGroup();
		}

		private void RenderCustomEditorForField(FieldInfo field, DataField attribute)
		{
			switch (attribute.customFieldType)
			{
			case DataField.CustomFieldType.Prefab:
				object value = field.GetValue(this);
				string currentValue = "";
				if (value != null)
					currentValue = value.ToString();
				currentValue = RenderPrefabField(attribute.EditorLabel, currentValue, attribute.customFieldTypeLimit, attribute.customFieldPreview);
				field.SetValue(this, currentValue);
				break;
			}
		}

		protected virtual void RenderUnhandledEditorField(FieldInfo field, DataField attribute)
		{
			
		}

		private void RenderStringField(FieldInfo field, DataField attribute)
		{
			object value = field.GetValue(this);
			string currentValue = "";
			if (value != null)
				currentValue = value.ToString();
			
			field.SetValue(this, EditorGUILayout.TextField(attribute.EditorLabel, currentValue));
		}

		private void RenderIntField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.IntField(attribute.EditorLabel, int.Parse(field.GetValue(this).ToString())));
		}

		private void RenderFloatField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.FloatField(attribute.EditorLabel, float.Parse(field.GetValue(this).ToString())));
		}

		private void RenderBoolField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.Toggle(attribute.EditorLabel, bool.Parse(field.GetValue(this).ToString())));
		}

		private void RenderEnumField(FieldInfo field, DataField attribute)
		{
			object value = field.GetValue(this);
			System.Enum newValue = EditorGUILayout.EnumPopup(attribute.EditorLabel, (System.Enum) System.Enum.Parse(value.GetType(), value.ToString()));
			object uv = System.Convert.ChangeType(newValue, System.Enum.GetUnderlyingType(value.GetType()));
			field.SetValue(this, uv);
		}

		private void ValidateField(FieldInfo field)
		{
			object[] attrs = null;

			attrs = field.GetCustomAttributes(typeof(DataValidateClamp), true);
			foreach (object attr in attrs)
			{
				DataValidateClamp fieldAttr = attr as DataValidateClamp;
				if (fieldAttr != null)
				{
					if (field.FieldType == typeof(int))
						field.SetValue(this, Mathf.Clamp(int.Parse(field.GetValue(this).ToString()), fieldAttr.Min, fieldAttr.Max));
					else if (field.FieldType == typeof(float))
						field.SetValue(this, Mathf.Clamp(float.Parse(field.GetValue(this).ToString()), fieldAttr.MinFloat, fieldAttr.MaxFloat));
				}
			}

			attrs = field.GetCustomAttributes(typeof(DataValidateMin), true);
			foreach (object attr in attrs)
			{
				DataValidateMin fieldAttr = attr as DataValidateMin;
				if (fieldAttr != null)
				{
					if (field.FieldType == typeof(int))
						field.SetValue(this, Mathf.Max(int.Parse(field.GetValue(this).ToString()), fieldAttr.Min));
					else if (field.FieldType == typeof(float))
						field.SetValue(this, Mathf.Max(float.Parse(field.GetValue(this).ToString()), fieldAttr.MinFloat));
				}
			}

			attrs = field.GetCustomAttributes(typeof(DataValidateMax), true);
			foreach (object attr in attrs)
			{
				DataValidateMax fieldAttr = attr as DataValidateMax;
				if (fieldAttr != null)
				{
					if (field.FieldType == typeof(int))
						field.SetValue(this, Mathf.Min(int.Parse(field.GetValue(this).ToString()), fieldAttr.Max));
					else if (field.FieldType == typeof(float))
						field.SetValue(this, Mathf.Min(float.Parse(field.GetValue(this).ToString()), fieldAttr.MaxFloat));
				}
			}
		}

		public void CopyValuesFrom(IGameDataModel data, bool copyID)
		{
			FieldInfo[] fields = GetFields(data);

			foreach (FieldInfo field in fields)
			{
				if (field.Name.Equals("_id") && !copyID)
					continue;

				object[] attrs = field.GetCustomAttributes(typeof(DataField), true);
				foreach (object attr in attrs)
				{
					DataField fieldAttr = attr as DataField;
					if (fieldAttr != null)
					{
						ReplaceValueForFieldInCurrentInstance(field, data);
					}
				}
			}
		}

		private void ReplaceValueForFieldInCurrentInstance(FieldInfo field, object target)
		{
			FieldInfo[] fields = GetFields(this);

			foreach (FieldInfo currentField in fields)
			{
				if (currentField.Equals(field))
				{
					object value = field.GetValue(target);
					currentField.SetValue(this, value);
					ValidateField(currentField);
				}
			}
		}
		#endregion
		#endif

		#region Helpers - Runtime
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

		public virtual void OverrideValuesFrom(string json)
		{
			JsonData data = JsonMapper.ToObject(json);
			FieldInfo[] fields = GetFields(this);
			foreach (string key in data.Keys)
			{
				OverrideField(fields, key, data[key]);
			}
		}

		protected virtual void OverrideField(FieldInfo[] fields, string fieldname, JsonData value)
		{
			//fieldname = fieldname.Trim();
			foreach (FieldInfo field in fields)
			{
				if (field.Name.Equals(fieldname))
				{
					try
					{
						if (value.IsInt || value.IsLong)
							field.SetValue(this, int.Parse(value.ToString()));
						else if (value.IsBoolean)
							field.SetValue(this, bool.Parse(value.ToString()));
						else if (value.IsDouble)
							field.SetValue(this, float.Parse(value.ToString()));
						else if (value.IsString)
							field.SetValue(this, value.ToString());
					}
					catch (System.Exception) { }
					break;
				}
			}
		}
		#endregion
	}
}

