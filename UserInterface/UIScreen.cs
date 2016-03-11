using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System;

namespace Zedarus.ToolKit.UI
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CanvasGroup))]
	public class UIScreen : MonoBehaviour
	{
		#region Static Properties
		private static int ID_COUNTER = 1;
		#endregion

		#region Settings
		private const float defaultAnimationScale = 1f;
		public enum AnimationState
		{
			Closed = 1,
			Closing = 2,
			Open = 3,
			Opening = 4,
		}
		#endregion

		#region Parameters
		[SerializeField] private Animator _animator;
		[SerializeField] private string _customID = "";
		[SerializeField] private bool _useCustomID = false;
		[SerializeField] private bool _sendOpenEventOnOpening = false;
		[SerializeField] private bool _sendClosedEventOnClosing = false;
		[SerializeField] private bool _skipInQueue = false;
		#endregion

		#region Properties
		private int _id;
		private bool _open = false;
		private AnimationState _animationState = AnimationState.Closed;
		private bool _skipFirstOpenAnimation = false;
		private CanvasGroup _canvasGroup = null;
		#endregion

		#region Events
		public event UIScreenManager<UIScreen>.ScreenOpenDelegate ScreenOpen;
		public event UIScreenManager<UIScreen>.ScreenClosedDelegate ScreenClosed;
		public event UIScreenManager<UIScreen>.ScreenOpeningDelegate ScreenOpening;
		public event UIScreenManager<UIScreen>.ScreenClosingDelegate ScreenClosing;
		#endregion

		#region Initialization
		public void Init()
		{
			_id = ID_COUNTER++;

			if (_animator == null)
				_animator = GetComponent<Animator>();

			if (_canvasGroup == null)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
				if (_canvasGroup != null) _canvasGroup.alpha = 0f;
			}

			Assert.IsTrue(_animator, "Animator for UI screen needs to be set!");
		}
		#endregion

		#region Controls
		public virtual void Cycle(float deltaTime)
		{

		}

		/// <summary>
		/// This callback function is only used in Unity animation events system.
		/// Please avoid calling it anywhere in your code for stability.
		/// </summary>
		/// <param name="state">New animation state</param>
		public void SetAnimationState(AnimationState state)
		{
			if (state != _animationState)
			{
				_animationState = state;
				switch (state)
				{
					case AnimationState.Closed:
						ResetCustomData();
						SendClosedEvent();
						break;
					case AnimationState.Open:
						SendOpenEvent();
						break;
					case AnimationState.Closing:
						SendClosingEvent();
						break;
					case AnimationState.Opening:
						SendOpeningEvent();
						break;
				}
			}
		}

		public void SetCustomData(IUIScreenData data)
		{
			if (data != null)
			{
				ProcessCustomData(data);
				data.Clear();
				data = null;
			}
		}

		public void MarkAsFirst(bool skipAnimation)
		{
			_skipFirstOpenAnimation = skipAnimation;
		}

		public void Open()
		{
			Open(defaultAnimationScale);
		}

		private void Open(float animationSpeedScale)
		{
			if (!IsOpen)
			{
				if (_skipFirstOpenAnimation)
				{
					animationSpeedScale = 1000f;
					_skipFirstOpenAnimation = false;
				}

				IsOpen = true;
				_animator.SetFloat(IDs.Animation.OpenAnimationSpeedScale, animationSpeedScale);
				_animator.SetTrigger(IDs.Animation.OpenTrigger);
			}
		}

		public void Close()
		{
			Close(defaultAnimationScale);
		}

		private void Close(float animationSpeedScale)
		{
			if (IsOpen)
			{
				IsOpen = false;
				_animator.SetFloat(IDs.Animation.CloseAnimationSpeedScale, animationSpeedScale);
				_animator.SetTrigger(IDs.Animation.CloseTrigger);
			}
		}

		virtual protected void ProcessCustomData(IUIScreenData customData) {}
		virtual public void Refresh() {}

		public virtual bool BackKeyPressed()
		{
			return true;
		}

		public void OnBackKeyPressed()
		{
			BackKeyPressed();
		}

		virtual protected void ResetCustomData() {}
		#endregion

		#region Getters
		public int ID
		{
			get { return _id; }
		}

		public string CustomID
		{
			get
			{
				if (_useCustomID && _customID != null && _customID.Length > 0)
					return _customID.Trim();
				else
					return null;
			}
		}

		public bool IsOpen
		{
			get { return _open; }
			private set { _open = value; }
		}

		public virtual bool SkipInQueue
		{
			get { return _skipInQueue; }
		}
		#endregion

		#region Event Sending
		private void SendOpenEvent()
		{
			if (!_sendOpenEventOnOpening)
			{
				if (ScreenOpen != null)
					ScreenOpen(ID);
			}
		}

		private void SendOpeningEvent()
		{
			if (_canvasGroup != null)
				_canvasGroup.alpha = 1f;

			if (ScreenOpening != null) 
			{
				UpdateTranslatedTexts();
				ScreenOpening(ID);
			}

			if (_sendOpenEventOnOpening)
			{
				if (ScreenOpen != null)
					ScreenOpen(ID);
			}
		}

		private void UpdateTranslatedTexts()
		{
			/*var children = transform.GetAllComponentsInChildren<LocalizeText>();
			foreach (LocalizeText text in children)
			{
				text.TranslateCurrentText();
			}*/
		}

		private void SendClosedEvent()
		{
			if (_canvasGroup != null)
				_canvasGroup.alpha = 0f;

			if (!_sendClosedEventOnClosing)
			{
				if (ScreenClosed != null)
					ScreenClosed(ID);
			}
		}

		private void SendClosingEvent()
		{
			if (ScreenClosing != null)
				ScreenClosing(ID);

			if (_sendClosedEventOnClosing)
			{
				if (ScreenClosed != null)
					ScreenClosed(ID);
			}
		}
		#endregion
	}
}
	