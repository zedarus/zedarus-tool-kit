using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public interface IRemoteDataWrapperInterface
	{
		#region Events
		event Action<string> DataReceived;
		#endregion

		#region Controls
		void RequestData();
		#endregion

		#region Queries
		string RemoteData { get; }
		#endregion
	}
}
