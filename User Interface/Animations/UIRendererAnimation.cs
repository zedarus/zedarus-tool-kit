using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	[AddComponentMenu("ZTK/UI/Renderer Animation")]
	public class UIRendererAnimation : UIElementAnimation
	{
		private UISprite[] _sprites;

		public override void Init()
		{
			_sprites = GetComponentsInChildren<UISprite>();
			base.Init();
		}

		#region Controls
		public override void Show()
		{
			foreach (UISprite sprite in _sprites)
				sprite.enabled = true;
			base.Show();
		}
		
		public override void Hide()
		{
			foreach (UISprite sprite in _sprites)
				sprite.enabled = false;
			base.Hide();
		}
		
		public override void Reset()
		{
			if (_sprites != null)
			{
				foreach (UISprite sprite in _sprites)
					sprite.enabled = false;
			}
			base.Reset();
		}
		#endregion
	}
}
