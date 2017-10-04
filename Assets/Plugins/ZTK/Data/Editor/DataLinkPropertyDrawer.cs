using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Zedarus.Toolkit.Data
{
	[CustomPropertyDrawer(typeof(DataLinkAttribute))]
	public class DataLinkPropertyDrawer : PropertyDrawer
	{
		private const string FieldName = "_ZTKDataLinks";
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DataLinksManager links = GetLinksManager(property);

			if (links == null)
			{
				EditorGUI.LabelField(position, label, new GUIContent("No " + FieldName + " property found on this object!"));
			}
			else
			{
				int selectedIndex = EditorGUI.Popup(position, label, 0, DataSource.GetEntriesNamesOfType(GetPropertyType(property)));
			}
			
			// TODO: here's how to apply changes in links manager
			//serializedObject.CopyFromSerializedProperty(new SerializedObject(target).FindProperty("ach"));

			property.serializedObject.ApplyModifiedProperties();
		}

		private DataLinksManager GetLinksManager(SerializedProperty p)
		{
			Type t = p.serializedObject.targetObject.GetType();
			FieldInfo field = t.GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				return field.GetValue(p.serializedObject.targetObject) as DataLinksManager;
			}
			return null;
		}

		private Type GetPropertyType(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
					return typeof(int);
			}

			return null;
		}

		private IData DataSource
		{
			get { return DataSimulation.Instance; }
		}
	}
}
