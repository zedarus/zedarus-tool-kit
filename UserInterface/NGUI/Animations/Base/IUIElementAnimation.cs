using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface.NGUI
{
	#if ZTK_NGUI
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
	#endif
}
