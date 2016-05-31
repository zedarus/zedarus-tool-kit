using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI.Elements
{
	[System.Serializable]
	public class UICarouselScrollLayer
	{
		#region Parameters
		[SerializeField] 
		private Transform _pivot;

		[SerializeField] 
		private float _parallax = 1f;

		[SerializeField] 
		[Range(0f, 0.5f)]
		private float _nextPageShowPercent = 0f;	// how much of a surface area of next page should be visible on screen

		[SerializeField] 
		private bool _pullAllowed = false;

		[SerializeField]
		[Range(0f, 1f)]
		private float _pullPercentFromPageWidth = 0.5f;
		#endregion

		#region Properties
		private float _pageWidth;
		private int _pages = 0;
		private float _scrollPosition = 0;
		private float _targetScrollPosition = 0;
		#endregion

		#region Init
		public void Init(float pageWidth)
		{
			_pageWidth = pageWidth;
		}
		#endregion

		#region Controls
		public void CreatePage(IUICarouselScrollPage page)
		{
			if (_nextPageShowPercent > 0f)
			{
				float pageSize = PageWidth * 0.5f - (page.PageSize * _nextPageShowPercent) + page.PageSize * 0.5f;
				_parallax = pageSize / PageWidth;
			}

			page.InitAsPage(_pageWidth, Parallax);

			Vector3 scale = page.PageTransform.localScale;
			page.PageTransform.SetParent(Pivot);
			page.PageTransform.localScale = scale;
			page.PageTransform.localPosition = new Vector2(PageWidth * Parallax * Pages, 0);

			_pages++;
		}

		public void Update(float easing)
		{
			_scrollPosition += (_targetScrollPosition - _scrollPosition) * easing;
			Pivot.localPosition = new Vector2(_scrollPosition * Parallax, 0);
		}

		public void Drag(float delta)
		{
			_targetScrollPosition = Mathf.Clamp(_targetScrollPosition + delta, MinDrag, MaxDrag);
		}

		public bool ShowPage(int page, bool tween = true)
		{
			if (page >= 0 && page < Pages)
			{
//				_targetDrag = -_pageWidth * _scale * page;
				_targetScrollPosition = -PageWidth * page;

				if (!tween) 
				{
					_scrollPosition = _targetScrollPosition;
				}

				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Getters
		private Transform Pivot
		{
			get { return _pivot; }
		}

		private int Pages
		{
			get { return _pages; }
		}

		private float Parallax
		{
			get { return _parallax; }
		}

		private float PageWidth
		{
			get { return _pageWidth; }
		}

		private float MinDrag
		{
			// TODO: use scale here
			get { return -((Pages - 1) * PageWidth) - PullDistance; }
//			return -((MaxPages - 1) * PageWidth * _scale) - (allowOffBoard ? OffBorderDragDistance : 0f);
		}

		private float MaxDrag
		{
			get { return 0f + PullDistance; }
		}

		private float PullDistance
		{
			get { return _pullAllowed ? PageWidth * _pullPercentFromPageWidth : 0f; }
		}
		#endregion
	}
}
