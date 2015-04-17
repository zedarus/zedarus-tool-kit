using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Adapters;
using Zedarus.ToolKit.Data.Player.Models;
using Serialization;
using LitJson;

namespace Zedarus.ToolKit.Data.Player
{
	public class PlayerData
	{
		#region Constants
		private const string datafilename = "qpdata002.sav";
		#endregion

		#region Parameters
		[SerializeThis] private string _uuid;
		[SerializeThis] private string _name;
		[SerializeThis] private string _country;
		#endregion

		#region Models
		#endregion
		
		#region Readers
		private static DataReader<PlayerData, SerializedPersistentFileAdapter> _playerDataReader;
		#endregion

		#region Init
		public PlayerData()
		{
			_uuid = System.Guid.NewGuid().ToString();
			_name = "Player Name";
			_country = DetectCountry();
		}
		#endregion

		#region Loading Data
		public static PlayerData Load()
		{
			PlayerData playerData = Reader.Load(datafilename);
			if (playerData == null) playerData = new PlayerData();
			return playerData;
		}
		#endregion

		#region Saving Data
		public static void Save(PlayerData data)
		{
			Reader.Save(data, datafilename);
		}
		#endregion

		#region Controls

		#endregion

		#region Getters
		public string Country
		{
			get { return _country; }
		}
		#endregion

		#region Helpers
		private static DataReader<PlayerData, SerializedPersistentFileAdapter> Reader
		{
			get
			{
				if (_playerDataReader == null) _playerDataReader = new DataReader<PlayerData, SerializedPersistentFileAdapter>();
				return _playerDataReader;
			}
		}

		private string DetectCountry()
		{
			// TODO: implement correct country code detection here
			return "us";
		}
		#endregion
	}
}
