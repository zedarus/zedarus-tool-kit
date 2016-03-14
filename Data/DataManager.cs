using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Zedarus.ToolKit;
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

		private Action _dataLoadedCallback;

		#region Init
		public DataManager() { }
		#endregion

		#region Controls
		public void LoadGameData()
		{
			// We need to use Object.Instantiate here because GameData is going
			// to change at runtime by rematoe overrides and we don't want those
			// changes to actually save in .asset file when testing in editor
			_gameData = UnityEngine.Object.Instantiate(Resources.Load<GD>(GameData.DATABASE_LOCAL_PATH));
		}

		public void LoadPlayerData(string dataFilename)
		{
			_playerDataFilename = dataFilename;
			_playerData = PlayerData.Load<PD>(_playerDataFilename);
			//_playerData = typeof(PD).GetMethod("Load",BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { _playerDataFilename }) as PD
		}

		public void Save()
		{
			_playerData.UpdateVersionAndTimestamp("0.0.0", 20);  // TODO: use actial values here
			PlayerData.Save<PD>(_playerData, _playerDataFilename);
			//typeof(PD).GetMethod("Save", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { _playerData, _playerDataFilename });
		}

		public void ApplyRemoteData(string json)
		{
			if (_gameData != null)
			{
				_gameData.ApplyRemoteData(json);
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
	}
}
