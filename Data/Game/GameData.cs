using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Helpers;
using Zedarus.ToolKit.Data.Adapters;
using Zedarus.ToolKit.Data.Game.Models;
using SimpleSQL;

namespace Zedarus.ToolKit.Data.Game
{
	public class GameData
	{
		#region Properties
		protected DateTime _date;
		protected int _version;
		#endregion

		#region Models
		protected ModelCollection<Enemy> _enemies;
		#endregion

		#region Init
		public GameData()
		{
			_version = 0;
			Load();
		}
		#endregion

		#region Loading Data
		protected virtual void Load()
		{
			LoadVersionData();
			_enemies = LoadFromDB<Enemy>(Enemy.GetDBTable());
		}

		protected virtual void LoadVersionData()
		{
			SimpleDataTable result = SQLiteAdapter.Manager.QueryGeneric("SELECT * FROM version");
			string version = result.rows[0].fields[1].ToString();
			string date = result.rows[0].fields[2].ToString();
			
			int.TryParse(version, out _version);
			_date = GeneralHelper.ParseTime(date);
		}

		private ModelCollection<T> LoadFromDB<T>(string table) where T : Model
		{
			ModelCollection<T> container = new ModelCollection<T>();
			SimpleDataTable result = SQLiteAdapter.Manager.QueryGeneric(LoadFromDBQuery(table + TableNameSuffix));
			for (int i = 0; i < result.rows.Count; i++)
			{
				T item = (T)Activator.CreateInstance(typeof(T), result.columns, result.rows[i]);
				container.Add(item.ID, item);
			}
			return container;
		}

		protected virtual string LoadFromDBQuery(string table)
		{
			return "SELECT * FROM " + table;
		}
		#endregion

		#region Saving Data
		protected virtual void Save()
		{
			SaveVersionData();
			SaveToDB<Enemy>(_enemies, Enemy.GetDBTable());
		}

		protected virtual void SaveVersionData()
		{
			string query = "UPDATE version SET version = ?, date = ? WHERE id = 1";
			SQLiteAdapter.Manager.Execute(query, _version.ToString(), _date.ToString("yyyy-MM-dd HH:mm:ss"));
		}

		private void SaveToDB<T>(ModelCollection<T> collection, string table) where T : Model
		{
			string fullTableName = table + TableNameSuffix;
			ClearTable(fullTableName);

			foreach (KeyValuePair<int, T> item in collection)
				SaveModelToDB(item.Value, fullTableName);
		}

		protected virtual void SaveModelToDB(Model model, string table)
		{
			SimpleDataTable result = SQLiteAdapter.Manager.QueryGeneric("SELECT * FROM " + table + " WHERE id = " + model.ID);
			if (result.rows.Count > 0)
				SQLiteAdapter.Manager.Execute(model.GetUpdateQuery(table));
			else
				SQLiteAdapter.Manager.Execute(model.GetInsertQuery(table));
		}

		protected virtual void ClearTable(string table)
		{
			// TODO: this potentially might lead to data corruption if interrupted
			string query = "DELETE FROM " + table + " WHERE 1 = 1";
			SQLiteAdapter.Manager.Execute(query);
		}
		#endregion

		#region Merge
		public void Merge(GameData data)
		{
			if (_version < data.Version)
				ApplyNewVersion(data);
			Save();
		}

		private void ApplyNewVersion(GameData data)
		{
			_version = data.Version;
			_date = data.Date;

			MergeModels<Enemy>(_enemies, data.Enemies);
		}

		private void MergeModels<T>(ModelCollection<T> oldModels, ModelCollection<T> newModels) where T : Model
		{
			foreach (KeyValuePair<int, T> newModel in newModels)
				oldModels[newModel.Key] = newModel.Value;

			oldModels.Reindex();
		}
		#endregion

		#region Queries
		public ModelCollection<Enemy> Enemies { get { return _enemies; } }
		#endregion

		#region Getters
		public int Version
		{
			get { return _version; }
		}

		public DateTime Date
		{
			get { return _date; }
		}

		protected virtual string TableNameSuffix
		{
			get { return ""; }
		}
		#endregion

		/// <summary>
		/// This method is used to fix AOT bug on iOS/Android.
		/// DO NOT CALL THIS METHOD ANYWHERE IN YOUR CODE!
		/// </summary>
		private void AOTBugFix()
		{
			Dictionary<int, Enemy> o6 = new Dictionary<int, Enemy>();

			Debug.Log(o6);

			ModelCollectionIndex<string, Enemy> a6 = new ModelCollectionIndex<string, Enemy>(null);

			Debug.Log(a6);
			ModelCollectionIndex<int, Enemy> b6 = new ModelCollectionIndex<int, Enemy>(null);

			Debug.Log(b6);
			ModelCollectionIndex<float, Enemy> c6 = new ModelCollectionIndex<float, Enemy>(null);

			Debug.Log(c6);
		}
	}
}

