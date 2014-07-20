using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Helpers.Assets
{
	public class AssetsLoader
	{
		static public Texture2D LoadTexture(string path, int width, int height)
		{
			TextAsset file = Resources.Load(path, typeof(TextAsset)) as TextAsset;
			Texture2D levelTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
			levelTexture.LoadImage(file.bytes);
			levelTexture.name = path + "_" + Random.Range(0, 1001);
			
			// TODO: this might cause memory leaks, so take care of created object later
			return levelTexture;
		}
	}
}
