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
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Settings;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class PlayerData
	{
		#region Build Info
		[SerializeField] private int _buildNumber = 0;
		[SerializeField] private DateTime _timestamp;
		#endregion

		#region Models
		[SerializeField] private Dictionary<string, IPlayerDataModel> _models;
		#endregion

		#region Init
		public PlayerData()
		{
			_models = new Dictionary<string, IPlayerDataModel>();
		}

		protected virtual void SetupModelsList()
		{
		}
		#endregion

		#region Merging
		private void MergeData(PlayerData dataToMerge)
		{
			if (dataToMerge != null)
			{
				if (DateTime.Compare(dataToMerge.Timestamp, Timestamp) > 0)
				{
					foreach (KeyValuePair<string, IPlayerDataModel> model in dataToMerge._models)
					{
						if (model.Value != null && _models.ContainsKey(model.Key))
						{
							_models[model.Key].Merge(model.Value);
						}
					}
				}
			}
		}
		#endregion

		#region Controls
		public void AddModel<T>() where T : IPlayerDataModel
		{
			string t = typeof(T).FullName;

			if (!_models.ContainsKey(t))
			{
				_models.Add(t, (T)Activator.CreateInstance(typeof(T)));
			}
		}

		public void UpdateVersionAndTimestamp(string version, int build)
		{
			_buildNumber = build;
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
				data.SetupModelsList();

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
	}
}
