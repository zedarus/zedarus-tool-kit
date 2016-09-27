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
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Extentions.OneTapGames.RateMePopup;

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
			AddModel<APIState>();
			AddModel<AchievementsTracker>();
			AddModel<RateMePopupModel>();
			AddModel<AudioState>();
		}

		internal void SetGameDataReference(GameData gameData)
		{
			if (AchievementsTracker != null)
			{
				AchievementsTracker.SetGameDataReference(gameData);
			}
		}
		#endregion

		#region Merging
		public bool ShouldMergeData(PlayerData dataToMerge)
		{
			if (dataToMerge != null)
			{
				if (DateTime.Compare(dataToMerge.Timestamp, Timestamp) > 0)
				{
					return true;
				}
			}

			return false;
		}

		public bool MergeData(PlayerData dataToMerge)
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

					return true;
				}
			}

			return false;
		}
		#endregion

		#region Controls
		public virtual void PostInit()
		{
			
		}

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

		public virtual void OnGameDataChange()
		{
			PostInit();
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

		public AchievementsTracker AchievementsTracker
		{
			get { return GetModel<AchievementsTracker>(); }
		}

		public AudioState AudioState
		{
			get { return GetModel<AudioState>(); }
		}

		public APIState APIState
		{
			get { return GetModel<APIState>(); }
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
		public static byte[] Serialize<PlayerDataClass>(PlayerDataClass data) where PlayerDataClass : PlayerData
		{
			try
			{
				using (MemoryStream stream = new MemoryStream())
				{
					new BinaryFormatter().Serialize(stream, data);
					return stream.ToArray();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static PlayerDataClass Deserialize<PlayerDataClass>(byte[] data) where PlayerDataClass : PlayerData
		{
			try
			{
				using (MemoryStream stream = new MemoryStream(data))
				{
					return new BinaryFormatter().Deserialize(stream) as PlayerDataClass;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

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
