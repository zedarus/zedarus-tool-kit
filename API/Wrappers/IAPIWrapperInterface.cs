using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public interface IAPIWrapperInterface
	{
		event Action Initialized;
		
		void Init(object[] parameters);
		void Destroy();
		void SetAPI(int api);
		int API { get; }
	}
}
