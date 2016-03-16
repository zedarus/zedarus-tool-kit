using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.StateMachines;

namespace Zedarus.ToolKit.UI
{
	public class UIScreenManager<T> where T : UIScreen
	{
		#region Parameters
		private Dictionary<int, T> _screens = new Dictionary<int, T>();
		#endregion

		#region Properties
		private StateMachine _state;
		private Queue<int> _queue;
		private Queue<IUIScreenData> _dataQueue;
		private bool _queueSupported;
		private GameObject _clicksBlocker;
		private Animator _backgroundAnimator;
		#endregion

		#region Delegates
		public delegate void ScreenOpenDelegate(int screenID);
		public delegate void ScreenClosedDelegate(int screenID);
		public delegate void ScreenOpeningDelegate(int screenID);
		public delegate void ScreenClosingDelegate(int screenID);
		#endregion

		#region Settings
		private const int NoScreen = 0;
		#endregion

		#region Initialization
		public void Init(List<T> screens, int firstIndex, bool firstScreenInstantAnimation, bool supportQueue, GameObject clicksBlocker, Animator backgroundAnimator)
		{
			_queueSupported = supportQueue;
			_queue = new Queue<int>(supportQueue ? 50 : 1);
			_dataQueue = new Queue<IUIScreenData>(supportQueue ? 50 : 1);
			_clicksBlocker = clicksBlocker;
			_backgroundAnimator = backgroundAnimator;

			_state = new StateMachine();
			_state.CreateState(NoScreen, NoScreenUpdate, NoScreenEnter, NoScreenExit, null);

			T screen;
			for (int i = 0; i < screens.Count; i++)
			{
				screen = screens[i];

				if (screen != null)
				{
					screen.Init();
					_state.CreateState(screen.ID, screen.Cycle, screen.Open, screen.Close, null);
					_screens.Add(screen.ID, screen);
					CreateEventListenersForScreen(screen);
				}
			}

			ChangeState(NoScreen);

			if (firstIndex >= 0 && firstIndex < screens.Count && screens[firstIndex] != null)
			{
				screens[firstIndex].MarkAsFirst(firstScreenInstantAnimation);
				Open(screens[firstIndex].ID, null);
			}
		}

		public void Destroy()
		{
			foreach (KeyValuePair<int, T> screen in _screens)
			{
				RemoveEventListenersForScreen(screen.Value);
			}
		}
		#endregion

		#region Controls
		public void Update(float dt)
		{
			if (_state != null)
				_state.Update(dt);
		}

		public void Open(int id, IUIScreenData data)
		{
			if (EmptyScreen)
			{
				ChangeState(id);
				SetDataToScreen(id, data);
			}
			else
			{
				QueueAdd(id, data);

				if (!QueueSupported)
					CloseCurrentScreen();
				else if (QueueSupported && CurrentScreen != null && CurrentScreen.SkipInQueue)
					CloseCurrentScreen();
			}
		}

		public void Open(string customID, IUIScreenData data)
		{
			customID = customID.Trim();

			int id = 0;
			string stringID = null;
			foreach (KeyValuePair<int, T> screen in _screens)
			{
				stringID = screen.Value.CustomID;
				if (stringID != null && stringID.Equals(customID))
				{
					id = screen.Value.ID;
					break;
				}
			}

			if (id > 0)
				Open(id, data);
			else
				Debug.LogWarning("Screen with custom ID \"" + customID + "\" does not exist in current scene or not registered in UIManager. This can also be caused by spelling error.");
		}

		public bool HandleBackPress()
		{
			if (CurrentState!=NoScreen) {
				return CurrentScreen.BackKeyPressed();
			}
			return false;
		}

		private void CloseCurrentScreen()
		{
			if (CurrentScreen != null)
			{
				CurrentScreen.Close();
			}
		}

		private void SwitchToEmptyScreen()
		{
			if (CurrentState != NoScreen)
			{
				ChangeState(NoScreen);
				QueueNext();
			}
		}

		private void SetDataToScreen(int screenID, IUIScreenData data)
		{
			if (_screens.ContainsKey(screenID))
			{
				_screens[screenID].SetCustomData(data);
			}
		}
		#endregion

		#region Event Listeners
		private void CreateEventListenersForScreen(T screen)
		{
			screen.ScreenOpen += OnScreenOpen;
			screen.ScreenClosed += OnScreenClosed;
			screen.ScreenOpening += OnScreenOpening;
			screen.ScreenClosing += OnScreenClosing;
		}

		private void RemoveEventListenersForScreen(T screen)
		{
			screen.ScreenOpen -= OnScreenOpen;
			screen.ScreenClosed -= OnScreenClosed;
			screen.ScreenOpening -= OnScreenOpening;
			screen.ScreenClosing -= OnScreenClosing;
		}
		#endregion

		#region Helpers
		private void ChangeState(int newState)
		{
			_state.ChangeState(newState);

			bool displayBackground = false;
			switch (newState)
			{
				case NoScreen:
					displayBackground = false;
					break;
				default:
					displayBackground = true;
					break;
			}

			if (_clicksBlocker != null)
				_clicksBlocker.SetActive(displayBackground);

			if (_backgroundAnimator != null && _backgroundAnimator.isInitialized)
			{
				_backgroundAnimator.ResetTrigger(IDs.Animation.OpenTrigger);
				_backgroundAnimator.ResetTrigger(IDs.Animation.CloseTrigger);
				_backgroundAnimator.SetTrigger(displayBackground ? IDs.Animation.OpenTrigger : IDs.Animation.CloseTrigger);
			}
		}
		#endregion

		#region Event Handlers
		private void OnScreenOpen(int screenID) {}

		private void OnScreenClosed(int screenID)
		{
			SwitchToEmptyScreen();
		}

		private void OnScreenOpening(int screenID) {}

		private void OnScreenClosing(int screenID) {}
		#endregion

		#region Queue
		private void QueueAdd(int screenID, IUIScreenData data)
		{
			_queue.Enqueue(screenID);
			_dataQueue.Enqueue(data);
		}

		private void QueueNext()
		{
			if (_queue.Count > 0)
			{
				int nextScreenID = _queue.Dequeue();
				IUIScreenData data = _dataQueue.Dequeue();

				if (QueueSupported && _screens[nextScreenID] != null && _screens[nextScreenID].SkipInQueue && _queue.Count > 0)
				{
					QueueNext();
					return;
				}
				else
				{
					Open(nextScreenID, data);
				}
			}
		}
		#endregion

		#region State
		private void NoScreenUpdate(float delta) {}
		private void NoScreenEnter() {}
		private void NoScreenExit() {}
		#endregion

		#region Getters
		private int CurrentState
		{
			get { return _state.CurrentState; }
		}

		private UIScreen CurrentScreen
		{
			get { return _screens[CurrentState]; }
		}

		public bool EmptyScreen
		{
			get { return CurrentState == NoScreen; }
		}

		private bool QueueSupported
		{
			get { return _queueSupported; }
		}
		#endregion
	}
}
