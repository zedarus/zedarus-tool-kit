using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Helpers;

namespace Zedarus.ToolKit.Events
{
	public class EventManager : SimplePrivateSingleton<EventManager>
	{
		private EventRegister _register;
		private List<Event> _events;
		private List<EventListener> _listeners;

		public EventManager()
		{
			_register = new EventRegister();
			_events = new List<Event>();
			_listeners = new List<EventListener>();
			//InitSceneObject();
		}

		#region Controls
		static public bool RegisterEvent(int e)
		{
			return Instance.AddEventToRegister(e);
		}

		static public void SendEvent(int e)
		{
			Instance.CreateEvent(e);
		}

		static public void SendEvent<T1>(int e, T1 param1)
		{
			Instance.CreateEvent<T1>(e, param1);
		}

		static public void SendEvent<T1,T2>(int e, T1 param1, T2 param2)
		{
			Instance.CreateEvent<T1,T2>(e, param1, param2);
		}

		static public void SendEvent<T1,T2,T3>(int e, T1 param1, T2 param2, T3 param3)
		{
			Instance.CreateEvent<T1,T2,T3>(e, param1, param2, param3);
		}

		static public void SendEvent<T1,T2,T3,T4>(int e, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			Instance.CreateEvent<T1,T2,T3,T4>(e, param1, param2, param3, param4);
		}

		/// <summary>
		/// Adds the listener.
		/// </summary>
		/// <param name="e">Event id.</param>
		/// <param name="handler">Handler method.</param>
		/// <param name="consume">If set to <c>true</c>, then event will be consumed and will not trigger other listeners.</param>
		/// <param name="oneTime">If set to <c>true</c>, handler will be only triggered once.</param>
		static public void AddListener(int e, System.Action handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener(e, handler, consume, oneTime);
		}

		static public void AddListener<T>(int e, System.Action<T> handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener<T>(e, handler, consume, oneTime);
		}

		static public void AddListener<T1,T2>(int e, System.Action<T1,T2> handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener<T1,T2>(e, handler, consume, oneTime);
		}

		static public void AddListener<T1,T2,T3>(int e, System.Action<T1,T2,T3> handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener<T1,T2,T3>(e, handler, consume, oneTime);
		}

		static public void AddListener<T1,T2,T3,T4>(int e, System.Action<T1,T2,T3,T4> handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener<T1,T2,T3,T4>(e, handler, consume, oneTime);
		}

		static public void RemoveListener(int e, System.Action handler)
		{
			Instance.DestroyListener(e, handler);
		}

		static public void RemoveListener<T1>(int e, System.Action<T1> handler)
		{
			Instance.DestroyListener<T1>(e, handler);
		}

		static public void RemoveListener<T1,T2>(int e, System.Action<T1,T2> handler)
		{
			Instance.DestroyListener<T1,T2>(e, handler);
		}

		static public void RemoveListener<T1,T2,T3>(int e, System.Action<T1,T2,T3> handler)
		{
			Instance.DestroyListener<T1,T2,T3>(e, handler);
		}

		static public void RemoveListener<T1,T2,T3,T4>(int e, System.Action<T1,T2,T3,T4> handler)
		{
			Instance.DestroyListener<T1,T2,T3,T4>(e, handler);
		}

		static public void Update()
		{
			Instance.ProcessEvents();
		}
		
		/// <summary>
		/// Removes all unprocessed events and listeners
		/// </summary>
		static public void Clear() 
		{
			Instance.ClearEventsAndListeners();
		}

		static public bool IsListenerExists(int e)
		{
			return Instance.GetListenersCountForEvent(e) > 0;
		}
		#endregion

		#region Events
		public void ProcessEvents()
		{
			Event eventObject;
			EventListener listener;
			for (int e = _events.Count - 1; e >= 0; e--)
			{
				eventObject = _events[e];
				for (int l = _listeners.Count - 1; l >= 0; l--)
				{
					listener = _listeners[l];
					if (listener.Event == eventObject.ID)
					{
						listener.Call(eventObject);

						if (listener.Expired)
							_listeners.RemoveAt(l);

						if (listener.Consume)
						{
							_events.RemoveAt(e);
							break;
						}
					}
				}
			}

			// TODO: add lifelength for unprocessed events so they are not removed immidately if not processed
			_events.Clear();
		}

		public bool AddEventToRegister(int e)
		{
			return _register.Register(e);
		}

		public void CreateEvent(int e)
		{
			if (_register.IsRegistered(e))
			{
				_events.Add(new Event(e));
				UpdateSceneObject();
				ProcessEvents();
			} else
				Debug.LogError("Can't send event with ID " + e + " because it's not registerd");
		}

		public void CreateEvent<T1>(int e, T1 param1)
		{
			if (_register.IsRegistered(e))
			{
				_events.Add(new Event<T1>(e, param1));
				UpdateSceneObject();
				ProcessEvents();
			} else
				Debug.LogError("Can't send event with ID " + e + " because it's not registerd");
		}

		public void CreateEvent<T1,T2>(int e, T1 param1, T2 param2)
		{
			if (_register.IsRegistered(e))
			{
				_events.Add(new Event<T1,T2>(e, param1, param2));
				UpdateSceneObject();
				ProcessEvents();
			} else
				Debug.LogError("Can't send event with ID " + e + " because it's not registerd");
		}

		public void CreateEvent<T1,T2,T3>(int e, T1 param1, T2 param2, T3 param3)
		{
			if (_register.IsRegistered(e))
			{
				_events.Add(new Event<T1,T2,T3>(e, param1, param2, param3));
				UpdateSceneObject();
				ProcessEvents();
			} else
				Debug.LogError("Can't send event with ID " + e + " because it's not registerd");
		}

		public void CreateEvent<T1,T2,T3,T4>(int e, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			if (_register.IsRegistered(e))
			{
				_events.Add(new Event<T1,T2,T3,T4>(e, param1, param2, param3, param4));
				UpdateSceneObject();
				ProcessEvents();
			} else
				Debug.LogError("Can't send event with ID " + e + " because it's not registerd");
		}
		#endregion

		#region Listeners
		public int GetListenersCountForEvent(int e)
		{
			int count = 0;
			foreach (EventListener listener in _listeners)
			{
				if (listener.Event == e)
					count++;
			}
			return count;
		}

		public void CreateListener(int e, System.Action handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void CreateListener<T>(int e, System.Action<T> handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener<T>(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void CreateListener<T1,T2>(int e, System.Action<T1,T2> handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener<T1,T2>(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void CreateListener<T1,T2,T3>(int e, System.Action<T1,T2,T3> handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener<T1,T2,T3>(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void CreateListener<T1,T2,T3,T4>(int e, System.Action<T1,T2,T3,T4> handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener<T1,T2,T3,T4>(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void DestroyListener(int e, System.Action handler)
		{
			DestroyListener(e, handler.Method.Name);
		}

		public void DestroyListener<T1>(int e, System.Action<T1> handler)
		{
			DestroyListener(e, handler.Method.Name);
		}

		public void DestroyListener<T1,T2>(int e, System.Action<T1,T2> handler)
		{
			DestroyListener(e, handler.Method.Name);
		}

		public void DestroyListener<T1,T2,T3>(int e, System.Action<T1,T2,T3> handler)
		{
			DestroyListener(e, handler.Method.Name);
		}

		public void DestroyListener<T1,T2,T3,T4>(int e, System.Action<T1,T2,T3,T4> handler)
		{
			DestroyListener(e, handler.Method.Name);
		}

		public void DestroyListener(int e, string methodName)
		{
			for (int i = _listeners.Count - 1; i >= 0; i--)
			{
				if (_listeners[i].Event == e && _listeners[i].Handler.Equals(methodName))
					_listeners.RemoveAt(i);
			}
			UpdateSceneObject();
		}
		#endregion

		public void ClearEventsAndListeners() 
		{
			_listeners.Clear();
			UpdateSceneObject();
		}

		#region Scene Object
		//private float _lastCheckTime = 0;
		//private GameObject _go;

		private void InitSceneObject()
		{
			/*
			if (_go == null)
			{
				GameObject g = GameObject.Find("ZTKEventManager");
				if (g == null)
				{
					_go = new GameObject("ZTKEventManager");
					_go.AddComponent<EventManagerProcessor>();
				}
				else
					_go = g;
			}*/
		}

		private void UpdateSceneObject()
		{
			/*
			if (Time.realtimeSinceStartup - _lastCheckTime > 5f)
			{
				InitSceneObject();
				_lastCheckTime = Time.realtimeSinceStartup;
			}
			*/
		}
		#endregion
	}

	/*public class EventManagerProcessor : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			EventManager.Update();
		}
	}*/
}
