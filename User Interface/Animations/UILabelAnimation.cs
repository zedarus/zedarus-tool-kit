using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	[RequireComponent(typeof(UILabel))]
	public class UILabelAnimation : UIElementAnimation
	{
		#region Parameters
		private UILabel _label;
		private float _maxAlpha;
		#endregion

		public override void Init()
		{
			_label = GetComponent<UILabel>();
			_maxAlpha = _label.alpha;
			base.Init();
		}

		#region Controls
		public override void Show()
		{
			TweenAlpha.Begin(gameObject, ShowDuration, _maxAlpha);
			base.Show();
		}
		
		public override void Hide()
		{
			TweenAlpha.Begin(gameObject, HideDuration, 0f);
			base.Hide();
		}
		
		public override void Reset()
		{
			if (_label != null) _label.alpha = 0f;
			base.Reset();
		}
		#endregion
	}
}
