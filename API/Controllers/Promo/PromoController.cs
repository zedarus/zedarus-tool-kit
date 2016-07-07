using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class PromoController : APIController 
	{	
		#region Events
		#endregion

		#region Properties
		#endregion

		#region Initialization
		protected override void Setup() { }
		#endregion

		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Promo.Batch:
					return BatchWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls
		public void RequestNotificationsPermission()
		{
			if (Wrapper != null)
			{
				Wrapper.RequestNotificationsPermission();
			}
		}
		#endregion

		#region Queries

		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

			foreach (IPromoWrapperInterface wrapper in Wrappers)
			{
				
			}
		}

		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();

			foreach (IPromoWrapperInterface wrapper in Wrappers)
			{
				
			}
		}
		#endregion

		#region Event Handlers

		#endregion

		#region Getters
		protected IPromoWrapperInterface Wrapper
		{
			get { return (IPromoWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
	}
}
