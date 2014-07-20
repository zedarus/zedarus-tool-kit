using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Models
{
	public interface ISyncModelInterface<T>
	{
		string UUID { get; }
		
		void Merge(T data);
	}
}
