// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using UnityEditor;
using System;

namespace Zedarus.ToolKit.UserInterface
{
	#if !ZTK_DISABLE_NGUI
	[InitializeOnLoad]
	public class UIControllerCallback
	{
		private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
		private static Texture2D hierarchyIcon;

		private static Texture2D HierarchyIcon
		{
			get
			{
				if (UIControllerCallback.hierarchyIcon == null)
				{
					UIControllerCallback.hierarchyIcon = (Texture2D)Resources.Load("UIControllerIcon");
				}
				return UIControllerCallback.hierarchyIcon;
			}
		}

		static UIControllerCallback()
		{
			UIControllerCallback.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UIControllerCallback.DrawHierarchyIcon);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, UIControllerCallback.hiearchyItemCallback);
		}
	
		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject != null && gameObject.GetComponent<UIController>() != null)
			{
				Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
				GUI.DrawTexture(rect, UIControllerCallback.HierarchyIcon);
			}
		}
	}
#endif
}
