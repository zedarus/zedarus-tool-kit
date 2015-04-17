using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Adapters;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.Data.Adapters;

namespace Zedarus.ToolKit.Data
{
	public class DataManager : SimpleSingleton<DataManager>
	{
		#region Data
		private GameData _gameData;
		private PlayerData _playerData;
		private string _playerDataFilename;
		#endregion

		private Action _dataLoadedCallback;

		#region Init
		public DataManager()
		{
			_gameData = new GameData();
		}
		#endregion

		#region Controls
		public void Load(string playerDataFilename)
		{
			_playerDataFilename = playerDataFilename;
			_gameData.Load();
			_playerData = PlayerData.Load(_playerDataFilename);
		}

		public void Save()
		{
			PlayerData.Save(_playerData, _playerDataFilename);	
		}
		#endregion

		#region Queries
		public GameData Game 
		{
			get { return _gameData; }
		}

		public PlayerData Player
		{
			get { return _playerData; }
		}
		#endregion
	}
}
