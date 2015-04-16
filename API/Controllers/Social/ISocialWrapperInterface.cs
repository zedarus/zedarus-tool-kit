using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.API
{
	public interface ISocialWrapperInterface
	{
		#region Events
		event Action SharingStarted;
		event Action<bool> SharingFinished;
		#endregion
		
		#region Controls
		void PostTextAndImage(string subject, string text, string imagePath, byte[] image, string url);
		void Share(string link, string name, string caption, string description, string pictureURL);
		#endregion
	}
}
