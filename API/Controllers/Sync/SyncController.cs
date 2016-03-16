using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public class SyncController : APIController 
	{
		#region Events
		public event Action SyncFinished;
		#endregion
		
		#region Initialization
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Sync.iCloud:
					return ICloudWrapper.Instance;
				case APIs.Sync.GooglePlayGameServices:
					return GooglePlayGameSavesWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void Sync() 
		{
			if (Wrapper != null)
				Wrapper.Sync();
		}
		
		public bool SaveData(byte[] data) 
		{
			if (Wrapper != null)
				return Wrapper.SaveData(data);
			else
				return false;
		}
		
		public byte[] GetPlayerData() 
		{
			if (Wrapper != null)
				return Wrapper.GetData();
			else
				return null;
		}

		public void DisplayUI()
		{
			if (Wrapper != null)
				Wrapper.DisplayUI();
		}
		#endregion
		
		#region Getters
		protected ISyncWrapperInterface Wrapper
		{
			get { return (ISyncWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();
			
			foreach (ISyncWrapperInterface wrapper in Wrappers)
			{
				wrapper.SyncFinished += OnSyncFinished;
			}
		}
		
		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();
			
			foreach (ISyncWrapperInterface wrapper in Wrappers)
			{
				wrapper.SyncFinished -= OnSyncFinished;
			}
		}
		#endregion
		
		#region Event Handlers
		private void OnSyncFinished()
		{
			if (SyncFinished != null)
				SyncFinished();
		}
		#endregion
	}
}
