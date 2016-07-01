using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Data.Game;

namespace Zedarus.ToolKit.Data
{
	public class DataManager<GD, PD> where GD : GameData where PD : PlayerData
	{
		#region Data
		private GD _gameData;
		private PD _playerData;
		private string _playerDataFilename;
		#endregion

		#region Properties
		private string _queuedRemoteData = null;
		#endregion

		#region Events
		public event Action<bool> PlayerDataSaved;
		#endregion

		#region Init
		public DataManager() 
		{
			EventManager.AddListener<string>(IDs.Events.RemoteDataReceived, OnRemoteDataReceived, true);
		}
		#endregion

		#region Controls
		public void LoadGameData()
		{
			// We need to use Object.Instantiate here because GameData is going
			// to change at runtime by rematoe overrides and we don't want those
			// changes to actually save in .asset file when testing in editor
			_gameData = UnityEngine.Object.Instantiate(Resources.Load<GD>(GameData.DATABASE_LOCAL_PATH));

			if (Game != null && _queuedRemoteData != null)
			{
				Game.ApplyRemoteData(_queuedRemoteData);
				if (Player != null)
					Player.OnGameDataChange();
				_queuedRemoteData = null;
			}
		}

		public void LoadPlayerData(string dataFilename)
		{
			_playerDataFilename = dataFilename;
			_playerData = PlayerData.Load<PD>(_playerDataFilename);
		}

		public void Save(bool sync)
		{
			Debug.Log("Save data");
			Player.UpdateVersionAndTimestamp(Game.Settings.Version, Game.Settings.Build);
			PlayerData.Save<PD>(Player, _playerDataFilename);

			if (PlayerDataSaved != null)
			{
				PlayerDataSaved(sync);
			}
		}
		#endregion

		#region Queries
		public GD Game
		{
			get { return _gameData; }
		}

		public PD Player
		{
			get { return _playerData; }
		}
		#endregion

		#region Event Handlers
		private void OnRemoteDataReceived(string json)
		{
			if (Game != null)
			{
				Game.ApplyRemoteData(json);
				if (Player != null)
					Player.OnGameDataChange();
			}
			else
			{
				_queuedRemoteData = json;
			}
		}
		#endregion
	}
}
