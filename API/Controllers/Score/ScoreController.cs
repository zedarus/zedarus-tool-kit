using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class ScoreController : APIController 
	{
		#region Events
		public event Action<string> DisplayAchievementUnlockedNotification;
		#endregion
		
		#region Initialization
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override void InitWrappers() 
		{
			base.InitWrappers();
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Score.GameCenter:
					return GameCenterWrapper.Instance;
				case APIs.Score.GooglePlayPlayServices:
					return GooglePlayGameServicesWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void Login()
		{
			if (Wrapper != null) Wrapper.Login();
		}
		
		public void UnlockAchievement(string achievementID) 
		{
			if (Wrapper != null)
			{
				Wrapper.UnlockAchievement(achievementID);
				
				if (!Wrapper.HasNotifications && DisplayAchievementUnlockedNotification != null)
					DisplayAchievementUnlockedNotification(achievementID);
			}
		}
		
		public void RestoreAchievement(string achievementID) 
		{
			if (Wrapper != null) Wrapper.RestoreAchievement(achievementID);
		}
		
		public void SubmitScore(int score, string leaderboardID) 
		{
			if (Wrapper != null) Wrapper.SubmitScore(score, leaderboardID);
		}
		
		public void DisplayAchievementsList() 
		{
			if (Wrapper != null) Wrapper.DisplayAchievementsList();
		}
		
		public void DisplayLeaderboardsList() 
		{
			if (Wrapper != null) Wrapper.DisplayLeaderboardsList();
		}
		
		public void DisplayDefaultView() 
		{
			if (Wrapper != null) Wrapper.DisplayDefaultView();
		}
		#endregion
		
		#region Getters
		protected IScoreWrapperInterface Wrapper
		{
			get { return (IScoreWrapperInterface)CurrentWrapperBase; }
		}

		public bool LoggedIn
		{
			get 
			{ 
				if (Wrapper != null)
					return Wrapper.LoggedIn;
				else
					return false;
			}
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			base.CreateEventListeners();
			EventManager.AddListener<string>(IDs.Events.AchievementUnlocked, OnAchievementUnlocked);
			EventManager.AddListener<string>(IDs.Events.AchievementRestored, OnAchievementRestored);
		}

		protected override void RemoveEventListeners()
		{
			base.RemoveEventListeners();
			EventManager.RemoveListener<string>(IDs.Events.AchievementUnlocked, OnAchievementUnlocked);
			EventManager.RemoveListener<string>(IDs.Events.AchievementRestored, OnAchievementRestored);
		}
		#endregion

		#region Event Handlers
		private void OnAchievementUnlocked(string achievementID)
		{
			UnlockAchievement(achievementID);
		}

		private void OnAchievementRestored(string achievementID)
		{
			RestoreAchievement(achievementID);
		}
		#endregion
		
		protected override void CompleteInitialization()
		{
			base.CompleteInitialization();
			
			// TODO: PlayerDataManager.Instance.UploadScore();
			// TODO: PlayerDataManager.Instance.UploadAchievements();
		}
	}
}
