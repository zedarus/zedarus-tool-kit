using UnityEngine;
using System;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInput
{
	public class InputManager : SimpleSingleton<InputManager>
	{
		#region Properties
		private List<InputListener> _listeners;
		private Func<Vector2, Vector2> _converPositionHandler;
		private LayerMask _mask;
		private Vector2 _mousePosition;
		private Collider2D[] _colliders;
		private bool _processOnlyFirstCollider;
		#endregion

		#region Initialization
		public void Init(Func<Vector2, Vector2> convertPositionHandler, LayerMask mask, bool processOnlyFirstCollider)
		{
			_processOnlyFirstCollider = processOnlyFirstCollider;
			_converPositionHandler = null;
			_colliders = new Collider2D[64];

			if (_listeners != null)
			{
				_listeners.Clear();
				_listeners = null;
			}

			_listeners = new List<InputListener>();
			_converPositionHandler = convertPositionHandler;
			_mask = mask;
		}

		public void Destroy()
		{
			_converPositionHandler = null;

			if (_listeners != null)
			{
				_listeners.Clear();
				_listeners = null;
			}
		}
		#endregion

		#region Controls
		public void CreateListener(int id, Action click, Action press, Action release, Action releaseOutside) 
		{
			InputListener listener = new InputListener(id, click, press, release, releaseOutside);
			_listeners.Add(listener);
		}

		public void RemoveListener(int id)
		{
			for (int i = _listeners.Count - 1; i >= 0; i--)
			{
				if (_listeners[i].ID == id)
					_listeners.RemoveAt(i);
			}
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
				int numberOfColliders = Physics2D.OverlapPointNonAlloc(_mousePosition, _colliders, _mask.value);
				List<int> collidersIDs = new List<int>();
				for (int i = 0; i < numberOfColliders; i++)
				{
					collidersIDs.Add(_colliders[i].GetInstanceID());
					if (_processOnlyFirstCollider) break;
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
							listener.ReleaseOutside();
					}
				}

				collidersIDs.Clear();
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
