using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI.Elements
{
	public interface IUICarouselScrollPage
	{
		void InitAsPage(float pageWidth, float parallax);
		Transform PageTransform { get; }
		float GraphicsSize { get; }
		void UpdateScrollPivot(Vector2 pivot);
	}
}
