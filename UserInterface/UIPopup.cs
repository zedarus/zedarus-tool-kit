using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI
{
	public class UIPopup : UIScreen
	{
		public override bool BackKeyPressed()
		{
			Close();
			return true;
		}
	}
}
