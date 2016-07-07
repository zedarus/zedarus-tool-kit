using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class BatchWrapperSettings : APIWrapperSettings
	{
		private string _filename = "";

		public BatchWrapperSettings(object[] settings) : base(settings)
		{
			Assert.IsTrue(settings.Length > 0, "Incorrect number of parameters for ICloud wrapper");
			Assert.IsTrue(settings[0].GetType() == typeof(string), "First parameter must be string");

			_filename = settings[0].ToString();
		}

		public string Filename
		{
			get { return _filename; }
		}
	}

	public class BatchWrapper : APIWrapper<BatchWrapper>, IPromoWrapperInterface 
	{
		#region Parameters
		#endregion

		#region Events
		#endregion

		#region Properties
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			#if UNITY_IPHONE && API_SYNC_ICLOUD
			BatchWrapperSettings batchSettings = settings as BatchWrapperSettings;
			if (batchSettings != null)
			{
//				_filename = batchSettings.Filename;
			}

			#endif

			SendInitializedEvent();	// TODO: temp
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return new ICloudWrapperSettings(settings);
		}
		#endregion

		#region Controls

		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			
		}

		protected override void RemoveEventListeners() 
		{
			
		}
		#endregion

		#region Event Handlers

		#endregion
	}
}
