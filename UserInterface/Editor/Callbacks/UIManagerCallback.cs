using UnityEngine;
using UnityEditor;
using System;

namespace Zedarus.ToolKit.UI
{
	[InitializeOnLoad]
	public class UIManagerCallback
	{
		private static EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
		private static string[] icons = new string[] 
		{ 
			"UIManagerIcon", 
			"UIScreenIcon",
			"UIPopupIcon"
		};
		private static Texture2D[] hierarchyIcons = new Texture2D[icons.Length];

		private static Texture2D GetHierarchyIcon(int index)
		{
			if (UIManagerCallback.hierarchyIcons[index] == null)
			{
				UIManagerCallback.hierarchyIcons[index] = (Texture2D)Resources.Load(icons[index]);
			}
			return UIManagerCallback.hierarchyIcons[index];
		}

		static UIManagerCallback()
		{
			UIManagerCallback.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UIManagerCallback.DrawHierarchyIcon);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, UIManagerCallback.hiearchyItemCallback);
		}
	
		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (gameObject != null)
			{
				int index = 0;

				if (gameObject.GetComponent<UIManager>() != null)
					index = 0;
				else if (gameObject.GetComponent<UIPopup>() != null)
					index = 2;
				else if (gameObject.GetComponent<UIScreen>() != null)
					index = 1;
				else
					index = -1;

				if (index >= 0)
				{
					Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
					GUI.DrawTexture(rect, UIManagerCallback.GetHierarchyIcon(index));
				}
			}
		}
	}
}
