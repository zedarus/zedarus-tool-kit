using System;
using UnityEngine;

namespace Zedarus.Toolkit.Data
{
	public interface IData
	{
		IDataEntry[] GetEntriesOfType(Type t);
		GUIContent[] GetEntriesNamesOfType(Type t);
	}
}