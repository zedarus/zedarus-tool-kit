using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	public interface IUIElementAnimation
	{
		void Init();
		void Show();
		void Hide();
		void Reset();
		float ShowDuration { get; set; }
		float HideDuration { get; set; }
		GameObject ObjectReference { get; }
	}
}
