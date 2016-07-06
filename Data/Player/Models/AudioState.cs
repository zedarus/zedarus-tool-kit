using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class AudioState : IPlayerDataModel
	{
		#region Properties
		[SerializeField]
		private bool _soundEnabled;
		[SerializeField]
		private bool _musicEnabled;
		#endregion

		#region Init
		public AudioState() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_soundEnabled = true;
			_musicEnabled = true;
		}
		#endregion

		#region Controls
		public void SetSoundState(bool enabled)
		{
			_soundEnabled = enabled;
			SendStateUpdateEvent();
		}

		public void SetMusicState(bool enabled)
		{
			_musicEnabled = enabled;
			SendStateUpdateEvent();
		}
		#endregion

		#region Getters
		public bool SoundEnabled
		{
			get { return _soundEnabled; }
		}

		public bool MusicEnabled
		{
			get { return _musicEnabled; }
		}
		#endregion

		#region Helpers
		private void SendStateUpdateEvent()
		{
			EventManager.SendEvent(Zedarus.ToolKit.Settings.IDs.Events.AudioStateUpdated);
		}
		#endregion

		#region Controls
		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{ 
			AudioState other = (AudioState)data;
			if (other != null)
			{
				_soundEnabled = other._soundEnabled;
				_musicEnabled = other._musicEnabled;
				SendStateUpdateEvent();
				return true;
			}
			else
			{
				return true; 
			}
		}
		#endregion
	}
}
