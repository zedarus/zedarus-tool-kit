using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.UserInput
{
	public class InputGestureListener
	{
		#region Properties
		private Action<Vector3> _swipeHandler;
		#endregion

		public InputGestureListener(Action<Vector3> swipeHandler)
		{
			_swipeHandler = swipeHandler;
		}

		public void Swipe(Vector3 direction)
		{
			if (_swipeHandler != null)
				_swipeHandler(direction);
		}
	}
}
