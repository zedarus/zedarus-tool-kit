using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Adapters;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.API;
using Serialization;
using LitJson;

namespace Zedarus.ToolKit.Data.Player
{
	public class PlayerData
	{
		#region Build Info
		[SerializeThis] private string _gameVersion;
		[SerializeThis] private int _buildNumber = 0;
		[SerializeThis] private DateTime _timestamp;
		#endregion

		#region Parameters
		//private Dictionary<Type, int> _idsTable;
		//[SerializeThis] private string _uuid;
		#endregion

		#region Models
		[SerializeThis] private bool _useDataSync;   // do not sync this
		[SerializeThis] private bool _askedToUseSync;   // do not sync this
		[SerializeThis] private Dictionary<string, PlayerDataModel> _models;
		#endregion
		
		#region Readers
		private static DataReader<PlayerData, SerializedPersistentFileAdapter> _playerDataReader;
		#endregion

		#region Init
		public PlayerData()
		{
			//_uuid = System.Guid.NewGuid().ToString();
			//_idsTable = new Dictionary<Type, int>();
			_models = new Dictionary<string, PlayerDataModel>();
			_useDataSync = false;
			_askedToUseSync = false;
		}
		#endregion

		#region Controls
		public void AddModel<T>() where T : PlayerDataModel
		{
			string t = typeof(T).FullName;

			if (_models.ContainsKey(t))
			{
				//_idsTable.Add(typeof(T), modelID);
				Debug.Log("Model with this id (" + t + ") already added to player data");
			}
			else
			{
				//_idsTable.Add(typeof(T), modelID);
				_models.Add(t, (T)Activator.CreateInstance(typeof(T)));
			}
		}
		#endregion
		
		#region Getters
		public T GetModel<T>() where T : PlayerDataModel
		{	
			string key = typeof(T).FullName;
			if (_models.ContainsKey(key))
				return _models[key] as T;
			else
				return null;
		}
		#endregion

		#region Loading Data
		public static PlayerData Load(string filename)
		{
			PlayerData playerData = Reader.Load(filename);
			if (playerData == null) playerData = new PlayerData();
			return playerData;
		}
		#endregion

		#region Saving Data
		public static void Save(PlayerData data, string filename)
		{
			Reader.Save(data, filename);
		}
		#endregion

		#region Event Listeners
		private void CreateEventListeners() 
		{
			//AudioManager.Instance.soundStateUpdate += OnSoundStateUpdate;
			//AudioManager.Instance.musicStateUpdate += OnMusicStateUpdate;
			
			APIManager.Instance.Sync.SyncFinished += OnSyncFinished;
		}
		
		private void RemoveEventListeners() 
		{
			//AudioManager.Instance.soundStateUpdate -= OnSoundStateUpdate;
			//AudioManager.Instance.musicStateUpdate -= OnMusicStateUpdate;
			
			APIManager.Instance.Sync.SyncFinished -= OnSyncFinished;
		}
		#endregion

		#region Event Handlers
		private void OnSyncFinished()
		{	
			ZedLogger.Log("OnSyncFinished", LoggerContext.iCloud);
			DownloadData();
		}
		#endregion

		#region Syncing
		private void UploadData()
		{
			if (UseDataSync)
				APIManager.Instance.Sync.SetData<PlayerData>(this);
		}
		
		private void DownloadData()
		{
			ZedLogger.Log("DownloadData, use data sync: " + UseDataSync, LoggerContext.iCloud);
			if (UseDataSync)
			{
				MergeData(APIManager.Instance.Sync.GetData<PlayerData>());
			}
			else if (!AskedToUseSync)
			{
				// TODO: use events to display popup
				//PopupManager.Instance.ShowUseICloudDataConfirmPopup(OniCloudConfirmPopupConfirm, OniCloudConfirmPopupCancel);
				MarkAsAskedToUseSync();
			}
		}
		
		private void MergeData(PlayerData dataToMerge)
		{
			ZedLogger.Log("MergeData, dataToMerge: " + dataToMerge, LoggerContext.iCloud);
			if (dataToMerge != null)
			{
				ZedLogger.Log("MergeData, step 2", LoggerContext.iCloud);
				//ZedLogger.Log("Merging data: " + dataToMerge.GameVersion + ", build: " + dataToMerge.BuildNumber + ", " + dataToMerge.GetCurrentLanguage(), LoggerContext.iCloud);
				//MigrateDataBetweenVersions(dataToMerge);
				MergeOnSync(dataToMerge);
				dataToMerge = null;

				// TODO: use ZTK events here
				//if (DataUpdated != null)
				//	DataUpdated();
			}
		}
		
		private void MergeOnSync(PlayerData mergeData)
		{
			Debug.Log("Merge with: " + mergeData);
			// TODO: get list of models in current data and new data
		}

		/*
		private void OniCloudConfirmPopupConfirm()
		{
			SetUseDataSync(true);
			DownloadData();
		}
		
		private void OniCloudConfirmPopupCancel()
		{
			SetUseDataSync(false);
		}
		*/
		#endregion

		#region Migration
		public void MigrateVersion(int buildNumber, string version)
		{
			if (_buildNumber != buildNumber)
			{
				ZedLogger.Log("Migrating from build " + _buildNumber.ToString() + " to " + buildNumber.ToString() + ": no action required");
			}
			else
				ZedLogger.Log("Build number is the same as in the save game file, no action required");
			
			_buildNumber = buildNumber;
			_gameVersion = version;
			Debug.Log(_gameVersion);
		}
		#endregion

		#region Helpers
		private bool UseDataSync
		{
			get { return _useDataSync; }
		}

		public bool AskedToUseSync
		{
			get { return _askedToUseSync; }
		}

		public void SetUseDataSync(bool use)
		{
			APIManager.Instance.Analytics.LogDataSyncStatusChange(use);
			_useDataSync = use;
		}

		public void MarkAsAskedToUseSync()
		{
			_askedToUseSync = true;
		}

		private static DataReader<PlayerData, SerializedPersistentFileAdapter> Reader
		{
			get
			{
				if (_playerDataReader == null) _playerDataReader = new DataReader<PlayerData, SerializedPersistentFileAdapter>();
				return _playerDataReader;
			}
		}
		#endregion
	}
}
