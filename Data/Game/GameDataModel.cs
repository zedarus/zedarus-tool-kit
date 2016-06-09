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
		private GameData _dataReference;

		protected GameData DataRererence
		{
			get { return _dataReference; }
		}

		public void SetDataReference(GameData dataReference)
		{
			_dataReference = dataReference;

			FieldInfo[] fields = GetFields(this);
			foreach (FieldInfo field in fields)
			{
				if (field.FieldType.GetInterface(typeof(IGameDataModel).Name) != null)
				{
					IGameDataModel model = field.GetValue(this) as IGameDataModel;
					if (model != null)
					{
						model.SetDataReference(dataReference);
					}
				}
			}
		}

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
			else if (attribute.foreignKeyForTable != null)
			{
				MethodInfo method = this.GetType().GetMethod("RenderForeignKeyField", BindingFlags.Instance | BindingFlags.NonPublic);
				method = method.MakeGenericMethod(attribute.foreignKeyForTable);
				method.Invoke(this, new object[] { field, attribute });
			}
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
			else if (field.FieldType.IsArray)
				RenderArrayField(field, attribute);
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

		protected void RenderForeignKeyField<T>(FieldInfo field, DataField attribute) where T : IGameDataModel
		{
			T[] allValues = DataRererence.GetModels<T>();

			List<string> strings = new List<string>();
			List<int> values = new List<int>();

			foreach (T value in allValues)
			{
				strings.Add(value.ListName);
				values.Add(value.ID);
			}

			if (field.FieldType.IsArray)
			{
				if (field.FieldType.GetElementType().Equals(typeof(int)))
				{
					RenderArrayField(field, attribute, strings.ToArray(), values.ToArray());
				}
				else
				{
					Debug.LogError("Only int fields are current supported as foreign keys");
				}
			}
			else
			{
				if (field.FieldType.Equals(typeof(int)))
				{
					int currentID = 0;
					int.TryParse(field.GetValue(this).ToString(), out currentID);

					field.SetValue(this, EditorGUILayout.IntPopup(attribute.EditorLabel, currentID, strings.ToArray(), values.ToArray()));
				}
				else
				{
					Debug.LogError("Only int fields are current supported as foreign keys");
				}
			}
		}

		protected void RenderArrayField(FieldInfo field, DataField attribute, string[] options = null, int[] values = null)
		{
			System.Array array = field.GetValue(this) as System.Array;
			System.Type arrayElementType = field.FieldType.GetElementType();
			var listType = typeof(List<>).MakeGenericType(arrayElementType);
			IList list = (IList)System.Activator.CreateInstance(listType);

			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array.GetValue(i));
			}

			if (list.Count == 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(attribute.EditorLabel + string.Format(" ({0:N0})", list.Count));
			}

			int indexToRemove = -1;
			string label = "";
			for (int i = 0; i < list.Count; i++)
			{
				label = string.Format("Element {0:D}", i);
				EditorGUILayout.BeginHorizontal();

				if (i == 0)
				{
					EditorGUILayout.PrefixLabel(attribute.EditorLabel + string.Format(" ({0:N0})", list.Count));
				}
				else
				{
					GUILayout.Space(EditorGUIUtility.labelWidth);
				}

				if (arrayElementType == typeof(string))
				{
					list[i] = EditorGUILayout.TextField(label, list[i] as string);
				}
				else if (arrayElementType == typeof(int))
				{
					if (options != null && values != null)
					{
						list[i] = EditorGUILayout.IntPopup(label, int.Parse(list[i].ToString()), options, values);
					}
					else
					{
						UnityEngine.RangeAttribute rangeAttr = GetAttribute<UnityEngine.RangeAttribute>(field);

						if (rangeAttr != null)
						{
							list[i] = EditorGUILayout.IntSlider(label, (int)list[i], Mathf.FloorToInt(rangeAttr.min), Mathf.FloorToInt(rangeAttr.max));
						}
						else
						{
							list[i] = EditorGUILayout.IntField(label, (int)list[i]);
						}
					}
				}
				else if (arrayElementType == typeof(float))
				{
					UnityEngine.RangeAttribute rangeAttr = GetAttribute<UnityEngine.RangeAttribute>(field);

					if (rangeAttr != null)
					{
						list[i] = EditorGUILayout.Slider(label, (float)list[i], rangeAttr.min, rangeAttr.max);
					}
					else
					{
						list[i] = EditorGUILayout.FloatField(label, (float)list[i]);
					}
				}
				else if (arrayElementType == typeof(bool))
				{
					list[i] = EditorGUILayout.Toggle(label, (bool)list[i]);
				}
				else if (arrayElementType == typeof(AnimationCurve))
				{
					list[i] = EditorGUILayout.CurveField(label, (AnimationCurve)list[i]);
				}
				else if (arrayElementType.IsEnum)
				{
					System.Enum newValue = EditorGUILayout.EnumPopup(label, (System.Enum) System.Enum.Parse(list[i].GetType(), list[i].ToString()));
					list[i] = System.Convert.ChangeType(newValue, System.Enum.GetUnderlyingType(list[i].GetType()));
				}
				else
				{
					EditorGUILayout.LabelField(label, "Field type is not supported");
				}

				if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
				{
					if (EditorUtility.DisplayDialog("Warning!", "Are you sure?", "Yes", "No"))
					{
						indexToRemove = i;
					}
				}

				EditorGUILayout.EndHorizontal();
			}

			if (indexToRemove >= 0)
			{
				list.RemoveAt(indexToRemove);
			}

			if (list.Count == 0)
			{
				if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
				{
					list.Add(System.Activator.CreateInstance(arrayElementType));
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();

				if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
				{
					list.Add(System.Activator.CreateInstance(arrayElementType));
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			System.Array newArray = System.Array.CreateInstance(arrayElementType, list.Count);

			for (int i = 0; i < list.Count; i++)
			{
				newArray.SetValue(list[i], i);
			}

			field.SetValue(this, newArray);
		}

		protected void RenderStringField(FieldInfo field, DataField attribute)
		{
			object value = field.GetValue(this);
			string currentValue = "";
			if (value != null)
				currentValue = value.ToString();
			
			field.SetValue(this, EditorGUILayout.TextField(attribute.EditorLabel, currentValue));
		}

		protected void RenderIntField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			UnityEngine.RangeAttribute rangeAttr = GetAttribute<UnityEngine.RangeAttribute>(field);

			if (rangeAttr != null)
			{
				field.SetValue(this, EditorGUILayout.IntSlider(attribute.EditorLabel, int.Parse(field.GetValue(this).ToString()), Mathf.FloorToInt(rangeAttr.min), Mathf.FloorToInt(rangeAttr.max)));
			}
			else
			{
				field.SetValue(this, EditorGUILayout.IntField(attribute.EditorLabel, int.Parse(field.GetValue(this).ToString())));
			}
		}

		protected void RenderFloatField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			UnityEngine.RangeAttribute rangeAttr = GetAttribute<UnityEngine.RangeAttribute>(field);

			if (rangeAttr != null)
			{
				field.SetValue(this, EditorGUILayout.Slider(attribute.EditorLabel, float.Parse(field.GetValue(this).ToString()), rangeAttr.min, rangeAttr.max));
			}
			else
			{
				field.SetValue(this, EditorGUILayout.FloatField(attribute.EditorLabel, float.Parse(field.GetValue(this).ToString())));
			}

		}

		protected void RenderBoolField(FieldInfo field, DataField attribute)
		{
			// TODO: add errors check here too
			field.SetValue(this, EditorGUILayout.Toggle(attribute.EditorLabel, bool.Parse(field.GetValue(this).ToString())));
		}

		protected void RenderEnumField(FieldInfo field, DataField attribute)
		{
			object value = field.GetValue(this);
			System.Enum newValue = EditorGUILayout.EnumPopup(attribute.EditorLabel, (System.Enum) System.Enum.Parse(value.GetType(), value.ToString()));
			object uv = System.Convert.ChangeType(newValue, System.Enum.GetUnderlyingType(value.GetType()));
			field.SetValue(this, uv);
		}

		protected void RenderDateTimeField(FieldInfo field, DataField attribute)
		{
			EditorGUILayout.BeginHorizontal();

			string[] months = new string[] { 
				"01 January",
				"02 Februrary",
				"03 March",
				"04 April",
				"05 May",
				"06 June",
				"07 July",
				"08 August",
				"09 September",
				"10 October",
				"11 November",
				"12 December"
			};

			int[] monthsInt = new int[] { 
				1, 2, 3, 4, 5, 6,
				7, 8, 9, 10, 11, 12
			};

			DateData date = (DateData) field.GetValue(this);

			List<string> yearsNames = new List<string>();
			List<int> yearsValues = new List<int>();

			for (int y = System.DateTime.Now.Year; y < System.DateTime.Now.Year + 20; y++)
			{
				yearsNames.Add(y.ToString());
				yearsValues.Add(y);
			}

			int days = System.DateTime.DaysInMonth(date.Year, date.Month);

			List<string> daysNames = new List<string>();
			List<int> daysValues = new List<int>();
			for (int monthDay = 1; monthDay <= days; monthDay++)
			{
				daysNames.Add(monthDay.ToString());
				daysValues.Add(monthDay);
			}

			int year = EditorGUILayout.IntPopup(attribute.EditorLabel, date.Year, yearsNames.ToArray(), yearsValues.ToArray());
			int month = EditorGUILayout.IntPopup(date.Month, months, monthsInt);
			int day = EditorGUILayout.IntPopup(date.Day, daysNames.ToArray(), daysValues.ToArray());

			date.SetYear(year);
			date.SetMonth(month);
			date.SetDay(day);

			field.SetValue(this, date);

			EditorGUILayout.EndHorizontal();
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

		private T GetAttribute<T>(FieldInfo field) where T : PropertyAttribute
		{
			object[] attributes = field.GetCustomAttributes(typeof(T), true);

			foreach (object attribute in attributes)
			{
				T a = attribute as T;
				if (a != null)
				{
					return a;
				}
			}

			return null;
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

