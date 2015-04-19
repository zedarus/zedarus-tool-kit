using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Adapters;
using SimpleSQL;

namespace Zedarus.ToolKit.Data.Game.Models
{
	public class ModelCollection<T> : IModelCollection where T : Model
	{
		private string _tableName;
		private Dictionary<int, T> _models;
		private Dictionary<string, ModelCollectionIndex<string, T>> _indexes;
		private Dictionary<string, ModelCollectionIndex<int, T>> _indexesInt;
		private Dictionary<string, ModelCollectionIndex<float, T>> _indexesFloat;

		public ModelCollection()
		{
			T sampleInstance = (T)Activator.CreateInstance(typeof(T));
			_tableName = sampleInstance.GetDBTable();
			sampleInstance = null;
			_models = new Dictionary<int, T>();
			_indexes = new Dictionary<string, ModelCollectionIndex<string, T>>();
			_indexesInt = new Dictionary<string, ModelCollectionIndex<int, T>>();
			_indexesFloat = new Dictionary<string, ModelCollectionIndex<float, T>>();
		}

		private bool Add(int id, T model)
		{
			if (_models.ContainsKey(id))
				return false;

			Index(model);

			_models.Add(id, model);
			return true;
		}

		public bool LoadFromDB()
		{
			SimpleDataTable result = SQLiteAdapter.Manager.QueryGeneric("SELECT * FROM " + _tableName);
			for (int i = 0; i < result.rows.Count; i++)
			{
				T item = (T)Activator.CreateInstance(typeof(T), result.columns, result.rows[i]);
				Add(item.ID, item);
			}
			return false;
		}

		#region Get By ID
		public T Get(int id)
		{
			if (_models.ContainsKey(id))
				return _models[id];
			else
				return null;
		}

		// TODO: use index here instead
		public T this[int id]
		{
			get { return _models[id]; }
			set { _models[id] = value; }
		}
		#endregion

		public T First
		{
			get
			{
				T[] all = All;
				if (all != null)
					return all[0];
				else
					return null;
			}
		}

		public T Last
		{
			get
			{
				T[] all = All;
				if (all != null)
					return all[all.Length - 1];
				else
					return null;
			}
		}

		public T Random
		{
			get 
			{
				T[] all = All;
				if (all != null && all.Length > 0)
					return all[UnityEngine.Random.Range(0, all.Length)];
				else
					return null;
			}
		}

		#region First By
		public T GetFirstBy(Enum field, string value)
		{
			string fieldName = field.ToString();
			if (_indexes.ContainsKey(fieldName))
			{
				int id = _indexes[fieldName].Get(value);
				if (_models.ContainsKey(id))
					return _models[id];
				else
					return null;
			}
			else
				return null;
		}

		public T GetFirstBy(Enum field, int value)
		{
			string fieldName = field.ToString();
			if (_indexesInt.ContainsKey(fieldName))
			{
				int id = _indexesInt[fieldName].Get(value);
				if (_models.ContainsKey(id))
					return _models[id];
				else
					return null;
			}
			else
				return null;
		}

		public T GetFirstBy(Enum field, float value)
		{
			string fieldName = field.ToString();
			if (_indexesFloat.ContainsKey(fieldName))
			{
				int id = _indexesFloat[fieldName].Get(value);
				if (_models.ContainsKey(id))
					return _models[id];
				else
					return null;
			}
			else
				return null;
		}
		#endregion

		#region All By
		public T[] All
		{
			get
			{
				List<T> selected = new List<T>();
				foreach (KeyValuePair<int, T> model in _models)
					selected.Add(model.Value);

				return selected.ToArray();
			}
		}

		public T[] GetAllBy(Enum field, string value)
		{
			string fieldName = field.ToString();
			if (_indexes.ContainsKey(fieldName))
			{
				int[] ids = _indexes[fieldName].GetAll(value);
				List<T> selectedModels = new List<T>();
				foreach (int id in ids)
					selectedModels.Add(_models[id]);

				return selectedModels.ToArray();
			}
			else
				return null;
		}

		public T[] GetAllBy(Enum field, int value)
		{
			string fieldName = field.ToString();
			if (_indexesInt.ContainsKey(fieldName))
			{
				int[] ids = _indexesInt[fieldName].GetAll(value);
				List<T> selectedModels = new List<T>();
				foreach (int id in ids)
					selectedModels.Add(_models[id]);
				
				return selectedModels.ToArray();
			}
			else
				return null;
		}

		public T[] GetAllBy(Enum field, float value)
		{
			string fieldName = field.ToString();
			if (_indexesFloat.ContainsKey(fieldName))
			{
				int[] ids = _indexesFloat[fieldName].GetAll(value);
				List<T> selectedModels = new List<T>();
				foreach (int id in ids)
					selectedModels.Add(_models[id]);
				
				return selectedModels.ToArray();
			}
			else
				return null;
		}
		#endregion

		public void Reindex()
		{
			_indexes.Clear();
			_indexesInt.Clear();
			_indexesFloat.Clear();

			foreach (KeyValuePair<int, T> model in _models)
				Index(model.Value);
		}

		protected void Index(T model)
		{
			foreach (string index in model.GetIndexes())
			{
				Type propertyType = model.GetType().GetProperty(index).PropertyType;
				if (propertyType == typeof(int))
				{
					if (!_indexesInt.ContainsKey(index))
						_indexesInt.Add(index, new ModelCollectionIndex<int, T>(index));
				}
				else if (propertyType == typeof(float))
				{
					if (!_indexesFloat.ContainsKey(index))
						_indexesFloat.Add(index, new ModelCollectionIndex<float, T>(index));
				}
				else if (propertyType == typeof(string))
				{
					if (!_indexes.ContainsKey(index))
						_indexes.Add(index, new ModelCollectionIndex<string, T>(index));
				}
			}

			foreach (KeyValuePair<string, ModelCollectionIndex<string, T>> index in _indexes)
				index.Value.Add(model);

			foreach (KeyValuePair<string, ModelCollectionIndex<int, T>> index in _indexesInt)
				index.Value.Add(model);

			foreach (KeyValuePair<string, ModelCollectionIndex<float, T>> index in _indexesFloat)
				index.Value.Add(model);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return _models.GetEnumerator();
		}

		public int Count
		{
			get { return _models.Count; }
		}
	}
}
