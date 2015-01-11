using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Helpers;

namespace Zedarus.ToolKit.Events
{
	public class EventManager : SimplePrivateSingleton<EventManager>
	{
		private List<Event> _events;
		private List<EventListener> _listeners;

		public EventManager()
		{
			_events = new List<Event>();
			_listeners = new List<EventListener>();
			InitSceneObject();
		}

		#region Controls
		static public void SendEvent(int e, params object[] args)
		{
			Instance.RegisterEvent(e, args);
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

		/// <summary>
		/// Adds the listener with parameters.
		/// </summary>
		/// <param name="e">Event id.</param>
		/// <param name="handler">Handler method.</param>
		/// <param name="consume">If set to <c>true</c>, then event will be consumed and will not trigger other listeners.</param>
		/// <param name="oneTime">If set to <c>true</c>, handler will be only triggered once.</param>
		static public void AddListener(int e, System.Action<object[]> handler, bool consume = false, bool oneTime = false)
		{
			Instance.CreateListener(e, handler, consume, oneTime);
		}

		static public void RemoveListener(int e, System.Action handler)
		{
			Instance.DestroyListener(e, handler);
		}

		static public void RemoveListener(int e, System.Action<object[]> handler)
		{
			Instance.DestroyListener(e, handler);
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
		#endregion

		public void ProcessEvents()
		{
			Event e;
			EventListener listener;
			for (int i = _events.Count - 1; i >= 0; i--)
			{
				e = _events[i];
				for (int l = _listeners.Count - 1; l >= 0; l--)
				{
					listener = _listeners[i];
					if (listener.Event == e.ID)
					{
						listener.Call(e.Parameters);

						if (listener.Expired)
							_listeners.RemoveAt(i);

						if (listener.Consume)
						{
							_events.RemoveAt(i);
							break;
						}
					}
				}
			}

			// TODO: add lifelength for unprocessed events so they are not removed immidately if not processed
			_events.Clear();

			//Debug.Log("Update");
		}

		public void RegisterEvent(int e, object[] args)
		{
			_events.Add(new Event(e, args));
			UpdateSceneObject();
			ProcessEvents();
		}

		public void CreateListener(int e, System.Action handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void CreateListener(int e, System.Action<object[]> handler, bool consume, bool oneTime)
		{
			_listeners.Add(new EventListener(e, handler, consume, oneTime));
			UpdateSceneObject();
		}

		public void DestroyListener(int e, System.Action handler)
		{
			for (int i = _listeners.Count - 1; i >= 0; i--)
			{
				if (_listeners[i].Event == e && _listeners[i].Handler == handler)
					_listeners.RemoveAt(i);
			}
			UpdateSceneObject();
		}

		public void DestroyListener(int e, System.Action<object[]> handler)
		{
			for (int i = _listeners.Count - 1; i >= 0; i--)
			{
				if (_listeners[i].Event == e && _listeners[i].HandlerAlt == handler)
					_listeners.RemoveAt(i);
			}
			UpdateSceneObject();
		}

		public void ClearEventsAndListeners() 
		{
			_listeners.Clear();
			UpdateSceneObject();
			Debug.Log("Clear");
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

	public class EventManagerProcessor : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			EventManager.Update();
		}
	}
}
