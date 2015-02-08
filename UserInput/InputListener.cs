using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.UserInput
{
	public class InputListener
	{
		#region Properties
		private int _id;
		private Action _clickHandler;
		private Action _pressHandler;
		private Action _releaseHandler;
		private Action _releaseOutsideHandler;
		private bool _pressed;
		private Vector2 _pressPosition;
		#endregion

		public InputListener(int id, Action clickHandler, Action pressHandler, Action releaseHandler, Action releaseOutsideHandler)
		{
			_id = id;
			_clickHandler = clickHandler;
			_pressHandler = pressHandler;
			_releaseHandler = releaseHandler;
			_releaseOutsideHandler = releaseOutsideHandler;
		}

		public int ID
		{
			get { return _id; }
		}

		private void Click()
		{
			if (_clickHandler != null)
				_clickHandler();
		}

		public void Press(Vector2 position)
		{
			_pressPosition = position;

			if (_pressHandler != null)
				_pressHandler();

			_pressed = true;
		}

		public void Release(Vector2 position)
		{
			if (_releaseHandler != null)
				_releaseHandler();

			bool moved = Vector2.Distance(position, _pressPosition) > 0.1f;

			if (_pressed && !moved)
				Click();

			_pressed = false;
		}

		public void ReleaseOutside()
		{
			if (_pressed)
			{
				if (_releaseOutsideHandler != null)
					_releaseOutsideHandler();

				_pressed = false;
			}
		}
	}
}
