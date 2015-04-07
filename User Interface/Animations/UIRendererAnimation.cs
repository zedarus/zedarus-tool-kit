using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	#if !ZTK_DISABLE_NGUI
	[AddComponentMenu("ZTK/UI/Renderer Animation")]
	public class UIRendererAnimation : UIElementAnimation
	{
		private UIWidget[] _widgets;

		public override void Init()
		{
			_widgets = GetComponentsInChildren<UIWidget>();
			base.Init();
		}

		#region Controls
		public override void Show()
		{
			foreach (UIWidget widget in _widgets)
				widget.enabled = true;
			base.Show();
		}
		
		public override void Hide()
		{
			foreach (UIWidget widget in _widgets)
				widget.enabled = false;
			base.Hide();
		}
		
		public override void Reset()
		{
			if (_widgets != null)
			{
				foreach (UIWidget widget in _widgets)
					widget.enabled = false;
			}
			base.Reset();
		}
		#endregion
	}
	#endif
}
