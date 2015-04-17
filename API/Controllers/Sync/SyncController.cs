using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Data.GameData;
using Zedarus.Traffico.Data.GameData.Models;
using Zedarus.Traffico.Data.PlayerData;

namespace Zedarus.ToolKit.API
{
	public class SyncController : APIController 
	{
		#region Events
		public event Action SyncFinished;
		#endregion
		
		#region Initialization
		public SyncController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				#if API_ICLOUD_P31
				case APIs.AppleICloud:
					return ICloudWrapper.Instance;
				#endif
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
		
		public bool SavePlayerData(PlayerData data) 
		{
			if (Wrapper != null)
				return Wrapper.SavePlayerData(data);
			else
				return false;
		}
		
		public PlayerData GetPlayerData() 
		{
			if (Wrapper != null)
				return Wrapper.GetPlayerData();
			else
				return null;
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
			APIManager.Instance.Analytics.LogDataSyncEvent();
		}
		#endregion
	}
}
