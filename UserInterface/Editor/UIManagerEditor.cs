using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UI
{
	[CustomEditor(typeof(UIManager))]
	public class UIManagerEditor : Editor
	{
		private const string none = "-";

		public override void OnInspectorGUI()
		{
			UIManager myTarget = (UIManager)target;

			bool autoInit = myTarget.AutoInit;
			if (autoInit)
			{
				GUILayout.BeginHorizontal();
				autoInit = EditorGUILayout.ToggleLeft("Auto init", myTarget.AutoInit, GUILayout.Width(100f));
				EditorGUILayout.LabelField("Delay", GUILayout.Width(40f));
				myTarget.AutoInitDelay = EditorGUILayout.FloatField("", myTarget.AutoInitDelay, GUILayout.MinWidth(20f));
				EditorGUILayout.LabelField("seconds", GUILayout.Width(60f));
				GUILayout.EndHorizontal();
			}
			else
			{
				autoInit = EditorGUILayout.Toggle("Auto inititialize", myTarget.AutoInit);
			}
			myTarget.AutoInit = autoInit;

			GUILayout.Space(16);

			myTarget.FirstScreenIndex = RenderScreensList<UIScreen>("Screens", myTarget.Screens, myTarget.RemoveScreen, myTarget.AddScreen, myTarget.FirstScreenIndex, "Add screen", true, "First screen");
			myTarget.FirstScreenInstantAnimation = EditorGUILayout.Toggle("Show first screen instantly", myTarget.FirstScreenInstantAnimation);

			GUILayout.Space(16);

			RenderScreensList<UIPopup>("Popups", myTarget.Popups, myTarget.RemovePopup, myTarget.AddPopup, 0, "Add popup", false);
			myTarget.PopupsClickBlocker = EditorGUILayout.ObjectField("Popups Clicks Blocker", myTarget.PopupsClickBlocker, typeof(Image), true) as Image;
			myTarget.PopupsBackgroundAnimator = EditorGUILayout.ObjectField("Popups Background Animator", myTarget.PopupsBackgroundAnimator, typeof(Animator), true) as Animator;
		}

		private int RenderScreensList<T>(string title, List<T> screens, Action<int> RemoveHandler, Action AddHandler, int firstIndex, string addButtonLabel, bool pickFirst, string firstLabel = null) where T : UIScreen
		{
			string[] options = new string[screens.Count];

			if (screens.Count > 0)
				EditorGUILayout.LabelField(title + ":");

			int removeScreen = -1;
			for (int i = 0; i < screens.Count; i++)
			{
				T screen = screens[i];

				EditorGUILayout.BeginHorizontal();

				T newScreen = EditorGUILayout.ObjectField(screen, typeof(T), true) as T;
				if (!IsDuplicate<T>(newScreen, screens))
					screens[i] = newScreen;

				if (screen != null)
					options[i] = screen.name;
				else
					options[i] = none;

				if (screens[i] != null && screens[i].CustomID != null)
				{
					EditorGUILayout.LabelField("ID: \"" + screens[i].CustomID + "\"", GUILayout.MaxWidth(120f));
					//EditorGUILayout.SelectableLabel("Custom ID: " + screens[i].CustomID);
				}

				if (GUILayout.Button("-", GUILayout.Width(20)))
				{
					if (EditorUtility.DisplayDialog("Confirm", "Are you sure?", "Yes", "No"))
						removeScreen = i;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (removeScreen >= 0 && RemoveHandler != null)
				RemoveHandler(removeScreen);

			if (GUILayout.Button(addButtonLabel) && AddHandler != null)
				AddHandler();

			if (pickFirst && screens.Count > 0)
			{
				GUILayout.Space(4);

				int index = EditorGUILayout.Popup(firstLabel, firstIndex, options);
				if (options.Length > 0 && !options[index].Equals(none))
					firstIndex = index;
			}
			else
				firstIndex = 0;

			return firstIndex;
		}

		private bool IsDuplicate<T>(T screen, List<T> screens) where T : UIScreen
		{
			foreach (T s in screens)
			{
				if (s == screen)
					return true;
			}

			return false;
		}
	}
}
