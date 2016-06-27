using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		[Range(0f, 2f)]
		private float _nextPageShowPercent = 0f;	// how much of a surface area of next page should be visible on screen

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
		private float _dragScale = 1f;
		private float _pageWidth = 0;
		private float _scrollPosition = 0;
		private float _targetScrollPosition = 0;
		private float _velocity = 0f;
		private float _targetVelocity = 0f;
		private bool _pageSizeCached = false;
		private List<IUICarouselScrollPage> _pages = new List<IUICarouselScrollPage>();
		#endregion

		#region Controls
		public void CreatePage(IUICarouselScrollPage page)
		{
			if (!_pageSizeCached && _pivot != null)
			{
				if (_pivot is RectTransform)
				{
					RectTransform c = GetCanvasTransform(_pivot as RectTransform);
					if (c != null)
					{
						_pageWidth = c.sizeDelta.x;
						_dragScale = 1f;
					}
				}
				else
				{
					Vector3 p1 = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
					Vector3 p2 = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f));

					_pageWidth = Vector3.Distance(p1, p2);

					p1 = Camera.main.ViewportToScreenPoint(new Vector2(0f, 0f));
					p2 = Camera.main.ViewportToScreenPoint(new Vector2(1f, 0f));

					float screenWidth = Vector2.Distance(p1, p2);

					_dragScale = _pageWidth / screenWidth;
				}

				_pageSizeCached = true;
			}

			if (_nextPageShowPercent > 0f)
			{
				float pageSize = PageWidth * 0.5f - (page.GraphicsSize * _nextPageShowPercent) + page.GraphicsSize * 0.5f;
				_parallax = pageSize / PageWidth;
			}

			page.InitAsPage(PageWidth, Parallax);

			Vector3 scale = page.PageTransform.localScale;
			page.PageTransform.SetParent(Pivot);
			page.PageTransform.localScale = scale;
			page.PageTransform.localPosition = new Vector2(PageWidth * Parallax * Pages, 0);
			_pages.Add(page);
		}

		public void Update(float deltaTime)
		{
			_targetVelocity = (TargetScrollPosition - ScrollPosition);
			_velocity += (_targetVelocity - _velocity) * SpeedEasing;
			ScrollPosition += _velocity * ScrollSpeed * deltaTime;

			// TODO: only change position if change is significant enough
			Pivot.localPosition = new Vector2(ScrollPosition * Parallax, 0);

			// TODO: optimize this
			foreach (IUICarouselScrollPage page in _pages)
			{
				page.UpdateScrollPivot(Pivot.parent.transform.position);
			}
		}

		public void Drag(float delta)
		{
			TargetScrollPosition += delta * _dragScale; 
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

		#region Helpers
		private RectTransform GetCanvasTransform(RectTransform t)
		{
			int attempts = 100;
			while (attempts > 0)
			{
				if (t.GetComponent<Canvas>() != null)
				{
					return t;
				}
				else if (t.parent != null)
				{
					RectTransform parent = t.parent.GetComponent<RectTransform>();
					if (parent != null)
					{
						t = parent;
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}

				attempts--;
			}

			return null;
		}
		#endregion

		#region Getters
		private float ScrollPosition
		{
			get { return _scrollPosition; }
			set 
			{
				if (PullAllowed)
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

		public int Pages
		{
			get { return _pages.Count; }
		}

		private float Parallax
		{
			get { return _parallax; }
		}

		public float PageWidth
		{
			get { return _pageWidth; }
		}

		private float MinDrag
		{
			get { return -((Pages - 1) * PageWidth) - PullDistance; }
		}

		private float MaxDrag
		{
			get { return 0f + PullDistance; }
		}

		private float PullDistance
		{
			get { return PullAllowed ? PageWidth * _pullPercentFromPageWidth : 0f; }
		}

		private float ScrollSpeed
		{
			get { return _scrollSpeed; }
		}

		private float SpeedEasing
		{
			get { return _speedEasing; }
		}

		private bool PullAllowed
		{
			get { return _pullPercentFromPageWidth > 0f; }
		}
		#endregion
	}
}
