using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	[RequireComponent(typeof(UISprite))]
	public class UISpriteFadeAnimation : UIElementAnimation
	{
		#region Parameters
		[SerializeField] private bool _reverse = false;
		[SerializeField] private bool _forceMaxAlpha = false;
		[SerializeField] private float _maxAlpha = 1f;
		private UISprite _sprite;
		#endregion
		
		public override void Init()
		{
			_sprite = GetComponent<UISprite>();
			if (!_forceMaxAlpha)
				_maxAlpha = _sprite.alpha;
			base.Init();
		}
		
		#region Controls
		public override void Show()
		{
			TweenAlpha.Begin(gameObject, ShowDuration, _reverse ? 0f : _maxAlpha);
			base.Show();
		}
		
		public override void Hide()
		{
			TweenAlpha.Begin(gameObject, HideDuration, _reverse ? _maxAlpha : 0f);
			base.Hide();
		}
		
		public override void Reset()
		{
			if (_sprite != null) _sprite.alpha = _reverse ? _maxAlpha : 0f;
			base.Reset();
		}
		#endregion
	}
}
