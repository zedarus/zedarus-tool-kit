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

		public void Press()
		{
			if (_pressHandler != null)
				_pressHandler();

			_pressed = true;
		}

		public void Release()
		{
			if (_releaseHandler != null)
				_releaseHandler();

			if (_pressed)
				Click();

			_pressed = false;
		}

		public void ReleaseOutside()
		{
			if (_releaseOutsideHandler != null)
				_releaseOutsideHandler();

			_pressed = false;
		}
	}
}
