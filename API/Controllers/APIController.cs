using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{	
	public abstract class APIController
	{
		#region Parameters
		private bool _initialized = false;
		private bool _initializationStarted = false;
		private List<int> _apis = new List<int>();
		protected List<IAPIWrapperInterface> _wrappers;
		private object[] _wrapperParameters = null;
		private int _numberOfWrappersInitialized = 0;
		#endregion
		
		#region Events
		public event Action Initialized;
		#endregion
		
		#region Initialization
		public void Use(int api, float initializationDelay, params object[] parameters)
		{
			// TODO: implement initalization delay
			_apis.Add(api);
			_wrapperParameters = parameters;
		}
		
		public void Init()
		{
			if (!_initialized && !_initializationStarted)
			{
				_wrappers = new List<IAPIWrapperInterface>();
				
				Setup();
				CreateWrappers();
				CreateEventListeners();
				InitWrappers();
				
				_initializationStarted = true;
			}
		}
		
		public void Destroy()
		{
			RemoveEventListeners();
			_initialized = false;
			_initializationStarted = false;
		}
		
		protected virtual void CompleteInitialization()
		{
			_initialized = true;
			SendInitializedEvent();
		}
		
		protected abstract void Setup();
		#endregion
		
		#region Wrappers Initialization
		private void CreateWrappers()
		{
			foreach (int api in APIList)
			{
				AddWrapperForAPI(api);
			}
		}
		
		protected virtual void InitWrappers()
		{
			foreach (IAPIWrapperInterface wrapper in _wrappers)
				wrapper.Init(_wrapperParameters);
		}
		
		private void AddWrapperForAPI(int wrapperAPI)
		{
			IAPIWrapperInterface wrapper = GetWrapperForAPI(wrapperAPI);
			if (wrapper != null)
			{
				wrapper.SetAPI(wrapperAPI);
				_wrappers.Add(wrapper);
			}
		}
		
		protected abstract IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI);
		#endregion
		
		#region Event Senders
		protected void SendInitializedEvent()
		{
			if (Initialized != null)
				Initialized();
		}
		#endregion
		
		#region Event Listeners
		protected virtual void CreateEventListeners()
		{
			foreach (IAPIWrapperInterface wrapper in _wrappers)
				wrapper.Initialized += OnWrapperInitialized;
		}
		
		protected virtual void RemoveEventListeners()
		{
			foreach (IAPIWrapperInterface wrapper in _wrappers)
				wrapper.Initialized -= OnWrapperInitialized;
		}
		#endregion
		
		#region Event Handlers
		private void OnWrapperInitialized()
		{
			_numberOfWrappersInitialized++;
			if (_numberOfWrappersInitialized >= _wrappers.Count)
				CompleteInitialization();
		}
		#endregion
		
		#region Getters
		public bool IsInitialized
		{
			get { return _initialized; }
		}
		
		protected List<int> APIList
		{
			get { return _apis; }
		}
		
		protected List<IAPIWrapperInterface> Wrappers
		{
			get { return _wrappers; }
		}
		
		protected IAPIWrapperInterface CurrentWrapperBase
		{
			get
			{
				if (_wrappers != null && _wrappers.Count > 0)
					return _wrappers[0];
				else
					return null;
			}
		}
		
		protected IAPIWrapperInterface WrapperWithAPI(int api)
		{
			for (int i = 0; i < _wrappers.Count; i++)
			{
				if (_wrappers[i].API == api)
					return _wrappers[i];
			}
			
			return null;
		}
		#endregion
	}
}
