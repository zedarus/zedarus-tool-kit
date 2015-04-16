using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public enum MultipleAPIUseMode
	{
		OnlyFirst,
		Cycle,
		All,
		Select,
		None
	}
	
	public abstract class APIController
	{
		#region Parameters
		private bool _initialized = false;
		private bool _initializationStarted = false;
		private List<APIs> _apis = new List<APIs>();
		private MultipleAPIUseMode _apiUseMode;
		protected List<IAPIWrapperInterface> _wrappers;
		private int _currentWrapperIndex = 0;
		private int _numberOfWrappersInitialized = 0;
		#endregion
		
		#region Events
		public event Action Initialized;
		#endregion
		
		#region Initialization
		public void UseAPI(MultipleAPIUseMode useMode, params APIs[] values)
		{
			_apiUseMode = useMode;
			foreach (APIs api in values)
			{
				_apis.Add(api);
			}
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
			foreach (APIs api in APIList)
				AddWrapperForAPI(api);
		}
		
		protected virtual void InitWrappers()
		{
			foreach (IAPIWrapperInterface wrapper in _wrappers)
				wrapper.Init();
		}
		
		private void AddWrapperForAPI(APIs wrapperAPI)
		{
			IAPIWrapperInterface wrapper = GetWrapperForAPI(wrapperAPI);
			if (wrapper != null)
			{
				wrapper.SetAPI(wrapperAPI);
				_wrappers.Add(wrapper);
			}
		}
		
		protected abstract IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI);
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
			if (_numberOfWrappersInitialized == _wrappers.Count)
				CompleteInitialization();
		}
		#endregion
		
		#region Getters
		protected bool IsInitialized
		{
			get { return _initialized; }
		}
		
		protected List<APIs> APIList
		{
			get { return _apis; }
		}
		
		private MultipleAPIUseMode APIUseMode
		{
			get { return _apiUseMode; }
		}
		
		protected List<IAPIWrapperInterface> Wrappers
		{
			get { return _wrappers; }
		}
		
		protected IAPIWrapperInterface CurrentWrapperBase
		{
			get
			{
				switch (APIUseMode)
				{
					case MultipleAPIUseMode.OnlyFirst:
						if (_wrappers.Count > 0)
							return _wrappers[0];
						else
							return null;
					
					case MultipleAPIUseMode.Cycle:
						if (_currentWrapperIndex < _wrappers.Count)
						{
							IAPIWrapperInterface wrapper = _wrappers[_currentWrapperIndex];
							_currentWrapperIndex++;
							if (_currentWrapperIndex >= _wrappers.Count)
								_currentWrapperIndex = 0;
							return wrapper;
						}
						else
							return null;
					
					default:
						return null;
				}
			}
		}
		
		protected IAPIWrapperInterface WrapperWithAPI(APIs api)
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
