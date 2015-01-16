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
		#endregion

		public InputListener(int id, Action clickHandler)
		{
			_id = id;
			_clickHandler = clickHandler;
		}

		public int ID
		{
			get { return _id; }
		}

		public void Call()
		{
			if (_clickHandler != null)
				_clickHandler();
		}
	}
}
