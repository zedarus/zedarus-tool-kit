using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Audio
{
	#if NGUI
	/// <summary>
	/// NGUI button sound component that extends <see cref="UIButtonSound"/> to use BPAudioManager to play sounds instead of standard sound system.
	/// </summary>
	public class ButtonSound : UIPlaySound 
	{
		public SoundType defaultSound = SoundType.None;
		
		void OnHover (bool isOver) 
		{
			if (enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut))) 
				AudioManager.Instance.PlaySound(clip);
		}
	
		void OnPress (bool isPressed) 
		{
			if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease))) 
				AudioManager.Instance.PlaySound(clip);
		}
	
		void OnClick () 
		{
			if (enabled && trigger == Trigger.OnClick) 
				AudioManager.Instance.PlaySound(clip);
		}
		
		private AudioClip clip
		{
			get 
			{
				if (defaultSound != SoundType.None)
					return SoundsManager.Instance.GetAudioClip(defaultSound, false);
				else
					return audioClip;
			}
		}
	}
	#endif
}
