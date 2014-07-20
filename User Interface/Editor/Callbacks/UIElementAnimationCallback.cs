// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using UnityEditor;
using System;

namespace Zedarus.ToolKit.UserInterface
{
	[InitializeOnLoad]
	public class UIElementAnimationCallback
	{
		private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
		private static Texture2D hierarchyIcon;

		private static Texture2D HierarchyIcon
		{
			get
			{
				if (UIElementAnimationCallback.hierarchyIcon == null)
				{
					UIElementAnimationCallback.hierarchyIcon = (Texture2D)Resources.Load("UIElementAnimationIcon");
				}
				return UIElementAnimationCallback.hierarchyIcon;
			}
		}

		static UIElementAnimationCallback()
		{
			UIElementAnimationCallback.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UIElementAnimationCallback.DrawHierarchyIcon);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, UIElementAnimationCallback.hiearchyItemCallback);
		}
	
		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject != null && gameObject.GetInterface<IUIElementAnimation>() != null)
			{
				Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
				GUI.DrawTexture(rect, UIElementAnimationCallback.HierarchyIcon);
			}
		}
	}
}
