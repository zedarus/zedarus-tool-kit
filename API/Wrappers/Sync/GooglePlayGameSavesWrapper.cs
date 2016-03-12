using System;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class GooglePlayGameSavesWrapper : APIWrapper<GooglePlayGameSavesWrapper>, ISyncWrapperInterface 
	{
		#region Events
		public event Action SyncFinished;
		#endregion

		#region Properties
		#endregion

		#region Setup
		protected override void Setup(object[] parameters) {}
		#endregion

		#region Controls
		public void Sync() {}

		public bool SaveData(byte[] data)
		{
			return true;
		}

		public byte[] GetData()
		{
			return null;
		}

		public void DisplayUI()
		{
			#if UNITY_ANDROID && API_SYNC_GPGS
			UnityEngine.Debug.Log("Display UI");
			PlayGameServices.showSnapshotList(3, "Kubiko", true, false);
			#endif
		}

		private void Save()
		{
			#if UNITY_ANDROID && API_SYNC_GPGS
			PlayGameServices.saveSnapshot("Kubiko", true, PlayerDataManager.Instance.DataBytes, "Completed levels: " + PlayerDataManager.Instance.GetNumberOfLevelsCompletedForCurrentGameMode(), GPGSnapshotConflictPolicy.MostRecentlyModified);
			#endif
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			#if UNITY_ANDROID && API_SYNC_GPGS
			GPGManager.snapshotListUserSelectedSnapshotEvent += snapshotListUserSelectedSnapshotEvent;
			GPGManager.snapshotListUserRequestedNewSnapshotEvent += snapshotListUserRequestedNewSnapshotEvent;
			GPGManager.snapshotListCanceledEvent += snapshotListCanceledEvent;
			GPGManager.saveSnapshotSucceededEvent += saveSnapshotSucceededEvent;
			GPGManager.saveSnapshotFailedEvent += saveSnapshotFailedEvent;
			GPGManager.loadSnapshotSucceededEvent += loadSnapshotSucceededEvent;
			GPGManager.loadSnapshotFailedEvent += loadSnapshotFailedEvent;
			#endif
		}

		protected override void RemoveEventListeners() 
		{
			#if UNITY_ANDROID && API_SYNC_GPGS
			GPGManager.snapshotListUserSelectedSnapshotEvent -= snapshotListUserSelectedSnapshotEvent;
			GPGManager.snapshotListUserRequestedNewSnapshotEvent -= snapshotListUserRequestedNewSnapshotEvent;
			GPGManager.snapshotListCanceledEvent -= snapshotListCanceledEvent;
			GPGManager.saveSnapshotSucceededEvent -= saveSnapshotSucceededEvent;
			GPGManager.saveSnapshotFailedEvent -= saveSnapshotFailedEvent;
			GPGManager.loadSnapshotSucceededEvent -= loadSnapshotSucceededEvent;
			GPGManager.loadSnapshotFailedEvent -= loadSnapshotFailedEvent;
			#endif
		}
		#endregion

		#if UNITY_ANDROID && API_SYNC_GPGS
		#region Event Handlers
		private void snapshotListUserSelectedSnapshotEvent(GPGSnapshotMetadata metadata) 
		{
			UnityEngine.Debug.Log("snapshotListUserSelectedSnapshotEvent: " + metadata);
			PlayGameServices.loadSnapshot(metadata.name);
		}

		private void snapshotListUserRequestedNewSnapshotEvent() 
		{
			UnityEngine.Debug.Log("snapshotListUserRequestedNewSnapshotEvent");
			Save();
		}

		private void snapshotListCanceledEvent() 
		{
			UnityEngine.Debug.Log("snapshotListCanceledEvent");
		}

		private void saveSnapshotSucceededEvent() 
		{
			UnityEngine.Debug.Log("snapshotListCanceledEvent");
		}

		private void saveSnapshotFailedEvent(string reason) 
		{
			UnityEngine.Debug.Log("saveSnapshotFailedEvent: " + reason);
		}

		private void loadSnapshotSucceededEvent(GPGSnapshot snapshot) 
		{
			UnityEngine.Debug.Log("loadSnapshotSucceededEvent " + snapshot);

			if (snapshot.hasDataAvailable)
			{
				_data = UnitySerializer.Deserialize<PlayerData>(snapshot.snapshotData);
				if (_data != null && SyncFinished != null)
					SyncFinished();
			}
		}

		private void loadSnapshotFailedEvent(string reason) 
		{
			UnityEngine.Debug.Log("snapshotListCanceledEvent: " + reason);
		}
		#endregion
		#endif
	}
}

