using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Helpers.Assets;

namespace Zedarus.ToolKit.NGUI
{
	[RequireComponent(typeof(UIRoot))]
	/// <summary>
	/// <para>GUI resolution switch controller is a special class that determines which assets (images, atlases, fonts) to use based on current screen resolution.</para>
	/// <para>Should be attached to the <see cref="UIRoot"/> object.</para>
	/// </summary>
	public class UIResolutionSwitch : MonoBehaviour 
	{
		#region Parameters
		static private float _resolutionScale = 1f;
		static private ResolutionType _resolution;
		public Camera guiCamera;
		public bool disableInEditor = true;
		public bool useCustomResolutionCutoffs = false;
		[SerializeField] public ScreenSize HDResolutionCutOff = ScreenSettings.Instance.GetHDResolutionCutoff();
		[SerializeField] public ScreenSize SHDResolutionCutOff = ScreenSettings.Instance.GetSHDResolutionCutoff();
		
		[SerializeField] public AtlasSettings[] atlases;
		[SerializeField] public FontSettings[] fonts;
		[SerializeField] public BackgroundSettings[] backgrounds;
		public GameObject[] repositionWithBackground;
		public GameObject[] resizeWithBackground;
		
		private UIAtlas ReplacementAtlas;
		private UIFont ReplacementFont;
		private Vector2 screenSize;
		#endregion
		
		#region Initialization
		private void Awake() 
		{
			if (disableInEditor && Application.isEditor)
				return;
			
			if (useCustomResolutionCutoffs)
				_resolution = ScreenHelper.GetResolutionTypeForCamera(guiCamera, false, HDResolutionCutOff, SHDResolutionCutOff);
			else
				_resolution = ScreenHelper.GetResolutionTypeForCamera(guiCamera, true);
			
			switch (_resolution)
			{
				case ResolutionType.SD:
					_resolutionScale = 0.5f;
					break;
				case ResolutionType.HD:
					_resolutionScale = 1f;
					break;
				case ResolutionType.SHD:
					_resolutionScale = 2f;
					break;
			}
			
			LoadAtlases(_resolution);
			LoadFonts(_resolution);
			
			screenSize = new Vector2(Screen.width, Screen.height);
			
			AdjustGUIHeight(_resolution);
			SwitchBackgroundTextures(_resolution);
		}
		
		private void Update() 
		{
			if (screenSize.x != Screen.width || screenSize.y != Screen.height) 
			{
				ResolutionType resolution;
				
				if (useCustomResolutionCutoffs)
					resolution = ScreenHelper.GetResolutionTypeForCamera(guiCamera, false, HDResolutionCutOff, SHDResolutionCutOff);
				else
					resolution = ScreenHelper.GetResolutionTypeForCamera(guiCamera, true);
				
				SwitchBackgroundTextures(resolution);
				screenSize = new Vector2(Screen.width, Screen.height);
			}
		}
		
		private void OnDestroy()
		{
			foreach (BackgroundSettings backgroundSettings in backgrounds) 
			{
				UITexture t = backgroundSettings.background.GetComponent<UITexture>();
				if (t != null && t.mainTexture != null)
				{
					Destroy(t.mainTexture);
				}
			}
		}
		
		static public float ResolutionScale
		{
			get { return _resolutionScale; }
		}
		
		static public ResolutionType Resolution
		{
			get { return _resolution; }
		}
		
		/// <summary>
		/// Adjusts the height of the NGUI so the UI elements maintain the same size and only change their position on the screen.
		/// </summary>
		/// <param name='resolution'>
		/// Resolution density type.
		/// </param>
		private void AdjustGUIHeight(ResolutionType resolution) 
		{
			float pixelSize = 1f;
			Rect screenSize = new Rect(0, 0, guiCamera.pixelWidth, guiCamera.pixelHeight);
			
			#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android) 
			{
				screenSize.width = DisplayMetricsAndroid.WidthPixels;
				screenSize.height = DisplayMetricsAndroid.HeightPixels;
			}
			#endif
			
			UIRoot root = gameObject.GetComponent<UIRoot>();
			root.manualHeight = Mathf.RoundToInt(screenSize.height * pixelSize / ResolutionScale);
		}
		#endregion
		
		#region Background
		// TODO: add docs
		public void SetBackground(string filename)
		{
			if (filename == null || filename.Length < 1)
				return;
			
			foreach (BackgroundSettings backgroundSettings in backgrounds) 
			{
				backgroundSettings.filename = filename;
			}
		}
		
		/// <summary>
		/// Switches the background textures based on current screen resolution type.
		/// </summary>
		/// <param name='resolution'>
		/// Resolution type.
		/// </param>
		private void SwitchBackgroundTextures(ResolutionType resolution) 
		{
			Texture2D replacementTexture;
			string path;
			
			foreach (BackgroundSettings backgroundSettings in backgrounds) 
			{
				switch (resolution) 
				{
					case ResolutionType.SD:
						path = backgroundSettings.SDTexturePath;
						break;
					case ResolutionType.SHD:
						path = backgroundSettings.SHDTexturePath;
						break;
					case ResolutionType.HD:
					default:
						path = backgroundSettings.HDTexturePath;
						break;
				}
				
				path += backgroundSettings.filename;
				
				//replacementTexture = LoadBackgroundTextureFromResources(path);
				replacementTexture = LoadBackgroundTextureFromResourcesAsBinary(path);
				backgroundSettings.background.GetComponent<UITexture>().mainTexture = replacementTexture;
				ResizeBackground(backgroundSettings.background, backgroundSettings.align);
			}
		}
	
		/// <summary>
		/// Loads the background texture from resources folder.
		/// </summary>
		/// <returns>
		/// The background <see cref="Texture2D"/>.
		/// </returns>
		/// <param name='path'>
		/// Path to the texture file.
		/// </param>
		private Texture2D LoadBackgroundTextureFromResources(string path) 
		{
			return Resources.Load(path, typeof(Texture2D)) as Texture2D;
		}
		
		// TODO: add docs
		private Texture2D LoadBackgroundTextureFromResourcesAsBinary(string path)
		{
			return AssetsLoader.LoadTexture(path, Mathf.FloorToInt(1280 * ResolutionScale), Mathf.FloorToInt(800 * ResolutionScale));
		}
		
		/// <summary>
		/// Resizes the background image to fit into current screen resolution.
		/// <para>Also resizes and repositions all objects whose position is related to background image.</para>
		/// </summary>
		/// <param name='background'>
		/// Background <see cref="GameObject"/>.
		/// </param>
		private void ResizeBackground(GameObject background, BackgroundAlign align) 
		{
			Rect screenSize = new Rect(0, 0, guiCamera.pixelWidth, guiCamera.pixelHeight);
			
			#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android) 
			{
				screenSize.width = DisplayMetricsAndroid.WidthPixels;
				screenSize.height = DisplayMetricsAndroid.HeightPixels;
			}
			#endif
			
			float cameraScale = GetComponent<UIRoot>().activeHeight / screenSize.height;
			Vector3 backgroundScale = background.transform.localScale;
			float scaleWidth = (screenSize.width * cameraScale) / backgroundScale.x;
			float scaleHeight = (screenSize.height * cameraScale) / backgroundScale.y;
			float totalScale = scaleHeight;
			
			background.transform.localScale = new Vector3(Mathf.RoundToInt(backgroundScale.x * totalScale), Mathf.RoundToInt(backgroundScale.y * totalScale), 1);
			
			if (background.transform.localScale.x < screenSize.width * cameraScale) 
			{
				totalScale = scaleWidth;
				background.transform.localScale = new Vector3(Mathf.RoundToInt(backgroundScale.x * totalScale), Mathf.RoundToInt(backgroundScale.y * totalScale), 1);
			}
			
			Vector3 position = background.transform.localPosition;
			switch (align) 
			{
				case BackgroundAlign.Bottom:
					position.y += (background.transform.localScale.y - GetComponent<UIRoot>().activeHeight) * 0.5f;
					break;
			}
			background.transform.localPosition = position;
			
			Vector3 previousPosition;
			foreach (GameObject objectToReposition in repositionWithBackground) 
			{
				previousPosition = objectToReposition.transform.localPosition;
				previousPosition *= totalScale;
				previousPosition.x = Mathf.RoundToInt(previousPosition.x);
				previousPosition.y = Mathf.RoundToInt(previousPosition.y);
				previousPosition.z = Mathf.RoundToInt(previousPosition.z);
				objectToReposition.transform.localPosition = previousPosition;
			}
			
			Vector3 previousScale;
			Vector4 scaleOld;
			UIPanel panel;
			foreach (GameObject objectToResize in resizeWithBackground) 
			{
				panel = objectToResize.GetComponent<UIPanel>();
				if (panel != null) 
				{
					scaleOld = panel.baseClipRegion;
					scaleOld.z *= totalScale;
					scaleOld.w *= totalScale;
					panel.baseClipRegion = scaleOld;
				} 
				else 
				{
					previousScale = objectToResize.transform.localScale;
					previousScale *= totalScale;
					objectToResize.transform.localScale = previousScale;
				}
			}
		}
		#endregion
		
		#region Atlases
		/// <summary>
		/// Loads the atlases for current resolution.
		/// </summary>
		/// <param name='resolution'>
		/// Resolution.
		/// </param>
		private void LoadAtlases(ResolutionType resolution) 
		{
			UIAtlas replacementAtlas;
			string path;
			
			foreach (AtlasSettings atlasSettings in atlases) 
			{
				switch (resolution) 
				{
					case ResolutionType.SD:
						path = atlasSettings.SDAtlasPath;
						break;
					case ResolutionType.SHD:
						path = atlasSettings.SHDAtlasPath;
						break;
					case ResolutionType.HD:
					default:
						path = atlasSettings.HDAtlasPath;
						break;
				}
				replacementAtlas = Resources.Load(path, typeof(UIAtlas)) as UIAtlas;
				atlasSettings.referenceAtlas.replacement = replacementAtlas.GetComponent<UIAtlas>();
			}
		}
		#endregion
		
		#region Fonts
		/// <summary>
		/// Loads the fonts for current resolution.
		/// </summary>
		/// <param name='resolution'>
		/// Resolution.
		/// </param>
		private void LoadFonts(ResolutionType resolution) 
		{
			UIFont replacementFont;
			string path;
			
			foreach (FontSettings fontSettings in fonts) 
			{
				switch (resolution) 
				{
					case ResolutionType.SD:
						path = fontSettings.SDFontPath;
						break;
					case ResolutionType.SHD:
						path = fontSettings.SHDFontPath;
						break;
					case ResolutionType.HD:
					default:
						path = fontSettings.HDFontPath;
						break;
				}

				replacementFont = Resources.Load(path, typeof(UIFont)) as UIFont;
				fontSettings.referenceFont.replacement = replacementFont.GetComponent<UIFont>();
			}
		}
		#endregion

		#region Settings Classes
		[System.Serializable]
		public class AtlasSettings {
			public UIAtlas referenceAtlas;
			public string SDAtlasPath;
			public string HDAtlasPath;
			public string SHDAtlasPath;
		}
		
		[System.Serializable]
		public class FontSettings {
			public UIFont referenceFont;
			public string SDFontPath;
			public string HDFontPath;
			public string SHDFontPath;
		}
		
		public enum BackgroundAlign { Center, Bottom };
		
		[System.Serializable]
		public class BackgroundSettings {
			public GameObject background;
			public BackgroundAlign align = BackgroundAlign.Center;
			public string filename;
			public string SDTexturePath;
			public string HDTexturePath;
			public string SHDTexturePath;
		}
		#endregion
	}
}