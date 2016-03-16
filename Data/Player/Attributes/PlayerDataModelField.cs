using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Zedarus.ToolKit.Data.Player
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class DataField : System.Attribute
	{
		private int _buildNumber;

		public DataField(int buildNumber)
		{
			_buildNumber = buildNumber;
		}

		public int BuildNumber
		{
			get { return _buildNumber; }
		}
	}
}
