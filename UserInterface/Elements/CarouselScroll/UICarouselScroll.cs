﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UI.Elements
{
	public class UICarouselScroll
	{
		#region Properties
		private UICarouselScrollSettings _settings;
		private int _page;
		private float _dragDistance;
		private float _pageWidthInPixels;
		#endregion

		#region Events
		public event System.Action<int, int> UpdateNavigation;
		#endregion

		#region Init
		public UICarouselScroll(UICarouselScrollSettings settings)
		{
			_page = 0;
			_settings = settings;

			Vector2 left = Camera.main.ViewportToScreenPoint(new Vector2(0, 0));
			Vector2 right = Camera.main.ViewportToScreenPoint(new Vector2(1, 0));
			_pageWidthInPixels = Vector2.Distance(left, right);
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
		public void Update(float deltaTime)
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

		public void NextPage()
		{
			ShowPage(_page + 1);
		}

		public void PreviousPage()
		{
			ShowPage(_page - 1);
		}
		#endregion

		#region Helpers
		public bool ShowPage(int page, bool tween = true)
		{
			bool result = true;

			foreach (UICarouselScrollLayer layer in _settings.Layers)
			{
				if (!layer.ShowPage(page, tween))
					result = false;
			}

			if (result)
			{
				_page = page;

				if (UpdateNavigation != null)
				{
					UpdateNavigation(_page, MaxPages);		
				}
			}

			return result;
		}

		private float SwipeThreshold
		{
			get { return _pageWidthInPixels * _settings.SwipeThreshold; } 
		}
		#endregion

		#region Getters
		public int CurrentPage
		{
			get { return _page; }
		}

		private int MaxPages
		{
			get 
			{
				int pages = 0;
				foreach (UICarouselScrollLayer layer in _settings.Layers)
				{
					if (layer.Pages > pages)
						pages = layer.Pages;
				}
				return pages;
			}
		}
		#endregion
	}
}
