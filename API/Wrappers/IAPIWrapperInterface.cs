using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public interface IAPIWrapperInterface
	{
		event Action Initialized;
		
		void Init();
		void Destroy();
		void SetAPI(APIs api);
		APIs API { get; }
	}
}
