using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public class RemoteDataController : APIController 
	{
		#region Events
		public event Action<string> DataReceived;
		#endregion

		#region Initialization
		protected override void Setup() {}	
		#endregion

		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.RemoteData.HeyZap:
					return HeyZapDataWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls
		public void RequestData()
		{
			IRemoteDataWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
			{
				wrapper.RequestData();		
			}
		}
		#endregion

		#region Getters
		protected IRemoteDataWrapperInterface Wrapper
		{
			get { return (IRemoteDataWrapperInterface)CurrentWrapperBase; }
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

			foreach (IRemoteDataWrapperInterface wrapper in Wrappers)
			{
				wrapper.DataReceived += OnDataReceived;
			}
		}

		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();

			foreach (IRemoteDataWrapperInterface wrapper in Wrappers)
			{
				wrapper.DataReceived -= OnDataReceived;
			}
		}
		#endregion

		#region Event Handlers
		private void OnDataReceived(string data)
		{
			if (DataReceived != null)
			{
				DataReceived(data);
			}
		}
		#endregion
	}
}
