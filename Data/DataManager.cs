using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Player;
using Zedarus.Toolkit.Data.Game;

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
		public void Load(string playerDataFilename)
		{
			_playerDataFilename = playerDataFilename;
			_gameData = Resources.Load<GD>(GameData.DATABASE_LOCAL_PATH);
			//_playerData = PD.Load(_playerDataFilename);
		}

		public void Save()
		{
			//PD.Save(_playerData, _playerDataFilename);	
		}
		#endregion

		#region Queries
		public GD Game
		{
			get
			{
				if (_gameData == null)
				{
					_gameData = Resources.Load<GD>(GameData.DATABASE_LOCAL_PATH);
				}

				return _gameData;
			}
		}

		public PD Player
		{
			get { return _playerData; }
		}
		#endregion
	}
}
