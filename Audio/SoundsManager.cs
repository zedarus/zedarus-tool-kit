using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.API;

namespace Zedarus.ToolKit.Audio
{
	public enum SoundType
	{
		ButtonClick,
		PageSwitch,
		Popup,
		NumberFind,
		LevelCompletion,
		Task,
		None
	}
	
	public class SoundsManager : MonoBehaviour
	{
		#region Parameters
		public AudioClip[] buttonClick;
		public AudioClip[] popups;
		public AudioClip[] numberFind;
		public AudioClip[] levelCompletion;
		public AudioClip[] pageSwitch;
		public AudioClip[] task;
		#endregion
		
		private int _previousButtonClick = 99;
		private int _previousPopups = 99;
		private int _previousNumberFind = 99;
		private int _previousLevelCompletion = 99;
		private int _previousPageSwitch = 99;
		private int _previousTask = 99;
		
		void OnApplicationPause(bool paused)
		{
			if (!paused)
			{
				bool osMusicIsPlaying = false;

				#if PRIME31
				#if UNITY_IPHONE
				osMusicIsPlaying = MediaPlayerBinding.isiPodMusicPlaying();
				#endif
				#endif

				APIManager.Instance.Analytics.LogIPodMusicStatus(osMusicIsPlaying, false);
				if (osMusicIsPlaying)
					AudioManager.Instance.DisableMusic();
			}
		}
		
		public AudioClip GetAudioClip(SoundType type, bool noRepeat)
		{
			switch (type)
			{
				case SoundType.ButtonClick:
					return ChooseRandomSound(buttonClick, ref _previousButtonClick, noRepeat);
				case SoundType.PageSwitch:
					return ChooseRandomSound(pageSwitch, ref _previousPageSwitch, noRepeat);
				case SoundType.Popup:
					return ChooseRandomSound(popups, ref _previousPopups, noRepeat);
				case SoundType.NumberFind:
					return ChooseRandomSound(numberFind, ref _previousNumberFind, noRepeat);
				case SoundType.LevelCompletion:
					return ChooseRandomSound(levelCompletion, ref _previousLevelCompletion, noRepeat);
				case SoundType.Task:
					return ChooseRandomSound(task, ref _previousTask, noRepeat);
				default:
					return null;
			}
		}
		
		private AudioClip ChooseRandomSound(AudioClip[] arrayOfSounds, ref int previousIndex, bool noRepeat)
		{	
			if (arrayOfSounds.Length > 0)
			{
				int index = previousIndex;
				
				if (noRepeat && arrayOfSounds.Length > 1)
				{
					while (index == previousIndex)
						index = Random.Range(0, arrayOfSounds.Length);
				}
				else
					index = Random.Range(0, arrayOfSounds.Length);
				
				previousIndex = index;
					
				return arrayOfSounds[index];
			}
			else
				return null;
		}
		
		#region Singleton Stuff
		private static SoundsManager instance = null;
		
		static public SoundsManager Instance
		{
			get { return instance; }
		}
		
	    void Awake() 
		{
	        instance = this;
	    }
		
		void OnDestroy()
		{
			instance = null;
		}
		#endregion
	}
}
