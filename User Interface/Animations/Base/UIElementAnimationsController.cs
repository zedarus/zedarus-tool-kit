using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.UserInterface
{
	public class UIElementAnimationsController : MonoBehaviour
	{
		#region Parameters
		[SerializeField]
		private List<UIAnimation> _animations = new List<UIAnimation>();
		private bool _initialized = false;
		#endregion

		#region Controls
		public void Init()
		{
			for (int i = 0; i < _animations.Count; i++)
				_animations[i].Init();

			_initialized = true;
		}

		public float Show()
		{
			if (!_initialized)
			{
				ZedLogger.Log("Animations controller not initialized");
				return 0f;
			}

			float delay = 0f;
			float animationDelay = 0f;
			float totalDuration = 0f;
			for (int i = 0; i < _animations.Count; i++)
			{
				animationDelay = _animations[i].Show(delay + _animations[i].ShowDelay);

				if (i < _animations.Count - 1 && !_animations[i + 1].ShowSync)
					delay += animationDelay + _animations[i].ShowDelay;

				totalDuration += animationDelay + _animations[i].ShowDelay;
			}

			return totalDuration;
		}

		public float Hide()
		{
			if (!_initialized)
			{
				ZedLogger.Log("Animations controller not initialized");
				return 0f;
			}

			float delay = 0f;
			float animationDelay = 0f;
			float totalDuration = 0f;
			for (int i = _animations.Count - 1; i >= 0; i--)
			{
				animationDelay = _animations[i].Hide(delay + _animations[i].HideDelay);

				if (i > 0 && !_animations[i - 1].HideSync)
					delay += animationDelay + _animations[i].HideDelay;

				totalDuration += animationDelay + _animations[i].HideDelay;
			}

			return totalDuration;
		}
		#endregion

		#if UNITY_EDITOR
		public void CreateAnimation()
		{
			_animations.Add(new UIAnimation());
		}

		public void CreateAnimation(UIElement element)
		{
			UIAnimation newAnimation = new UIAnimation();
			newAnimation.Element = element;
			_animations.Add(newAnimation);
		}
		
		public List<UIAnimation> Animations
		{
			get { return _animations; }
			set { _animations = value; }
		}

		public bool ContainsElement(UIElement element)
		{
			foreach (UIAnimation animation in Animations)
			{
				if (animation.Element == element)
					return true;
			}
			return false;
		}
		#endif
	}
	
	[System.Serializable]
	public class UIAnimation
	{
		#region Parameters
		[SerializeField]
		private UIElement _element;
		[SerializeField]
		private float _showDelay = 0f;
		[SerializeField]
		private float _hideDelay = 0f;
		[SerializeField]
		private bool _showSync = false;
		[SerializeField]
		private bool _hideSync = false;
		#endregion

		public UIAnimation()
		{
		}

		public void Init()
		{
			if (_element != null)
				_element.Init();
		}

		public float Show(float delay)
		{
			if (_element != null)
				return _element.Show(delay);
			else
				return 0f;
		}

		public float Hide(float delay)
		{
			if (_element != null)
				return _element.Hide(delay);
			else
				return 0f;
		}

		public float ShowDelay
		{
			get { return _showDelay; }
			set { _showDelay = value; }
		}

		public float HideDelay
		{
			get { return _hideDelay; }
			set { _hideDelay = value; }
		}

		public bool ShowSync
		{
			get { return _showSync; }
			set { _showSync = value; }
		}

		public bool HideSync
		{
			get { return _hideSync; }
			set { _hideSync = value; }
		}

		public UIElement Element
		{
			get { return _element; }
			set { _element = value; }
		}
	}
}
