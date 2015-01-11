using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Events
{
	public class Event
	{
		private int _id;
		private object _sender;
		private object[] _parameters = null;

		public Event(int id)
		{
			_id = id;
		}

		public Event(int id, params object[] args)
		{
			_id = id;
			_parameters = args;
		}

		public int ID
		{
			get { return _id; }
		}

		public object[] Parameters
		{
			get { return _parameters; }
		}
	}
}
