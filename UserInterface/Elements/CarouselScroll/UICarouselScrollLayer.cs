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

		[Header("Scroll Settings")]
		[SerializeField]
		[Range(0.1f, 100f)]
		private float _scrollSpeed = 10f;

		[SerializeField]
		[Range(0.0001f, 1f)]
		private float _speedEasing = 0.5f;
		#endregion

		#region Properties
		private float _pageWidth;
		private int _pages = 0;
		private float _scrollPosition = 0;
		private float _targetScrollPosition = 0;
		private float _velocity = 0f;
		private float _targetVelocity = 0f;
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

		public void Update(float deltaTime)
		{
			_targetVelocity = (TargetScrollPosition - ScrollPosition);
			_velocity += (_targetVelocity - _velocity) * SpeedEasing;
			ScrollPosition += _velocity * ScrollSpeed * deltaTime;

			Pivot.localPosition = new Vector2(ScrollPosition * Parallax, 0);
		}

		public void Drag(float delta)
		{
			TargetScrollPosition += delta;
		}

		public bool ShowPage(int page, bool tween = true)
		{
			if (page >= 0 && page < Pages)
			{
				TargetScrollPosition = -PageWidth * page;

				if (!tween) 
				{
					ScrollPosition = TargetScrollPosition;
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
		private float ScrollPosition
		{
			get { return _scrollPosition; }
			set 
			{
				if (_pullAllowed)
					_scrollPosition = value; 
				else
					_scrollPosition = Mathf.Clamp(value, MinDrag, MaxDrag);
			}
		}

		private float TargetScrollPosition
		{
			get { return _targetScrollPosition; }
			set { _targetScrollPosition = Mathf.Clamp(value, MinDrag, MaxDrag); }
		}

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

		private float ScrollSpeed
		{
			get { return _scrollSpeed; }
		}

		private float SpeedEasing
		{
			get { return _speedEasing; }
		}
		#endregion
	}
}
