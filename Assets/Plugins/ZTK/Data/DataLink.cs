using System;
using UnityEngine;

namespace Zedarus.Toolkit.Data
{
	[Serializable]
	public class DataLink
	{
		[SerializeField]
		private string _propertyName;
		[SerializeField]
		private string _linkedData;

		public DataLink(string fieldName, string dataGUID)
		{
			_propertyName = fieldName;
			_linkedData = dataGUID;
		}
	}
}
