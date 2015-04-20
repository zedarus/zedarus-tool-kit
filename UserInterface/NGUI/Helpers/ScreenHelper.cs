using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.NGUI
{
	#if ZTK_NGUI
	public enum ResolutionType { None, SD, HD, SHD };
	
	public static class ScreenAspectRatio 
	{
		public const float s3x4 	= 3.0f	/ 4.0f;
		public const float s4x3 	= 4.0f	/ 3.0f;
		public const float s2x3 	= 2.0f	/ 3.0f;
		public const float s3x2 	= 3.0f	/ 2.0f;
		public const float s5x3		= 5.0f / 3.0f;
		public const float s3x5		= 3.0f / 5.0f;
		public const float s9x16 	= 9.0f	/ 16.0f;
		public const float s16x9 	= 16.0f	/ 9.0f;
		public const float s16x10 	= 16.0f	/ 10.0f;
		public const float s10x16 	= 10.0f	/ 16.0f;
		
		public const float GenericTabletWide	= ScreenAspectRatio.s4x3;
		public const float GenericTabletTall	= ScreenAspectRatio.s3x4;
		public const float GenericPhoneWide		= ScreenAspectRatio.s3x2;
		public const float GenericPhoneTall		= ScreenAspectRatio.s2x3;
		
		public static float GetAspectRatioForResolution(Rect screen) 
		{
			float ratio = (float)screen.width / (float)screen.height;
			
			float[] ratios = new float[10];
			ratios[0] = s3x4;
			ratios[1] = s4x3;
			ratios[2] = s2x3;
			ratios[3] = s3x2;
			ratios[4] = s5x3;
			ratios[5] = s3x5;
			ratios[6] = s9x16;
			ratios[7] = s16x9;
			ratios[8] = s16x10;
			ratios[9] = s10x16;
			
			float nearestRatio = GenericPhoneWide;
			float ratioDiff = float.MaxValue;
			float diff;
			
			foreach (float r in ratios) 
			{
				diff = Mathf.Abs(r - ratio);
				if (diff < ratioDiff) 
				{
					ratioDiff = diff;
					nearestRatio = r;
				}
			}
			
			return nearestRatio;
		}
	}
	
	[System.Serializable]
	public class ScreenSize 
	{
		public int width;
		public int height;
		
		public ScreenSize(int newWidth, int newHeight) 
		{
			width = newWidth;
			height = newHeight;
		}
	}
	
	public class ScreenHelper 
	{
		static private ResolutionType cachedResolution = ResolutionType.None;

		static public bool IsIpad()
		{
			return Screen.height == 768 || Screen.height == 1536;
		}

		static public float width
		{
			get { return Screen.width / UIResolutionSwitch.ResolutionScale; }
		}

		static public float height
		{
			get { return Screen.height / UIResolutionSwitch.ResolutionScale; }
		}

		/// <summary>
		/// Gets the resolution type for camera.
		/// </summary>
		/// <returns>
		/// The <see cref="ResolutionType"/> for camera: SD, HD or SHD
		/// </returns>
		/// <param name='camera'>
		/// Camera object.
		/// </param>
		/// <param name='customHDResolutionCutOff'>
		/// Custom HD resolution cut off.
		/// </param>
		/// <param name='customSHDResolutionCutOff'>
		/// Custom SHD resolution cut off.
		/// </param>
		static public ResolutionType GetResolutionTypeForCamera(Camera camera, bool useCachedIfPossible = false, ScreenSize customHDResolutionCutOff = null, ScreenSize customSHDResolutionCutOff = null) 
		{
			if (useCachedIfPossible && ScreenHelper.cachedResolution != ResolutionType.None)
				return ScreenHelper.cachedResolution;
			
			if (camera == null)
				return ResolutionType.None;
			
			ScreenSize HDResolutionCutOff;
			ScreenSize SHDResolutionCutOff;
			ResolutionType resolution;
			
			if (customHDResolutionCutOff == null)
				HDResolutionCutOff = ScreenSettings.Instance.GetHDResolutionCutoff();
			else
				HDResolutionCutOff = customHDResolutionCutOff;
			
			if (customSHDResolutionCutOff == null) 
				SHDResolutionCutOff = ScreenSettings.Instance.GetSHDResolutionCutoff();
			else
				SHDResolutionCutOff = customSHDResolutionCutOff;
			
			Rect screenSize = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);
			
			#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android)
			{
				screenSize.width = DisplayMetricsAndroid.WidthPixels;
				screenSize.height = DisplayMetricsAndroid.HeightPixels;
			}
			#endif
			
			resolution = ResolutionType.HD;
			
			if (screenSize.height >= SHDResolutionCutOff.height) 
			{
				resolution = ResolutionType.SHD;
				ZedLogger.Log("Setting texture to SHD");
			}
			else if (screenSize.height >= HDResolutionCutOff.height) 
			{
				resolution = ResolutionType.HD;
				ZedLogger.Log("Setting texture to HD");
			}
			else 
			{
				resolution = ResolutionType.SD;
				ZedLogger.Log("Setting texture to SD");
			}
			
			if (useCachedIfPossible)
				ScreenHelper.cachedResolution = resolution;
			
			return resolution;
		}
	}
#endif
}
