using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.StateMachines;

namespace Zedarus.ToolKit.UI
{
	[System.Serializable]
	public class UIManager : MonoBehaviour
	{
		// TODO: must be singleton!
		#region TEMP
		static private UIManager _instance;
		static public UIManager Instance
		{
			get { return _instance; }
		}
		#endregion

		#region Parameters
		[SerializeField] private bool _autoInit = false;
		[SerializeField] private float _autoInitDelay = 0.5f;
		[SerializeField] private List<UIScreen> _screensList = new List<UIScreen>();
		[SerializeField] private List<UIPopup> _popupsList = new List<UIPopup>();
		[SerializeField] private int _firstScreenIndex = 0;
		[SerializeField] private bool _firstScreenInstantAnimation = false;
		[SerializeField] private Image _popupsClickBlocker;
		[SerializeField] private Animator _popupsBackgroundAnimator;
		#endregion

		#region Properties
		private bool _initialized = false;
		private float _timeSinceStart = 0f;
		private UIScreenManager<UIScreen> _screens;
		private UIScreenManager<UIPopup> _popups;
		#endregion

		#region Unity Methods
		private void Start()
		{
			_instance = this;	// TODO: remove this later
			_timeSinceStart = 0f;
		}

		private void Update()
		{
			if (Initialized)
			{
				_screens.Update(Time.deltaTime);
			}
			else if (_autoInit)
			{
				_timeSinceStart += Time.deltaTime;
				if (_timeSinceStart > _autoInitDelay)
					Init();
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (Initialized && !_popups.HandleBackPress())
				{
					_screens.HandleBackPress();
				}
			}
		}

		private void OnDestroy()
		{
			if (Initialized)
				Destroy();
		}
		#endregion

		#region Initialization
		public void Init()
		{
			_screens = new UIScreenManager<UIScreen>();
			_screens.Init(_screensList, _firstScreenIndex, _firstScreenInstantAnimation, false, null, null);

			_popups = new UIScreenManager<UIPopup>();
			_popups.Init(_popupsList, -1, false, true, (_popupsClickBlocker != null) ? _popupsClickBlocker.gameObject : null, _popupsBackgroundAnimator);

			#if !UNITY_EDITOR
			_screensList.Clear();
			_screensList = null;

			_popupsList.Clear();
			_popupsList = null;
			#endif

			_initialized = true;
		}

		public void Destroy()
		{
			_screens.Destroy();
			_popups.Destroy();
		}
		#endregion

		#region Controls - Screens
		private void OpenScreen(int screenID, IUIScreenData data = null)
		{
			if (Initialized)
				_screens.Open(screenID, data);
		}

		public void OpenScreen(string customScreenID, IUIScreenData data = null)
		{
			if (Initialized)
				_screens.Open(customScreenID, data);
		}

		public void OpenScreen(UIScreen screen, IUIScreenData data = null)
		{
			OpenScreen(screen.ID, data);
		}
		#endregion

		#region Controls - Popups
		public void OpenPopup(int popupID, IUIScreenData data = null)
		{
			if (Initialized)
				_popups.Open(popupID, data);
		}

		public void OpenPopup(string customPopupID, IUIScreenData data = null)
		{
			if (Initialized)
				_popups.Open(customPopupID, data);
		}

		public void OpenPopup(UIPopup popup, IUIScreenData data = null)
		{
			OpenPopup(popup.ID, data);
		}
		#endregion

		#region Getters
		private bool Initialized
		{
			get { return _initialized; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public void AddScreen()
		{
			_screensList.Add(null);
		}

		public void RemoveScreen(int index)
		{
			if (index >= 0 && index < _screensList.Count)
				_screensList.RemoveAt(index);
		}

		public List<UIScreen> Screens
		{
			get { return _screensList; }
		}

		public int FirstScreenIndex
		{
			set { _firstScreenIndex = value; }
			get { return _firstScreenIndex; }
		}

		public bool FirstScreenInstantAnimation
		{
			set { _firstScreenInstantAnimation = value; }
			get { return _firstScreenInstantAnimation; }
		}

		public void AddPopup()
		{
			_popupsList.Add(null);
		}

		public void RemovePopup(int index)
		{
			if (index >= 0 && index < _popupsList.Count)
				_popupsList.RemoveAt(index);
		}

		public List<UIPopup> Popups
		{
			get { return _popupsList; }
		}

		public bool AutoInit
		{
			get { return _autoInit; }
			set { _autoInit = value; }
		}

		public float AutoInitDelay
		{
			get { return _autoInitDelay; }
			set { _autoInitDelay = value; }
		}

		public Image PopupsClickBlocker
		{
			get { return _popupsClickBlocker; }
			set { _popupsClickBlocker = value; }
		}

		public Animator PopupsBackgroundAnimator
		{
			get { return _popupsBackgroundAnimator; }
			set { _popupsBackgroundAnimator = value; }
		}
		#endregion
		#endif
	}
}
