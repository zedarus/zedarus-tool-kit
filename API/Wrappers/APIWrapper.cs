using UnityEngine;
using System;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public abstract class APIWrapper<T> : SimpleSingleton<T>, IAPIWrapperInterface where T : class
	{
		#region Parameters
		protected bool _initialized = false;
		private int _api;
		#endregion
		
		#region Events
		public event Action Initialized;
		#endregion
		
		#region Initialization
		public void Init(object[] parameters)
		{
			if (!_initialized)
			{
				CreateEventListeners();
				Setup(parameters);
				_initialized = true;
			}
		}
		
		public void Destroy()
		{
			RemoveEventListeners();
			_initialized = false;
		}
		
		public void SetAPI(int api)
		{
			_api = api;
		}
		#endregion
		
		#region Setup
		protected abstract void Setup(object[] parameters);
		#endregion
		
		#region Event Senders
		protected void SendInitializedEvent()
		{
			if (Initialized != null)
				Initialized();
		}
		#endregion
		
		#region Event Listeners
		protected abstract void CreateEventListeners();
		protected abstract void RemoveEventListeners();
		#endregion
		
		#region Getters/Setters
		public int API
		{
			get { return _api; }
		}
		#endregion
	}
}

