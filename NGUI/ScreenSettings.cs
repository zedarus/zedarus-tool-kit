using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.NGUI
{
	#if !ZTK_DISABLE_NGUI
	public class ScreenSettings : SimpleSingleton<ScreenSettings> 
	{
		#region Parameters
		private ScreenSize HDResolutionCutoff;
		private ScreenSize SHDResolutionCutoff;
		#endregion
		
		#region Initialization
		/// <summary>
		/// Initializes a new instance of the <see cref="GlobalSettings"/> class with default values.
		/// </summary>
		public ScreenSettings() 
		{
			HDResolutionCutoff	= new ScreenSize(960, 480);
			SHDResolutionCutoff	= new ScreenSize(1536, 1000);
		}
		#endregion
		
		/// <summary>
		/// Gets the default HD resolution cutoff.
		/// </summary>
		/// <returns>
		/// The HD resolution cutoff (width and height).
		/// </returns>
		public ScreenSize GetHDResolutionCutoff() 
		{
			return new ScreenSize(HDResolutionCutoff.width, HDResolutionCutoff.height);
		}
		
		/// <summary>
		/// Gets the default SHD resolution cutoff.
		/// </summary>
		/// <returns>
		/// The SHD resolution cutoff (width and height).
		/// </returns>
		public ScreenSize GetSHDResolutionCutoff() 
		{
			return new ScreenSize(SHDResolutionCutoff.width, SHDResolutionCutoff.height);
		}
	}
	#endif
}
