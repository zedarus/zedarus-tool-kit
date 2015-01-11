using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Events
{
	public class Event
	{
		private int _id;
		private object _sender;

		public Event(int id)
		{
			_id = id;
		}

		public int ID
		{
			get { return _id; }
		}
	}

	public class Event<T1> : Event
	{
		private T1 _param1;

		public Event(int id, T1 param1) : base(id)
		{
			_param1 = param1;
		}

		public T1 Param1
		{
			get { return _param1; }
		}
	}

	public class Event<T1,T2> : Event
	{
		private T1 _param1;
		private T2 _param2;

		public Event(int id, T1 param1, T2 param2) : base(id)
		{
			_param1 = param1;
			_param2 = param2;
		}

		public T1 Param1
		{
			get { return _param1; }
		}

		public T2 Param2
		{
			get { return _param2; }
		}
	}

	public class Event<T1,T2,T3> : Event
	{
		private T1 _param1;
		private T2 _param2;
		private T3 _param3;

		public Event(int id, T1 param1, T2 param2, T3 param3) : base(id)
		{
			_param1 = param1;
			_param2 = param2;
			_param3 = param3;
		}

		public T1 Param1
		{
			get { return _param1; }
		}

		public T2 Param2
		{
			get { return _param2; }
		}

		public T3 Param3
		{
			get { return _param3; }
		}
	}

	public class Event<T1,T2,T3,T4> : Event
	{
		private T1 _param1;
		private T2 _param2;
		private T3 _param3;
		private T4 _param4;

		public Event(int id, T1 param1, T2 param2, T3 param3, T4 param4) : base(id)
		{
			_param1 = param1;
			_param2 = param2;
			_param3 = param3;
			_param4 = param4;
		}

		public T1 Param1
		{
			get { return _param1; }
		}

		public T2 Param2
		{
			get { return _param2; }
		}

		public T3 Param3
		{
			get { return _param3; }
		}

		public T4 Param4
		{
			get { return _param4; }
		}
	}
}
