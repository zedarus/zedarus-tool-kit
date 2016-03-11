using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.API;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class PlayerData
	{
		#region Build Info
		[SerializeField] private string _gameVersion;
		[SerializeField] private int _buildNumber = 0;
		[SerializeField] private DateTime _timestamp;
		#endregion

		#region Parameters
		//[SerializeThis] private string _uuid;
		#endregion

		#region Models
		[SerializeField] private bool _useDataSync;   // do not sync this
		[SerializeField] private bool _askedToUseSync;   // do not sync this
		[SerializeField] private Dictionary<string, IPlayerDataModel> _models;
		#endregion

		#region Init
		public PlayerData()
		{
			//_uuid = System.Guid.NewGuid().ToString();
			//_idsTable = new Dictionary<Type, int>();
			_models = new Dictionary<string, IPlayerDataModel>();
			_useDataSync = false;
			_askedToUseSync = false;
		}

		protected virtual void CustomizeModels()
		{
			
		}
		#endregion

		#region Controls
		public void AddModel<T>() where T : IPlayerDataModel
		{
			string t = typeof(T).FullName;

			if (_models.ContainsKey(t))
			{
				Debug.Log("Model with this id (" + t + ") already added to player data");
			}
			else
			{
				_models.Add(t, (T)Activator.CreateInstance(typeof(T)));
			}
		}

		public void UpdateVersionAndTimestamp(string version, int build)
		{
			_buildNumber = build;
			_gameVersion = version;
			_timestamp = DateTime.UtcNow;
		}
		#endregion
		
		#region Getters
		public T GetModel<T>() where T : IPlayerDataModel
		{	
			string key = typeof(T).FullName;
			if (_models.ContainsKey(key))
				return (T) _models[key];
			else
				return default(T);
		}

		public int Build
		{
			get { return _buildNumber; }
		}

		public DateTime Timestamp
		{
			get { return _timestamp; }
		}
		#endregion

		#region Loading & Saving Data
		public static PlayerDataClass Load<PlayerDataClass>(string filename) where PlayerDataClass : PlayerData
		{
			PlayerDataClass data = null;

			if (!DoesSaveGameExist(filename))
			{
				data = (PlayerDataClass)Activator.CreateInstance(typeof(PlayerDataClass));
			}
			else
			{
				BinaryFormatter formatter = new BinaryFormatter();

				using (FileStream stream = new FileStream(GetSavePath(filename), FileMode.Open))
				{
					try
					{
						data = formatter.Deserialize(stream) as PlayerDataClass;
					}
					catch (Exception)
					{
						data = null;
					}
				}
			}

			if (data != null)
				data.CustomizeModels();

			return data;
		}

		public static bool Save<PlayerDataClass>(PlayerDataClass data, string filename) where PlayerDataClass : PlayerData
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (FileStream stream = new FileStream(GetSavePath(filename), FileMode.Create))
			{
				try
				{
					formatter.Serialize(stream, data);
				}
				catch (Exception)
				{
					return false;
				}
			}

			return true;
		}

		private static bool DoesSaveGameExist(string name)
		{
			return File.Exists(GetSavePath(name));
		}

		private static string GetSavePath(string name)
		{
			return Path.Combine(Application.persistentDataPath, name);
		}
		#endregion

		#region Event Listeners
		private void CreateEventListeners() 
		{
			APIManager.Instance.Sync.SyncFinished += OnSyncFinished;
		}
		
		private void RemoveEventListeners() 
		{
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
		#endregion
	}
}
