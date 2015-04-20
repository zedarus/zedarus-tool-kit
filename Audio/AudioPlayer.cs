using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Audio
{
	/// <summary>
	/// Audio player script that must be attached to GameObject and is used to play music.
	/// </summary>
	public class AudioPlayer : MonoBehaviour 
	{
		#region Singleton Stuff
		private static AudioPlayer instance = null;
		
	    void Awake() 
		{
	        if (instance != null && instance != this) 
			{
	            Destroy(gameObject);
	            return;
	        } 
			else 
			{
	            instance = this;
	        }
	        DontDestroyOnLoad(gameObject);
	    }
		#endregion
	}
}
