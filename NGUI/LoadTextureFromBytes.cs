using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Helpers.Assets;

namespace Zedarus.ToolKit.NGUI
{
	#if !ZTK_DISABLE_NGUI
	[System.Serializable]
	public class ImagePath 
	{
		public string SD;
		public string HD;
		public string SHD;
	}
	
	public class LoadTextureFromBytes : MonoBehaviour
	{
		#region Parameters
		public bool autoLoad = true;
		public bool ngui = true;
		public ImagePath texturePath;
		public int width = 1280;
		public int height = 800;
		public float alpha = 1f;
		#endregion
		
		#region Unity Methods
		private void Awake()
		{
			if (autoLoad) Load();
		}
		
		private void OnDestroy()
		{
			if (ngui)
			{
				UITexture t = GetComponent<UITexture>();
				if (t != null)
				{
					if (t.mainTexture != null) 
					{
						Destroy(t.mainTexture);
					}
					t.mainTexture = null;
				}
			}
			else
			{
				if (renderer != null && renderer.material.mainTexture != null)
				{
					Destroy(renderer.material.mainTexture);
					renderer.material.mainTexture = null;
				}
			}
		}
		
		public void Load()
		{
			string path = "";
				
			switch (UIResolutionSwitch.Resolution)
			{
				case ResolutionType.HD:
					path = texturePath.HD;
					break;
				case ResolutionType.SD:
					path = texturePath.SD;
					break;
				case ResolutionType.SHD:
					path = texturePath.SHD;
					break;
			}
			
			Texture2D texture = AssetsLoader.LoadTexture(path, Mathf.FloorToInt((float)width * UIResolutionSwitch.ResolutionScale), Mathf.FloorToInt((float)height * UIResolutionSwitch.ResolutionScale));
			
			if (ngui)
			{
				if (texture != null)
				{
					UITexture t = GetComponent<UITexture>();
					if (t != null)
					{
						t.mainTexture = texture;
						t.alpha = alpha;
					}
				}
			}
			else
			{
				if (renderer != null)
					renderer.material.mainTexture = texture;
			}
		}
		#endregion
	}
	#endif
}
