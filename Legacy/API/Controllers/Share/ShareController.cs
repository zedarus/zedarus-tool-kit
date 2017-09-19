using UnityEngine;
using System;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class ShareController : APIController
	{
		#region Parameters
//		private SocialAction _action;
		#endregion
		
		#region Events
//		public event Action SharingStarted;
//		public event Action<bool> SharingFinished;
		#endregion
		
		#region Initialization
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Sharing.Native:
					return NativeShareWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void Share(string text, string link, string imagePath)
		{
			IShareWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
			{
				wrapper.Share(text, link, imagePath);
			}
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();
			
//			foreach (ISocialWrapperInterface wrapper in Wrappers)
//			{
//				wrapper.SharingStarted += OnSharingStarted;
//				wrapper.SharingFinished += OnSharingFinished;
//			}
		}
		
		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();
			
//			foreach (ISocialWrapperInterface wrapper in Wrappers)
//			{
//				wrapper.SharingStarted -= OnSharingStarted;
//				wrapper.SharingFinished -= OnSharingFinished;
//			}
		}
		#endregion
		
		#region Event Handlers
		#endregion
		
		#region Getters
		protected IShareWrapperInterface Wrapper
		{
			get { return (IShareWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
	}
}
