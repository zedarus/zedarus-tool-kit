using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Localisation
{
	[CustomEditor(typeof(LocaliseText))]
	public class LocaliseTextEditor : Editor
	{
		private SerializedObject _target;
		private SerializedProperty _textLabel;
		private SerializedProperty _page;
		private SerializedProperty _phrase;
		private SerializedProperty _localiseAtStart;

		private void OnEnable()
		{
			_target = new SerializedObject(target);
			_textLabel = _target.FindProperty("_textLabel");
			_page = _target.FindProperty("_page");
			_phrase = _target.FindProperty("_phrase");
			_localiseAtStart = _target.FindProperty("_localiseAtStart");
		}

		public override void OnInspectorGUI()
		{
			if (!LocalisationManager.Instance.HasData)
				LocalisationManager.Instance.UpdateData();

			_target.Update();

			int index;

			_textLabel.objectReferenceValue = EditorGUILayout.ObjectField("Text", _textLabel.objectReferenceValue, typeof(UnityEngine.UI.Text), true);

			string error = "";

			// Page:
			string[] sheets = LocalisationManager.Instance.Sheets;
			string newValue = _page.stringValue;
			if (sheets != null && sheets.Length > 0)
			{
				index = GetIndex(sheets, newValue);
				if (index < 0)
					index = 0;
				index = EditorGUILayout.Popup("Page", index, sheets);
				newValue = sheets[index];
			}
			else
			{
				newValue = EditorGUILayout.TextField("Page", newValue);
				error += "No pages data found.\n";
			}

			_page.stringValue = newValue;

			// Phrase:
			string[] phrases = LocalisationManager.Instance.GetPhrasesForSheet(_page.stringValue);
			newValue = _phrase.stringValue;
			if (phrases != null && phrases.Length > 0)
			{
				index = GetIndex(phrases, newValue);
				if (index < 0)
					index = 0;
				index = EditorGUILayout.Popup("Phrase", index, phrases);
				newValue = phrases[index];
			}
			else
			{
				newValue = EditorGUILayout.TextField("Phrase", newValue);
				error += "No phrases data found for this page.\n";
			}

			_phrase.stringValue = newValue;

			// Params:
			EditorGUILayout.PropertyField(_localiseAtStart);

			if (error.Length > 0)
			{
				error += "Please refresh data.";
				EditorGUILayout.HelpBox(error, MessageType.Warning);
			}

			if (GUILayout.Button("Refresh Data"))
			{
				LocalisationManager.Instance.UpdateData();
			}

			//if (changed)
			_target.ApplyModifiedProperties();
		}

		#region Helpers
		private int GetIndex(string[] array, string element)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(element))
					return i;
			}

			return -1;
		}
		#endregion
	}
}
