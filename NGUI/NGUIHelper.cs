using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.NGUI
{
	public class NGUIHelper
	{
		static public float ScreenHeight
		{
			get
			{
				if (UIRoot.list.Count > 0)
					return UIRoot.list[0].activeHeight;
				else
					return Screen.height;
			}
		}
	}
}
