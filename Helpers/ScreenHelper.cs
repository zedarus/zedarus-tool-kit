using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Helpers
{
	public enum ResolutionType 
	{ 
		None, 
		SD, 
		HD, 
		SHD 
	};

	public enum ScreenRatio
	{
		r3x4,
		r4x3,
		r2x3,
		r3x2,
		r5x3,
		r3x5,
		r9x16,
		r16x9,
		r16x10,
		r10x16,
	}
	
	public class ScreenHelper 
	{
		private const float s3x4 	= 3.0f	/ 4.0f;
		private const float s4x3 	= 4.0f	/ 3.0f;
		private const float s2x3 	= 2.0f	/ 3.0f;
		private const float s3x2 	= 3.0f	/ 2.0f;
		private const float s5x3		= 5.0f / 3.0f;
		private const float s3x5		= 3.0f / 5.0f;
		private const float s9x16 	= 9.0f	/ 16.0f;
		private const float s16x9 	= 16.0f	/ 9.0f;
		private const float s16x10 	= 16.0f	/ 10.0f;
		private const float s10x16 	= 10.0f	/ 16.0f;

		static private ResolutionType cachedResolution = ResolutionType.None;

		static public bool IsIpad()
		{
			return Screen.height == 1024 || Screen.height == 2048;
		}

		static public ResolutionType GetResolutionType(int limitHD, int limitSHD)
		{
			return GetResolutionTypeForCamera(Camera.main, limitHD, limitSHD, true);
		}

		static public ResolutionType GetResolutionTypeForCamera(Camera camera, int limitHD, int limitSHD, bool useCachedIfPossible = false) {
			if (useCachedIfPossible && ScreenHelper.cachedResolution != ResolutionType.None)
				return ScreenHelper.cachedResolution;
			
			if (camera == null)
				return ResolutionType.None;

			ResolutionType resolution;
			Rect screenSize = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);
			
			#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android) {
				screenSize.width = DisplayMetricsAndroid.WidthPixels;
				screenSize.height = DisplayMetricsAndroid.HeightPixels;
			}
			#endif
			
			resolution = ResolutionType.HD;
			
			if(screenSize.height >= limitSHD) 
			{
				resolution = ResolutionType.SHD;
				ZedLogger.Log("Setting texture to SHD");
			}
			else if(screenSize.height >= limitHD) 
			{
				resolution = ResolutionType.HD;
				ZedLogger.Log("Setting texture to HD");
			}
			else 
			{
				resolution = ResolutionType.SD;
				ZedLogger.Log("Setting texture to SD");
			}

			ScreenHelper.cachedResolution = resolution;
			
			return resolution;
		}

		public static ScreenRatio GetAspectRatioForResolution(int width, int height) 
		{
			float ratio = (float)width / (float)height;

			Dictionary<float, ScreenRatio> ratios = new Dictionary<float, ScreenRatio>();
			ratios.Add(s3x4, ScreenRatio.r3x4);
			ratios.Add(s4x3, ScreenRatio.r4x3);
			ratios.Add(s2x3, ScreenRatio.r2x3);
			ratios.Add(s3x2, ScreenRatio.r3x2);
			ratios.Add(s5x3, ScreenRatio.r5x3);
			ratios.Add(s3x5, ScreenRatio.r3x5);
			ratios.Add(s9x16, ScreenRatio.r9x16);
			ratios.Add(s16x9, ScreenRatio.r16x9);
			ratios.Add(s16x10, ScreenRatio.r16x10);
			ratios.Add(s10x16, ScreenRatio.r10x16);
			
			ScreenRatio nearestRatio = ScreenRatio.r16x9;
			float ratioDiff = float.MaxValue;
			float diff;
			
			foreach (KeyValuePair<float, ScreenRatio> r in ratios) {
				diff = Mathf.Abs(r.Key - ratio);
				if (diff < ratioDiff) {
					ratioDiff = diff;
					nearestRatio = r.Value;
				}
			}
			
			return nearestRatio;
		}
	}
}
