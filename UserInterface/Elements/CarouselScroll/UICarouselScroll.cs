using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UI.Elements
{
	public class UICarouselScroll
	{
		#region Properties
		private UICarouselScrollSettings _settings;
		private int _page;
		private float _pageWidth;
		private float _dragDistance;
		#endregion

		#region Init
		public UICarouselScroll(float pageWidth, UICarouselScrollSettings settings)
		{
			_page = 0;
			_pageWidth = pageWidth;
			_settings = settings;

			foreach (UICarouselScrollLayer layer in _settings.Layers)
			{
				layer.Init(_pageWidth);
			}
		}

		public void CreatePage(IUICarouselScrollPage page, int layer)
		{
			if (layer >= 0 && layer < _settings.Layers.Length)
			{
				_settings.Layers[layer].CreatePage(page);
			}
			else
			{
				throw new System.ArgumentOutOfRangeException("layer", "No layer with such index");
			}
		}
		#endregion

		#region Controls
		public void Update(float deltaTime, bool drag)
		{
			foreach (UICarouselScrollLayer layer in _settings.Layers)
			{
				layer.Update(deltaTime);
			}
		}

		public void BeginDrag()
		{
			_dragDistance = 0f;
		}

		public void Drag(float delta)
		{
			_dragDistance += delta;
			foreach (UICarouselScrollLayer layer in _settings.Layers)
			{
				layer.Drag(delta);
			}
		}

		public void EndDrag()
		{
			if (Mathf.Abs(_dragDistance) >= SwipeThreshold)
			{
				bool result = false;

				if (_dragDistance < 0)
					result = ShowPage(_page + 1);
				else if (_dragDistance > 0)
					result = ShowPage(_page - 1);

				if (result)
					return;
			}

			ShowPage(_page);
		}
		#endregion

		#region Helpers
		private bool ShowPage(int page, bool tween = true)
		{
			bool result = true;

			foreach (UICarouselScrollLayer layer in _settings.Layers)
			{
				if (!layer.ShowPage(page, tween))
					result = false;
			}

			if (result)
				_page = page;

			return result;
		}

		private float GetPageWidth(int layer)
		{
			return PageWidth;
		}

		private float SwipeThreshold
		{
			get { return PageWidth * _settings.SwipeThreshold; } 
		}
		#endregion

		#region Getters
		private float PageWidth
		{
			get { return _pageWidth; }
		}
		#endregion
	}
}
