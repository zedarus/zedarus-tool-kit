using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
#if API_SHARE_NATIVE
using Prime31;
#endif

namespace Zedarus.ToolKit.API
{
	public class NativeShareWrapper : APIWrapper<NativeShareWrapper>, IShareWrapperInterface 
	{
		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion
		
		#region Controls
		public void Share(string text, string link, string imagePath) 
		{
			#if API_SHARE_NATIVE
			List<string> items = new List<string>();

			if (text != null)
			{
				items.Add(text);
			}
			if (link != null)
			{
				items.Add(link);
			}
			if (imagePath != null)
			{
				items.Add(imagePath);
			}

			SharingBinding.shareItems(items.ToArray());

			items.Clear();
			#endif
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			
		}
		
		protected override void RemoveEventListeners() 
		{
			
		}
		#endregion
	}
}
