// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using UnityEditor;
using System;

namespace Zedarus.ToolKit.UserInterface
{
	[InitializeOnLoad]
	public class UIElementsGroupCallback
	{
		private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
		private static Texture2D hierarchyIcon;

		private static Texture2D HierarchyIcon
		{
			get
			{
				if (UIElementsGroupCallback.hierarchyIcon == null)
				{
					UIElementsGroupCallback.hierarchyIcon = (Texture2D)Resources.Load("UIElementsGroupIcon");
				}
				return UIElementsGroupCallback.hierarchyIcon;
			}
		}

		static UIElementsGroupCallback()
		{
			UIElementsGroupCallback.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UIElementsGroupCallback.DrawHierarchyIcon);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, UIElementsGroupCallback.hiearchyItemCallback);
		}
	
		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject != null && gameObject.GetComponent<UIElementsGroup>() != null)
			{
				Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
				GUI.DrawTexture(rect, UIElementsGroupCallback.HierarchyIcon);
			}
		}
	}
}
