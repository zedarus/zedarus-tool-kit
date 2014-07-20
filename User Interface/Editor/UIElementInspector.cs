using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInterface
{
	[CustomEditor(typeof(UIElement), true)]
	public class UIElementInspector : Editor
	{
		private bool _editTotalDuration = false;
		private float _newShowAnimationDuration = 0;
		private float _newHideAnimationDuration = 0;

		public override void OnInspectorGUI ()
		{
			#if UNITY_EDITOR
			UIElement element = (UIElement)target;

			bool saveNewAnimationValues = false;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(200));
			EditorGUILayout.LabelField("Show", GUILayout.Width(64), GUILayout.ExpandWidth(false));
			EditorGUILayout.LabelField("Hide", GUILayout.Width(64), GUILayout.ExpandWidth(false));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Duriations", GUILayout.MaxWidth(200));
			if (_editTotalDuration)
			{
				_newShowAnimationDuration = EditorGUILayout.FloatField(_newShowAnimationDuration, GUILayout.Width(64), GUILayout.ExpandWidth(false));
				_newHideAnimationDuration = EditorGUILayout.FloatField(_newHideAnimationDuration, GUILayout.Width(64), GUILayout.ExpandWidth(false));
				saveNewAnimationValues = GUILayout.Button("Save", EditorStyles.miniButton, GUILayout.Width(64));
				_editTotalDuration = !saveNewAnimationValues;
			}
			else
			{
				EditorGUILayout.LabelField(element.ShowAnimationDuration.ToString(), GUILayout.Width(64), GUILayout.ExpandWidth(false));
				EditorGUILayout.LabelField(element.HideAnimationDuration.ToString(), GUILayout.Width(64), GUILayout.ExpandWidth(false));
				_editTotalDuration = GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(64));
				if (_editTotalDuration)
				{
					_newShowAnimationDuration = element.ShowAnimationDuration;
					_newHideAnimationDuration = element.HideAnimationDuration;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (_newShowAnimationDuration < 0) _newShowAnimationDuration = 0;
			if (_newHideAnimationDuration < 0) _newHideAnimationDuration = 0;

			float totatShowDuration = 0f;
			float totalHideDuration = 0f;
			float ratio = 0;
			int removeIndex = -1;
			int moveIndex = -1;
			int moveDirection = 0;
			bool moveDown = false;
			bool moveUp = false;

			IUIElementAnimation[] allAnimations = element.gameObject.GetInterfacesInChildren<IUIElementAnimation>();

			for (int i = 0; i < element.AnimatedObjects.Count; i++)
			{
				if (element.AnimatedObjects[i] == null) continue;

				IUIElementAnimation animation = element.AnimatedObjects[i].GetInterface<IUIElementAnimation>();
				if (animation == null)
					continue;

				EditorGUILayout.BeginHorizontal();
				//GUILayout.Label(animation.ObjectReference.GetInstanceID().ToString(), GUILayout.MaxWidth(32));
				EditorGUILayout.ObjectField(animation.ObjectReference, typeof(GameObject), false, GUILayout.MaxWidth(200));

				if (saveNewAnimationValues)
				{
					ratio = animation.ShowDuration / element.ShowAnimationDuration;
					if (ratio > 1f) ratio = 1f;
					else if (ratio == 0 || float.IsNaN(ratio)) ratio = 0f;
					animation.ShowDuration = _newShowAnimationDuration * ratio;

					ratio = animation.HideDuration / element.HideAnimationDuration;
					if (ratio > 1f) ratio = 1f;
					else if (ratio == 0 || float.IsNaN(ratio)) ratio = 0f;
					animation.HideDuration = _newHideAnimationDuration * ratio;
				}

				if (float.IsNaN(animation.ShowDuration)) animation.ShowDuration = 0f;
				if (float.IsNaN(animation.HideDuration)) animation.HideDuration = 0f;

				animation.ShowDuration = EditorGUILayout.FloatField(animation.ShowDuration, GUILayout.Width(64), GUILayout.ExpandWidth(false));
				animation.HideDuration = EditorGUILayout.FloatField(animation.HideDuration, GUILayout.Width(64), GUILayout.ExpandWidth(false));

				bool displayUpButton = i > 0;
				bool displayDownButton = i < element.AnimatedObjects.Count - 1;

				if (displayUpButton) 
					moveUp = GUILayout.Button("↑", (displayDownButton ? EditorStyles.miniButtonLeft : EditorStyles.miniButton), GUILayout.Width(16));
				else
					GUILayout.Space(18);

				if (displayDownButton) 
					moveDown = GUILayout.Button("↓", (displayUpButton ? EditorStyles.miniButtonRight : EditorStyles.miniButton), GUILayout.Width(16));
				else
					GUILayout.Space(18);

				if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(64)))
					removeIndex = i;

				totatShowDuration += animation.ShowDuration;
				totalHideDuration += animation.HideDuration;

				EditorGUILayout.EndHorizontal();

				if (moveUp)
				{
					moveIndex = i;
					moveDirection = 1;
				}
				else if (moveDown)
				{
					moveIndex = i;
					moveDirection = -1;
				}

				moveUp = false;
				moveDown = false;
			}

			if (moveIndex >= 0 && moveDirection != 0)
			{
				if (moveDirection > 0)
				{
					Transform temp = element.AnimatedObjects[moveIndex - 1];
					element.AnimatedObjects[moveIndex - 1] = element.AnimatedObjects[moveIndex];
					element.AnimatedObjects[moveIndex] = temp;
				} 
				else if (moveDirection < 0)
				{
					Transform temp = element.AnimatedObjects[moveIndex + 1];
					element.AnimatedObjects[moveIndex + 1] = element.AnimatedObjects[moveIndex];
					element.AnimatedObjects[moveIndex] = temp;
				}
			}

			if (removeIndex >= 0)
				element.AnimatedObjects.RemoveAt(removeIndex);

			if (allAnimations.Length > 0 && element.AnimatedObjects.Count != allAnimations.Length) 
			{
				GUILayout.Space(16);
				EditorGUILayout.LabelField("Unused animations", GUILayout.MaxWidth(300));
			}

			foreach (IUIElementAnimation animation in allAnimations)
			{
				if (!element.AnimatedObjects.Contains(animation.ObjectReference.transform))
				{
					EditorGUILayout.BeginHorizontal();
					//GUILayout.Label(animation.ObjectReference.GetInstanceID().ToString(), GUILayout.MaxWidth(32));
					EditorGUILayout.ObjectField(animation.ObjectReference, typeof(GameObject), false, GUILayout.MaxWidth(200));
					bool add = GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(64));
					EditorGUILayout.EndHorizontal();

					if (add)
						element.AnimatedObjects.Add(animation.ObjectReference.transform);
				}
			}

			element.ShowAnimationDuration = totatShowDuration;
			element.HideAnimationDuration = totalHideDuration;
			
			if (GUI.changed)
				EditorUtility.SetDirty(element);
			#endif
		}
	}
}
