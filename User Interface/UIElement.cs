using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInterface
{
	public class UIElement : MonoBehaviour
	{
		#region Parameters
		[SerializeField]
		private float _showAnimationDuration;
		[SerializeField]
		private float _hideAnimationDuration;
		[SerializeField]
		private List<Transform> _animatedObjects = new List<Transform>();
		private List<IUIElementAnimation> _animations = new List<IUIElementAnimation>();
		private bool _hasButton = true;
		private bool _displayed = false;
		#endregion

		#region Controls
		public void Init(bool reset = true)
		{
			foreach (Transform animatedObject in _animatedObjects)
			{
				if (animatedObject != null)
				{
					IUIElementAnimation animation = animatedObject.GetInterface<IUIElementAnimation>();
					if (animation != null) 
					{
						animation.Init();
						_animations.Add(animation);
					}
				}
			}

			if (reset) Reset();
		}

		public float Show()
		{
			_displayed = true;
			ToggleButton(true);
			StopAllCoroutines();

			float previousDuration = 0f;
			for (int i = 0; i < _animations.Count; i++)
			{
				IUIElementAnimation animation = _animations[i];
				StartCoroutine(ShowAnimationWithDelay(animation, previousDuration));
				previousDuration += animation.ShowDuration;
			}
			return _showAnimationDuration;
		}

		public float Show(float delay)
		{
			StopAllCoroutines();
			StartCoroutine(ShowWithDelay(delay));
			return _showAnimationDuration;
		}

		public float Hide()
		{
			_displayed = false;
			ToggleButton(false);
			StopAllCoroutines();

			float previousDuration = 0f;
			for (int i = _animations.Count - 1; i >= 0; i--)
			{
				IUIElementAnimation animation = _animations[i];
				StartCoroutine(HideAnimationWithDelay(animation, previousDuration));
				previousDuration += animation.HideDuration;
			}
			return _hideAnimationDuration;
		}

		public float Hide(float delay)
		{
			ToggleButton(false);
			StopAllCoroutines();
			StartCoroutine(HideWithDelay(delay));
			return _hideAnimationDuration;
		}

		private void Reset()
		{
			ToggleButton(false);
			foreach (IUIElementAnimation animation in _animations)
				animation.Reset();
		}
		#endregion

		#region Queries
		public bool Visible
		{
			get { return _displayed; }
		}
		#endregion

		#region Helpers
		private void ToggleButton(bool status)
		{
			if (_hasButton)
			{
				Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
				if (colliders.Length > 0) 
					foreach (Collider2D collider in colliders) collider.enabled = status;
				else
					_hasButton = false;

				UIButton[] buttons = GetComponentsInChildren<UIButton>();
				if (buttons.Length > 0) 
					foreach (UIButton button in buttons) button.SetState(UIButton.State.Normal, true);
				else
					_hasButton = false;
			}
		}

		private IEnumerator ShowWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			Show();
		}

		private IEnumerator HideWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			Hide();
		}

		private IEnumerator ShowAnimationWithDelay(IUIElementAnimation animation, float delay)
		{
			yield return new WaitForSeconds(delay);
			animation.Show();
		}

		private IEnumerator HideAnimationWithDelay(IUIElementAnimation animation, float delay)
		{
			yield return new WaitForSeconds(delay);
			animation.Hide();
		}
		
		public IUIElementAnimation GetAnimationByID(int id, IUIElementAnimation[] allAnimations)
		{
			for (int i = 0; i < allAnimations.Length; i++)
			{
				if (allAnimations[i].ObjectReference.GetInstanceID() == id)
					return allAnimations[i];
			}
			return null;
		}
		#endregion

		#if UNITY_EDITOR
		public List<Transform> AnimatedObjects
		{
			get { return _animatedObjects; }
			set { _animatedObjects = value; }
		}

		public float ShowAnimationDuration
		{
			get { return _showAnimationDuration; }
			set { _showAnimationDuration = value; }
		}

		public float HideAnimationDuration
		{
			get { return _hideAnimationDuration; }
			set { _hideAnimationDuration = value; }
		}
		#endif
	}
}
