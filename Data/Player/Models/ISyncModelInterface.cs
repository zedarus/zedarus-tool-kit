using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player.Models
{
	public interface ISyncModelInterface<T>
	{
		//string UUID { get; }	
		bool Merge(T data);
	}
}
