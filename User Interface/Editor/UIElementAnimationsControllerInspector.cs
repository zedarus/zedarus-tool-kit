using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInterface
{
	[CustomEditor(typeof(UIElementAnimationsController), true)]
	public class UIElementAnimationsControllerInspector : Editor
	{
		public override void OnInspectorGUI ()
		{
			#if UNITY_EDITOR
			UIElementAnimationsController controller = (UIElementAnimationsController)target;

			//
			// Draw animations
			// 
			List<int> removeIndexes = new List<int>();
			int moveIndex = -1;
			int moveDirection = 0;
			bool moveDown = false;
			bool moveUp = false;
			
			GUILayout.Label("Animated elements:");
			for (int i = 0; i < controller.Animations.Count; i++)
			{
				moveUp = false;
				moveDown = false;
				UIAnimation animation = controller.Animations[i];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField((i + 1).ToString(), GUILayout.MaxWidth(20));
				animation.Element = (UIElement) EditorGUILayout.ObjectField(animation.Element, typeof(UIElement), true, GUILayout.MaxWidth(256));

				GUILayout.FlexibleSpace();

				bool displayUpButton = i > 0;
				bool displayDownButton = i < controller.Animations.Count - 1;

				if (displayUpButton) 
					moveUp = GUILayout.Button("↑", (displayDownButton ? EditorStyles.miniButtonLeft : EditorStyles.miniButton));
				else
					GUILayout.Space(18);

				if (displayDownButton) 
					moveDown = GUILayout.Button("↓", (displayUpButton ? EditorStyles.miniButtonRight : EditorStyles.miniButton));
				else
					GUILayout.Space(18);

				if (GUILayout.Button("Remove", EditorStyles.miniButton))
					if (EditorUtility.DisplayDialog("Removing animation", "Are you sure you want to remove animation from queue?", "Yes", "No")) removeIndexes.Add(i);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(20));
				EditorGUILayout.LabelField("Show delay", GUILayout.Width(80));
				animation.ShowDelay = EditorGUILayout.FloatField(animation.ShowDelay, GUILayout.MaxWidth(80));
				animation.ShowSync = EditorGUILayout.ToggleLeft("sync with previous", animation.ShowSync, GUILayout.Width(128));
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(20));
				EditorGUILayout.LabelField("Hide delay", GUILayout.Width(80));
				animation.HideDelay = EditorGUILayout.FloatField(animation.HideDelay, GUILayout.MaxWidth(80));
				animation.HideSync = EditorGUILayout.ToggleLeft("sync with previous", animation.HideSync, GUILayout.Width(128));
				EditorGUILayout.EndHorizontal();

				if (animation.ShowDelay < 0f) animation.ShowDelay = 0f;
				if (animation.HideDelay < 0f) animation.HideDelay = 0f;

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
					UIAnimation temp = controller.Animations[moveIndex - 1];
					controller.Animations[moveIndex - 1] = controller.Animations[moveIndex];
					controller.Animations[moveIndex] = temp;
				} 
				else if (moveDirection < 0)
				{
					UIAnimation temp = controller.Animations[moveIndex + 1];
					controller.Animations[moveIndex + 1] = controller.Animations[moveIndex];
					controller.Animations[moveIndex] = temp;
				}
			}

			for (int i = removeIndexes.Count - 1; i >= 0; i--)
				controller.Animations.RemoveAt(removeIndexes[i]);

			GUILayout.Space(16);

			UIElement[] elements = controller.gameObject.GetComponentsInChildren<UIElement>();

			GUILayout.Label("Adding elements:");
			foreach (UIElement element in elements)
			{
				if (!controller.ContainsElement(element))
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.ObjectField(element, typeof(UIElement), false);

					if (GUILayout.Button("Add", GUILayout.Width(64)))
						controller.CreateAnimation(element);

					EditorGUILayout.EndHorizontal();
				}
			}

			bool addAnimation = GUILayout.Button("Add empty element");
			
			if (addAnimation)
				controller.CreateAnimation();

			GUILayout.Space(16);
			
			GUILayout.Label("Debug controls:");
			if (GUILayout.Button("Show")) controller.Show();
			if (GUILayout.Button("Hide")) controller.Hide();

			if (GUI.changed)
				EditorUtility.SetDirty(controller);
			#endif
		}
	}
}
