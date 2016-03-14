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
using Zedarus.ToolKit.Data.Remote;

namespace Zedarus.ToolKit.Data
{
	public class DataManager<GD, PD> where GD : GameData where PD : PlayerData
	{
		#region Data
		private GD _gameData;
		private PD _playerData;
		private RemoteData<GD> _remoteData;
		private string _playerDataFilename;
		#endregion

		private Action _dataLoadedCallback;

		#region Init
		public DataManager() 
		{
			_remoteData = new RemoteData<GD>();
		}
		#endregion

		#region Controls
		public void Load(string playerDataFilename)
		{
			_playerDataFilename = playerDataFilename;
			_gameData = Resources.Load<GD>(GameData.DATABASE_LOCAL_PATH);

			_playerData = PlayerData.Load<PD>(_playerDataFilename);
			//_playerData = typeof(PD).GetMethod("Load",BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { _playerDataFilename }) as PD;
		}

		public void Save()
		{
			_playerData.UpdateVersionAndTimestamp("0.0.0", 20);  // TODO: use actial values here
			PlayerData.Save<PD>(_playerData, _playerDataFilename);
			//typeof(PD).GetMethod("Save", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { _playerData, _playerDataFilename });
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

		public RemoteData<GD> Remote
		{
			get { return _remoteData; }
		}
		#endregion
	}
}
