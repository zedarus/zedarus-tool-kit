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
		private System.Action<object[]> _handlerWithParameters;

		public EventListener(int e, System.Action handler, bool consume, bool oneTime)
		{
			_event = e;
			_consume = consume;
			_oneTime = oneTime;
			_handler = handler;
			_handlerWithParameters = null;
			_expired = false;
		}

		public EventListener(int e, System.Action<object[]> handler, bool consume, bool oneTime)
		{
			_event = e;
			_consume = consume;
			_oneTime = oneTime;
			_handler = null;
			_handlerWithParameters = handler;
			_expired = false;
		}

		public void Call(object[] parameters = null)
		{
			if (parameters != null && _handlerWithParameters != null)
				_handlerWithParameters(parameters);
			else
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

		public System.Action Handler
		{
			get { return _handler; }
		}

		public System.Action<object[]> HandlerAlt
		{
			get { return _handlerWithParameters; }
		}
	}
}
