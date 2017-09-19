using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player
{
	// Read about version toleran serialization: https://msdn.microsoft.com/en-us/library/ms229752(v=vs.110).aspx
	public interface IPlayerDataModel
	{
		void Reset();
		bool Merge(IPlayerDataModel data);
	}
}
