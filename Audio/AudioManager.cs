using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Zedarus.ToolKit.API;

namespace Zedarus.ToolKit.Audio
{
	/// <summary>
/// Audio manager helper that is used to manage music and sounds playing.
/// </summary>
	public class AudioManager 
	{
		#region Parameters
		private AudioListener _audioListener;
		private AudioPlayer _audioPlayer = null;
		
		private string _currentMusic = null;
		
		private bool _musicEnabled = true;
		private bool _soundEnabled = true;
		#endregion
		
		#region Events
		public event Action<bool> soundStateUpdate;
		public event Action<bool> musicStateUpdate;
		#endregion
		
		public AudioManager()
		{
			bool osMusicIsPlaying = false;

			#if PRIME31
			#if UNITY_IPHONE
			osMusicIsPlaying = MediaPlayerBinding.isiPodMusicPlaying();
			#endif
			#endif

			APIManager.Instance.Analytics.LogIPodMusicStatus(osMusicIsPlaying, true);
			if (osMusicIsPlaying)
				DisableMusic();
		}
		
		#region Music Controls
		/// <summary>
		/// Plaies the music with specific audio file name (<paramref name="music"/>). If this audio file is already being played nothing will happen. 
		/// </summary>
		/// <param name='music'>
		/// Music filename.
		/// </param>
		public void PlayMusic(string music) 
		{
			if (_currentMusic == music)
				return;
			
			AudioSource source = GetAudioSource();
			if (source != null) 
			{
				string musicPath = "Audio/Music" + "/" + music;
				AudioClip sound = Resources.Load(musicPath, typeof(AudioClip)) as AudioClip;
				if (sound != null) 
				{
					source.clip = sound;
					source.loop = true;
					
					if (IsMusicEnabled())
						source.Play();
				}
			}
			
			_currentMusic = music;
		}
		
		/// <summary>
		/// Determines whether music is playing.
		/// </summary>
		/// <returns>
		/// <c>true</c> if music is playing; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMusicPlaying() 
		{
			return _currentMusic != null;
		}
		
		/// <summary>
		/// Toggles the music on and off.
		/// </summary>
		/// <returns>
		/// <c>true</c> if music if turned on; otherwise, <c>false</c>.
		/// </returns>
		public bool ToggleMusic() 
		{
			if (_musicEnabled)
				DisableMusic();
			else
				EnableMusic();
			
			return _musicEnabled;
		}
		
		/// <summary>
		/// Enables the music.
		/// </summary>
		public void EnableMusic() 
		{
			if (_musicEnabled)
				return;
			
			AudioSource source = GetAudioSource();
			if (source) 
				source.Play();
		
			_musicEnabled = true;
			SendMusicUpdateEvent(_musicEnabled);
			APIManager.Instance.Analytics.EnableMusic();
		}
		
		/// <summary>
		/// Disables the music.
		/// </summary>
		public void DisableMusic() 
		{
			if (!_musicEnabled)
				return;
			
			AudioSource source = GetAudioSource();
			if (source) 
				source.Pause();
			
			_musicEnabled = false;
			SendMusicUpdateEvent(_musicEnabled);
			APIManager.Instance.Analytics.DisableMusic();
		}
		
		/// <summary>
		/// Determines whether music is enabled.
		/// </summary>
		/// <returns>
		/// <c>true</c> if music is enabled; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMusicEnabled() 
		{
			return _musicEnabled;
		}
		
		/// <summary>
		/// Sends the music status update event.
		/// </summary>
		/// <param name='state'>
		/// Current music state: either <c>true</c> (on) or <c>false</c> (off).
		/// </param>
		private void SendMusicUpdateEvent(bool state) 
		{
			if (musicStateUpdate != null)
				musicStateUpdate(state);
		}
		#endregion
		
		#region Sound Controls
		/// <summary>
		/// Plays the <paramref name="sound"/> <see cref="AudioClip"/>.
		/// </summary>
		/// <param name='sound'>
		/// <see cref="AudioClip"/>.
		/// </param>
		public void PlaySound(AudioClip sound) 
		{
			if (!_soundEnabled || sound == null)
				return;
			
			CreateAudioPlayer();
			AudioSource.PlayClipAtPoint(sound, Vector3.zero);
		}
		
		/// <summary>
		/// Toggles the sound on and off.
		/// </summary>
		/// <returns>
		/// <c>true</c> if sound is turned on; otherwise, <c>false</c>.
		/// </returns>
		public bool ToggleSound() 
		{
			if (_soundEnabled)
				DisableSound();
			else
				EnableSound();
			
			return _soundEnabled;
		}
		
		/// <summary>
		/// Enables the sound.
		/// </summary>
		public void EnableSound() 
		{
			if (_soundEnabled)
				return;
		
			_soundEnabled = true;
			SendSoundUpdateEvent(_soundEnabled);
			APIManager.Instance.Analytics.EnableSounds();
		}
		
		/// <summary>
		/// Disables the sound.
		/// </summary>
		public void DisableSound() 
		{
			if (!_soundEnabled)
				return;
		
			_soundEnabled = false;
			SendSoundUpdateEvent(_soundEnabled);
			APIManager.Instance.Analytics.DisableSounds();
		}
		
		/// <summary>
		/// Determines whether sound is enabled.
		/// </summary>
		/// <returns>
		/// <c>true</c> if sound is enabled; otherwise, <c>false</c>.
		/// </returns>
		public bool IsSoundEnabled() 
		{
			return _soundEnabled;
		}
		
		/// <summary>
		/// Sends the sound status update event.
		/// </summary>
		/// <param name='state'>
		/// Current sound state: either <c>true</c> (on) or <c>false</c> (off).
		/// </param>
		private void SendSoundUpdateEvent(bool state) 
		{
			if (soundStateUpdate != null)
				soundStateUpdate(state);
		}
		#endregion
		
		#region Initialization
		/// <summary>
		/// Creates the audio player game object and places it on scene.
		/// </summary>
		private void CreateAudioPlayer() 
		{
			if (_audioPlayer == null) 
			{
				GameObject playerGameObject = GameObject.Find("AudioPlayer");
				if (playerGameObject == null) 
				{
					playerGameObject = new GameObject();
					playerGameObject.name = "AudioPlayer";
				}
				_audioPlayer = playerGameObject.GetComponent<AudioPlayer>();
				if (_audioPlayer == null)
					_audioPlayer = playerGameObject.AddComponent<AudioPlayer>();
			}
			
			if (_audioListener == null) 
			{
				_audioListener = _audioPlayer.gameObject.GetComponent<AudioListener>();
				if (_audioListener == null)
					_audioListener = _audioPlayer.gameObject.AddComponent<AudioListener>();
			}
		}
		
		/// <summary>
		/// Gets the audio source that is used to play music of sounds.
		/// </summary>
		/// <returns>
		/// The audio source.
		/// </returns>
		private AudioSource GetAudioSource() 
		{
			CreateAudioPlayer();
			
			AudioSource source = _audioPlayer.gameObject.GetComponent<AudioSource>();
			if (source == null) 
				source = _audioPlayer.gameObject.AddComponent<AudioSource>();
			
			return source;
		}
		
		public void SetSoundAndMusicState(bool soundEnabled, bool musicEnabled)
		{
			if (soundEnabled)
				EnableSound();
			else
				DisableSound();
			
			if (musicEnabled)
				EnableMusic();
			else
				DisableMusic();
		}
		#endregion
		
		#region Singleton
		private static AudioManager instance;
		
		public static AudioManager Instance 
		{
			get 
			{
				if (instance == null)
					instance = new AudioManager();
				return instance;
			}
		}
		#endregion
	}
}
