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
		private Action<Vector3> _swipeHandler;
		private bool _pressed;
		private Vector2 _pressPosition;
		#endregion

		public InputListener(int id, Action clickHandler, Action pressHandler, Action releaseHandler, Action releaseOutsideHandler, Action<Vector3> swipeHandler = null)
		{
			_id = id;
			_clickHandler = clickHandler;
			_pressHandler = pressHandler;
			_releaseHandler = releaseHandler;
			_releaseOutsideHandler = releaseOutsideHandler;
			_swipeHandler = swipeHandler;
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

			// TODO: we need to take screen density (SD, HD, SHD) into account here for touches
			bool moved = Vector2.Distance(position, _pressPosition) > 3f;

			if (_pressed && !moved)
				Click();
			else if (_pressed && moved)
			{
				Vector2 diff = position - _pressPosition;	

				Vector3 direction = Vector3.zero;
				if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
				{
					if (diff.x > 0)
						direction = Vector3.right;
					else
						direction = Vector3.left;
				}

				if (_swipeHandler != null)
					_swipeHandler(direction);
			}

			_pressed = false;
		}

		public void ReleaseOutside(Vector2 position)
		{
			if (_pressed)
			{
				if (_releaseOutsideHandler != null)
					_releaseOutsideHandler();

				// TODO: we need to take screen density (SD, HD, SHD) into account here for touches
				bool moved = Vector2.Distance(position, _pressPosition) > 3f;

				if (moved)
				{
					Vector2 diff = position - _pressPosition;	

					Vector3 direction = Vector3.zero;
					if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
					{
						if (diff.x > 0)
							direction = Vector3.right;
						else
							direction = Vector3.left;
					}

					if (_swipeHandler != null)
						_swipeHandler(direction);
				}

				_pressed = false;
			}
		}
	}
}
