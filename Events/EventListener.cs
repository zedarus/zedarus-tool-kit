using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Events
{
	public class EventListener
	{
		private int _event;
		private bool _consume;
		private bool _oneTime;
		private bool _expired;
		private System.Action _handler;

		public EventListener(int e, System.Action handler, bool consume, bool oneTime)
		{
			_event = e;
			_consume = consume;
			_oneTime = oneTime;
			_handler = handler;
			_expired = false;
		}

		public virtual void Call(Event e)
		{
			if (_handler != null)
				_handler();

			if (_oneTime)
				_expired = true;
		}

		public int Event
		{
			get { return _event; }
		}

		public bool Expired
		{
			get { return _expired; }
		}

		public bool Consume
		{
			get { return _consume; }
		}

		public string Handler
		{
			get { return _handler.Method.Name; }
		}
	}

	public class EventListener<T> : EventListener
	{
		private System.Action<T> _handler;

		public EventListener(int e, System.Action<T> handler, bool consume, bool oneTime) : base(e, null, consume, oneTime)
		{
			_handler = handler;
		}

		public override void Call(Event e)
		{
			Event<T> ec = e as Event<T>;

			if (ec == null)
			{
				Debug.LogWarning("Incorrect number of parameters in event");
				return;
			}

			if (_handler != null)
				_handler(ec.Param1);

			base.Call(null);
		}

		public new string Handler
		{
			get { return _handler.Method.Name; }
		}
	}

	public class EventListener<T1,T2> : EventListener
	{
		private System.Action<T1,T2> _handler;
		
		public EventListener(int e, System.Action<T1,T2> handler, bool consume, bool oneTime) : base(e, null, consume, oneTime)
		{
			_handler = handler;
		}

		public override void Call(Event e)
		{
			Event<T1,T2> ec = e as Event<T1,T2>;

			if (ec == null)
			{
				Debug.LogWarning("Incorrect number of parameters in event");
				return;
			}

			if (_handler != null)
				_handler(ec.Param1, ec.Param2);
			
			base.Call(null);
		}
		
		public new string Handler
		{
			get { return _handler.Method.Name; }
		}
	}

	public class EventListener<T1,T2,T3> : EventListener
	{
		private System.Action<T1,T2,T3> _handler;
		
		public EventListener(int e, System.Action<T1,T2,T3> handler, bool consume, bool oneTime) : base(e, null, consume, oneTime)
		{
			_handler = handler;
		}
		
		public override void Call(Event e)
		{
			Event<T1,T2,T3> ec = e as Event<T1,T2,T3>;

			if (ec == null)
			{
				Debug.LogWarning("Incorrect number of parameters in event");
				return;
			}

			if (_handler != null)
				_handler(ec.Param1, ec.Param2, ec.Param3);
			
			base.Call(null);
		}
		
		public new string Handler
		{
			get { return _handler.Method.Name; }
		}
	}

	public class EventListener<T1,T2,T3,T4> : EventListener
	{
		private System.Action<T1,T2,T3,T4> _handler;
		
		public EventListener(int e, System.Action<T1,T2,T3,T4> handler, bool consume, bool oneTime) : base(e, null, consume, oneTime)
		{
			_handler = handler;
		}
		
		public override void Call(Event e)
		{
			Event<T1,T2,T3,T4> ec = e as Event<T1,T2,T3,T4>;

			if (ec == null)
			{
				Debug.LogWarning("Incorrect number of parameters in event");
				return;
			}

			if (_handler != null)
				_handler(ec.Param1, ec.Param2, ec.Param3, ec.Param4);
			
			base.Call(null);
		}
		
		public new string Handler
		{
			get { return _handler.Method.Name; }
		}
	}
}
