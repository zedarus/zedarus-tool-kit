// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using UnityEditor;
using System;

namespace Zedarus.ToolKit.UserInterface.NGUI
{
	#if ZTK_NGUI
	[InitializeOnLoad]
	public class UIElementCallback
	{
		private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
		private static Texture2D hierarchyIcon;

		private static Texture2D HierarchyIcon
		{
			get
			{
				if (UIElementCallback.hierarchyIcon == null)
				{
					UIElementCallback.hierarchyIcon = (Texture2D)Resources.Load("UIElementIcon");
				}
				return UIElementCallback.hierarchyIcon;
			}
		}

		static UIElementCallback()
		{
			UIElementCallback.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UIElementCallback.DrawHierarchyIcon);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, UIElementCallback.hiearchyItemCallback);
		}
	
		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject != null && gameObject.GetComponent<UIElement>() != null)
			{
				Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
				GUI.DrawTexture(rect, UIElementCallback.HierarchyIcon);
			}
		}
	}
	#endif
}
