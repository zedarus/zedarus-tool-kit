using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Data.Player;
#if AUDIO_MASTER_AUDIO
using DarkTonic.MasterAudio;
#endif

namespace Zedarus.ToolKit
{
	public class AudioController
	{
		#region Properties
		private PlayerData _playerDataRef = null;
		#endregion

		#region Initialization
		public void Init(PlayerData playerDataRef)
		{
			_playerDataRef = playerDataRef;
			EventManager.AddListener(IDs.Events.AudioStateUpdated, OnAudioStateUpdate);
			OnAudioStateUpdate();
		}
		#endregion

		#region Controls
		public void PlayMusic(string playlist)
		{
			if (!string.IsNullOrEmpty(playlist))
			{
				#if AUDIO_MASTER_AUDIO
				if (MasterAudio.SafeInstance != null)
					MasterAudio.StartPlaylist(playlist);
				#endif
			}
		}

		public void ToggleMusic()
		{
			if (PlayerDataRef != null)
			{
				PlayerDataRef.AudioState.SetMusicState(!PlayerDataRef.AudioState.MusicEnabled);
			}
		}

		public void ToggleSound()
		{
			if (PlayerDataRef != null)
			{
				PlayerDataRef.AudioState.SetSoundState(!PlayerDataRef.AudioState.SoundEnabled);
			}
		}

		public void PlaySound(string id, float volume = 1f, float? pitch = default(float?))
		{
			if (!string.IsNullOrEmpty(id))
			{
				#if AUDIO_MASTER_AUDIO
				if (MasterAudio.SafeInstance != null)
					MasterAudio.PlaySound(id, volume, pitch);
				#endif
			}
		}

		private bool MusicMuted
		{
			get 
			{ 
				#if AUDIO_MASTER_AUDIO
				return MasterAudio.PlaylistsMuted; 
				#else
				return false;
				#endif
			}
		}

		private bool SoundMuted
		{
			get 
			{ 
				#if AUDIO_MASTER_AUDIO
				return MasterAudio.MixerMuted; 
				#else
				return false;
				#endif
			}
		}
		#endregion

		#region Getters
		private PlayerData PlayerDataRef
		{
			get { return _playerDataRef; }
		}
		#endregion

		#region Event Handlers
		private void OnAudioStateUpdate()
		{
			#if AUDIO_MASTER_AUDIO
			if (MasterAudio.SafeInstance != null && PlayerDataRef != null)
			{
				Debug.Log("OnAudioStateUpdate");
				if (PlayerDataRef.AudioState.MusicEnabled)
				{
					MasterAudio.UnmutePlaylist();
				}
				else
				{
					MasterAudio.MutePlaylist();
				}

				if (PlayerDataRef.AudioState.SoundEnabled)
				{
					MasterAudio.MixerMuted = false;
				}
				else
				{
					MasterAudio.MixerMuted = true;
				}
			}
			else
			{
				// TODO: cache changes
			}
			#endif
		}
		#endregion
	}
}
