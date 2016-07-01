using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
#if UNITY_IPHONE && API_SYNC_ICLOUD
using Prime31;
#endif

namespace Zedarus.ToolKit.API
{
	public class ICloudWrapperSettings : APIWrapperSettings
	{
		private string _filename = "";

		public ICloudWrapperSettings(object[] settings) : base(settings)
		{
			Assert.IsTrue(settings.Length > 0, "Incorrect number of parameters for ICloud wrapper");
			Assert.IsTrue(settings[0].GetType() == typeof(string), "First parameter must be string");

			_filename = settings[0].ToString();
		}

		public string Filename
		{
			get { return _filename; }
		}
	}

	public class ICloudWrapper : APIWrapper<ICloudWrapper>, ISyncWrapperInterface 
	{
		#region Parameters
		private DateTime _lastSync;
		#endregion
		
		#region Events
		public event Action<byte[]> SyncFinished;
		#endregion
		
		#region Properties
		private string _filename = "sync.dat";
		#endregion
		
		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			ICloudWrapperSettings icloudSettings = settings as ICloudWrapperSettings;
			if (icloudSettings != null)
			{
				_filename = icloudSettings.Filename;
			}

			_lastSync = new DateTime(1986, 1, 1);
			iCloudBinding.getUbiquityIdentityToken();
			#endif
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return new ICloudWrapperSettings(settings);
		}
		#endregion
		
		#region Controls
		public void Sync() 
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			ZedLogger.Log("Sync iCloud", LoggerContext.iCloud);
			iCloudBinding.synchronize();
			#endif
		}
		
		public bool SaveData(byte[] data)
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			if (!iCloudBinding.documentStoreAvailable())
			{
				ZedLogger.Log("Documetns store is not available", LoggerContext.iCloud);
				return false;
			}

//			PlayerData remoteData = GetPlayerData();
//			if (remoteData != null)
//				data.Merge(remoteData);
			
//			byte[] bytes = UnitySerializer.Serialize(data);
			bool result = P31CloudFile.writeAllBytes(Filename, data);
			
			ZedLogger.Log("File written in the cloud: " + result, LoggerContext.iCloud);

			return result;
			#else
			return false;
			#endif
		}
		
		public byte[] GetData()
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			if (P31CloudFile.exists(Filename))
			{
				return P31CloudFile.readAllBytes(Filename);
//				PlayerData data = UnitySerializer.Deserialize<PlayerData>(bytes);
//				return bytes;
			}
			
			ZedLogger.Log("Could not restore player data from cloud", LoggerContext.iCloud);
			return null;
			#else
			return null;
			#endif
		}

		public void DisplayUI() {}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			iCloudManager.keyValueStoreDidChangeEvent += keyValueStoreDidChangeEvent;
			iCloudManager.ubiquityIdentityDidChangeEvent += ubiquityIdentityDidChangeEvent;
			iCloudManager.entitlementsMissingEvent += entitlementsMissingEvent;
			iCloudManager.documentStoreUpdatedEvent += documentStoreUpdatedEvent;
			#endif
		}
		
		protected override void RemoveEventListeners() 
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			iCloudManager.keyValueStoreDidChangeEvent -= keyValueStoreDidChangeEvent;
			iCloudManager.ubiquityIdentityDidChangeEvent -= ubiquityIdentityDidChangeEvent;
			iCloudManager.entitlementsMissingEvent -= entitlementsMissingEvent;
			iCloudManager.documentStoreUpdatedEvent -= documentStoreUpdatedEvent;
			#endif
		}
		#endregion

		#if UNITY_IPHONE && API_SYNC_ICLOUD
		#region Event Handlers
		private void keyValueStoreDidChangeEvent(List<object> keys)
		{
			ZedLogger.Log("keyValueStoreDidChangeEvent. changed keys:", LoggerContext.iCloud);
			foreach(var key in keys)
				ZedLogger.Log(key, LoggerContext.iCloud);
		}
		
		private void ubiquityIdentityDidChangeEvent()
		{
			ZedLogger.Log("ubiquityIdentityDidChangeEvent", LoggerContext.iCloud);
		}
		
		private void entitlementsMissingEvent()
		{
			ZedLogger.Log("entitlementsMissingEvent", LoggerContext.iCloud);
		}
		
		private void documentStoreUpdatedEvent(List<iCloudManager.iCloudDocument> files)
		{
			ZedLogger.Log("documentStoreUpdatedEvent. changed files: ", LoggerContext.iCloud);
			
			bool neededFileDownloaded = false;
			
			foreach(var doc in files)
			{
				ZedLogger.Log(doc, LoggerContext.iCloud);
				
				if (doc.filename.Equals(Filename))
				{
					if (doc.isDownloaded)
					{
						int result = DateTime.Compare(doc.contentChangedDate, _lastSync);
						ZedLogger.Log("Comparing " + doc.contentChangedDate + " to " + _lastSync + ", result: " + result, LoggerContext.iCloud);
						// Only sync if file change date is different from the last one:
						if (result > 0)
						{
							neededFileDownloaded = true;
							_lastSync = doc.contentChangedDate;
						}
					}
					else
						iCloudBinding.isFileDownloaded(doc.filename);
				}
			}
			
			if (neededFileDownloaded)
				SendSyncFinishedEvent();
		}
		#endregion
		#endif
		
		#region Event Senders
		private void SendSyncFinishedEvent()
		{
			ZedLogger.Log("Sending sync finished event", LoggerContext.iCloud);
			if (SyncFinished != null)
				SyncFinished(GetData());
		}
		#endregion

		#region Getters
		private string Filename
		{
			get { return _filename; }
		}
		#endregion
	}
}
