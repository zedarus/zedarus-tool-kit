using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Data.Game.Models
{
	public class ModelCollectionIndex<T, M> where M : Model
	{
		private string _field;
		private List<KeyValuePair<T, int>> _entries;

		public ModelCollectionIndex(string field)
		{
			_field = field;
			_entries = new List<KeyValuePair<T, int>>();
		}

		public int Get(T key)
		{
			foreach (KeyValuePair<T, int> entry in _entries)
			{
				if (entry.Key.Equals(key))
					return entry.Value;
			}

			return -1;
		}

		public int[] GetAll(T key)
		{
			List<int> ids = new List<int>();

			foreach (KeyValuePair<T, int> entry in _entries)
			{
				if (entry.Key.Equals(key))
					ids.Add(entry.Value);
			}

			return ids.ToArray();
		}

		public void Add(M model)
		{
			T value = (T) model.GetType().GetProperty(_field).GetValue(model, null);
			_entries.Add(new KeyValuePair<T, int>(value, model.ID));
		}

		/// <summary>
		/// This method is used to fix AOT bug on iOS/Android.
		/// DO NOT CALL THIS METHOD ANYWHERE IN YOUR CODE!
		/// </summary>
		private void AOTBugFix()
		{
			List<KeyValuePair<int, int>> a = new List<KeyValuePair<int, int>>();
			List<KeyValuePair<float, int>> b = new List<KeyValuePair<float, int>>();
			List<KeyValuePair<string, int>> c = new List<KeyValuePair<string, int>>();

			Debug.Log(a + "" + b + "" + c);
		}
	}
}
