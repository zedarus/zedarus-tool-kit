using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Events
{
	public class RequestManager
	{
		#region Controls
		static public bool RegisterRequest(int r)
		{
			return EventManager.RegisterEvent(ConvertRequestID(r));
		}
		#endregion

		#region Send Requests
		public static void SendRequest(int r, Action<bool> callback)
		{
			EventManager.SendEvent<Action<bool>>(ConvertRequestID(r), callback);
		}

		public static void SendRequest<T1>(int r, Action<bool> callback, T1 param1)
		{
			EventManager.SendEvent<Action<bool>, T1>(ConvertRequestID(r), callback, param1);
		}

		public static void SendRequest<T1, T2>(int r, Action<bool> callback, T1 param1, T2 param2)
		{
			EventManager.SendEvent<Action<bool>, T1, T2>(ConvertRequestID(r), callback, param1, param2);
		}

		public static void SendRequest<T1, T2, T3>(int r, Action<bool> callback, T1 param1, T2 param2, T3 param3)
		{
			EventManager.SendEvent<Action<bool>, T1, T2, T3>(ConvertRequestID(r), callback, param1, param2, param3);
		}
		#endregion

		#region Add Request Processors
		public static void AddRequestProcessor(int r, Action<Action<bool>> processor)
		{
			r = ConvertRequestID(r);
			if (!IsRequestProcessExists(r))
				EventManager.AddListener<Action<bool>>(r, processor, true);
			else
				Debug.LogWarning("Can't have more than one request processor for one request ID");
		}

		public static void AddRequestProcessor<T1>(int r, Action<Action<bool>, T1> processor)
		{
			r = ConvertRequestID(r);
			if (!IsRequestProcessExists(r))
				EventManager.AddListener<Action<bool>, T1>(r, processor, true);
			else
				Debug.LogWarning("Can't have more than one request processor for one request ID");
		}

		public static void AddRequestProcessor<T1, T2>(int r, Action<Action<bool>, T1, T2> processor)
		{
			r = ConvertRequestID(r);
			if (!IsRequestProcessExists(r))
				EventManager.AddListener<Action<bool>, T1, T2>(r, processor, true);
			else
				Debug.LogWarning("Can't have more than one request processor for one request ID");
		}

		public static void AddRequestProcessor<T1, T2, T3>(int r, Action<Action<bool>, T1, T2, T3> processor)
		{
			r = ConvertRequestID(r);
			if (!IsRequestProcessExists(r))
				EventManager.AddListener<Action<bool>, T1, T2, T3>(r, processor, true);
			else
				Debug.LogWarning("Can't have more than one request processor for one request ID");
		}
		#endregion
			
		#region Remove Request Processors
		static public void RemoveRequestProcessor(int r, System.Action<Action<bool>> processor)
		{
			EventManager.RemoveListener<Action<bool>>(ConvertRequestID(r), processor);
		}

		static public void RemoveRequestProcessor<T1>(int r, System.Action<Action<bool>, T1> processor)
		{
			EventManager.RemoveListener<Action<bool>, T1>(ConvertRequestID(r), processor);
		}

		static public void RemoveRequestProcessor<T1, T2>(int r, System.Action<Action<bool>, T1, T2> processor)
		{
			EventManager.RemoveListener<Action<bool>, T1, T2>(ConvertRequestID(r), processor);
		}

		static public void RemoveRequestProcessor<T1, T2, T3>(int r, System.Action<Action<bool>, T1, T2, T3> processor)
		{
			EventManager.RemoveListener<Action<bool>, T1, T2, T3>(ConvertRequestID(r), processor);
		}
		#endregion

		#region Helpers
		private static int ConvertRequestID(int id)
		{
			if (id > 0)
				return -id;
			else
				return id;
		}

		private static bool IsRequestProcessExists(int r)
		{
			return EventManager.IsListenerExists(r);
		}
		#endregion
	}
}
