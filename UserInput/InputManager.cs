using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInput
{
	public class InputManager : SimpleSingleton<InputManager>
	{
		#region Properties
		private List<InputListener> _listeners;
		private List<InputListener> _newListeners;
		private List<int> _listenersToRemove;
		private Func<Vector2, Vector2> _converPositionHandler;
		private Func<Vector2, Vector2> _converIgnorePositionHandler;
		private LayerMask _mask;
		private LayerMask _ignoreMask;
		private Vector2 _mousePosition;
		private Collider2D[] _colliders;
		private bool _processOnlyFirstCollider;
		#endregion

		#region Initialization
		public void Init(Func<Vector2, Vector2> convertPositionHandler, Func<Vector2, Vector2> convertIgnorePositionHandler, LayerMask mask, LayerMask ignoreMask, bool processOnlyFirstCollider)
		{
			_processOnlyFirstCollider = processOnlyFirstCollider;
			_converPositionHandler = null;
			_converIgnorePositionHandler = null;
			_colliders = new Collider2D[64];

			if (_listeners != null)
			{
				_listeners.Clear();
				_listeners = null;
			}

			if (_newListeners != null)
			{
				_newListeners.Clear();
				_newListeners = null;
			}

			if (_listenersToRemove != null)
			{
				_listenersToRemove.Clear();
				_listenersToRemove = null;
			}

			_listeners = new List<InputListener>();
			_newListeners = new List<InputListener>();
			_listenersToRemove = new List<int>();

			_converPositionHandler = convertPositionHandler;
			_converIgnorePositionHandler = convertIgnorePositionHandler;
			_mask = mask;
			_ignoreMask = ignoreMask;
		}

		public void Destroy()
		{
			_converPositionHandler = null;

			if (_listeners != null)
			{
				_listeners.Clear();
				_listeners = null;
			}

			if (_newListeners != null)
			{
				_newListeners.Clear();
				_newListeners = null;
			}

			if (_listenersToRemove != null)
			{
				_listenersToRemove.Clear();
				_listenersToRemove = null;
			}
		}
		#endregion

		#region Controls
		public void CreateListener(Collider2D collider, Action click, Action press, Action release, Action releaseOutside, Action<Vector3> swipeGesture = null)
		{
			CreateListener(collider.GetInstanceID(), click, press, release, releaseOutside, swipeGesture);
		}

		public void CreateListener(int id, Action click, Action press, Action release, Action releaseOutside, Action<Vector3> swipeGesture = null) 
		{
			InputListener listener = new InputListener(id, click, press, release, releaseOutside, swipeGesture);
			_newListeners.Add(listener);
		}

		public void RemoveListener(Collider2D collider)
		{
			RemoveListener(collider.GetInstanceID());
		}

		public void RemoveListener(int id)
		{
			_listenersToRemove.Add(id);
		}

		public void Update()
		{
			_mousePosition = Input.mousePosition;

			if (_converPositionHandler != null)
				_mousePosition = _converPositionHandler(_mousePosition);

			//bool click = Input.GetMouseButton(0);
			bool press = Input.GetMouseButtonDown(0);
			bool release = Input.GetMouseButtonUp(0);

			if (press || release)
			{
				if (press)
				{
					Vector2 ignorePosition = Vector2.zero;
					if (_converIgnorePositionHandler != null)
						ignorePosition = _converIgnorePositionHandler(Input.mousePosition);
					int ignoreColliers = Physics2D.OverlapPointNonAlloc(ignorePosition, _colliders, _ignoreMask.value);

					if (ignoreColliers > 0) return;
				}

				int numberOfColliders = Physics2D.OverlapPointNonAlloc(_mousePosition, _colliders, _mask.value);
				List<int> collidersIDs = new List<int>();
				for (int i = 0; i < numberOfColliders; i++)
				{
					collidersIDs.Add(_colliders[i].GetInstanceID());
					if (_processOnlyFirstCollider && press) break;
				}

				foreach (InputListener listener in _listeners)
				{
					if (collidersIDs.Contains(listener.ID))
					{
						if (press)
							listener.Press(_mousePosition);
						if (release)
							listener.Release(_mousePosition);
					}
					else
					{
						if (release)
							listener.ReleaseOutside(_mousePosition);
					}
				}

				collidersIDs.Clear();
			}

			// Remove existing and add new listeners for next frame
			if (_listenersToRemove != null)
			{
				foreach (int id in _listenersToRemove)
				{
					for (int i = _listeners.Count - 1; i >= 0; i--)
					{
						if (_listeners[i].ID == id)
							_listeners.RemoveAt(i);
					}
				}
				_listenersToRemove.Clear();
			}

			if (_newListeners.Count > 0)
			{
				_listeners.AddRange(_newListeners);
				_newListeners.Clear();
			}
		}
		#endregion

		#region Getters
		public Vector2 MousePosition
		{
			get { return _mousePosition; }
		}
		#endregion
	}
}
