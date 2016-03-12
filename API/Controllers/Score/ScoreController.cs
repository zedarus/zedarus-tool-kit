using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public class ScoreController : APIController 
	{
		#region Events
		public event Action<int> DisplayAchievementUnlockedNotification;
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
		public void RequestAuthorisation()
		{
			if (Wrapper != null) Wrapper.RequestAuthorisation();	
		}
		
		public void Login()
		{
			if (Wrapper != null) Wrapper.Login();
		}
		
		public void UnlockAchievement(int achievementID) 
		{
			if (Wrapper != null)
			{
				Wrapper.UnlockAchievement(achievementID);
				
				if (!Wrapper.HasNotifications && DisplayAchievementUnlockedNotification != null)
					DisplayAchievementUnlockedNotification(achievementID);
			}
		}
		
		public void RestoreAchievement(int achievementID) 
		{
			if (Wrapper != null) Wrapper.RestoreAchievement(achievementID);
		}
		
		public void SubmitTotalScore(int score)
		{
			throw new NotImplementedException();
		}
		
		public void SubmitScore(int score, int leaderboardID) 
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
		
		protected override void CompleteInitialization()
		{
			base.CompleteInitialization();
			
			// TODO: PlayerDataManager.Instance.UploadScore();
			// TODO: PlayerDataManager.Instance.UploadAchievements();
		}
	}
}
