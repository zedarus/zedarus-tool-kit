using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player.Models
{
	public interface IPlayerDataModel<T>
	{
		//string UUID { get; }
		void Reset();
		bool Merge(T data);
	}
}
