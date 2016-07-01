using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public interface IShareWrapperInterface
	{
		#region Events
		//event Action SharingStarted;
		//event Action<bool> SharingFinished;
		#endregion
		
		#region Controls
		void Share(string text, string link, string imagePath);
		#endregion
	}
}
