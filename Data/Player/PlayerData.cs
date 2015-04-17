using UnityEngine;
using System;
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
		#region Parameters
		private Dictionary<Type, int> _idsTable;
		//[SerializeThis] private string _uuid;
		#endregion

		#region Models
		[SerializeThis] private Dictionary<int, PlayerDataModel> _models;
		#endregion
		
		#region Readers
		private static DataReader<PlayerData, SerializedPersistentFileAdapter> _playerDataReader;
		#endregion

		#region Init
		public PlayerData()
		{
			//_uuid = System.Guid.NewGuid().ToString();
			_idsTable = new Dictionary<Type, int>();
			_models = new Dictionary<int, PlayerDataModel>();
		}
		#endregion

		#region Controls
		public void MergeOnSync(PlayerData mergeData)
		{
			Debug.Log("Merge with");
		}

		public void AddModel<T>(int modelID) where T : PlayerDataModel
		{
			if (_models.ContainsKey(modelID))
			{
				_idsTable.Add(typeof(T), modelID);
				//Debug.Log("Model with this id (" + modelID + ") already added to player data: " + typeof(T));
			}
			else
			{
				_idsTable.Add(typeof(T), modelID);
				_models.Add(modelID, (T)Activator.CreateInstance(typeof(T)));
			}
		}
		#endregion
		
		#region Getters
		public T GetModel<T>() where T : PlayerDataModel
		{	
			Type key = typeof(T);
			if (_idsTable.ContainsKey(key))
			{
				int modelID = _idsTable[key];
				if (_models.ContainsKey(modelID))
					return _models[modelID] as T;
				else
					return null;
			} else
				return null;
		}
		#endregion

		#region Loading Data
		public static PlayerData Load(string filename)
		{
			PlayerData playerData = Reader.Load(filename);
			if (playerData == null) playerData = new PlayerData();
			return playerData;
		}
		#endregion

		#region Saving Data
		public static void Save(PlayerData data, string filename)
		{
			Reader.Save(data, filename);
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
		#endregion
	}
}
