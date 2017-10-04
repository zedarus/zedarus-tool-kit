using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zedarus.Toolkit.Data
{
	[Serializable]
	public class DataLinksManager
	{
		[SerializeField]
		private DataLink[] _links;

		public void AddLink(string fieldName, string dataGUID)
		{
			if (_links == null)
			{
				_links = new DataLink[0];
			}
			
			var links = new List<DataLink>(_links);
			links.Add(new DataLink(fieldName, dataGUID));
			_links = links.ToArray();
			links.Clear();
		}

		/// <summary>
		/// Pulls latest values from DB and applies them to all linked fields
		/// </summary>
		public void UpdateValues()
		{
			
		}
	}
}
