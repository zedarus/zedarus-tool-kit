using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.PoolManagement
{
	public interface IPoolItem
	{
		void Init();
		void Activate(Vector3 position);
		void Deactivate();
	}
}
