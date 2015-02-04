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
		#endregion

		#region Initialization
		public void Init(Func<Vector2, Vector2> convertPositionHandler, LayerMask mask)
		{
			_converPositionHandler = null;

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
		public void CreateListener(int id, Action click) 
		{
			InputListener listener = new InputListener(id, click);
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

			if (Input.GetMouseButtonDown(0))
			{
				Collider2D[] colliders = Physics2D.OverlapPointAll(_mousePosition, _mask.value);
				foreach (Collider2D c in colliders)
				{
					foreach (InputListener listener in _listeners)
					{
						if (listener.ID == c.GetInstanceID())
							listener.Call();
					}
				}
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
