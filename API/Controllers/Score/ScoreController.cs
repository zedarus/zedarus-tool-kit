using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class ScoreController : APIController 
	{
		#region Events
		public event Action<int> DisplayAchievementUnlockedNotification;
		#endregion
		
		#region Initialization
		public ScoreController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override void InitWrappers() 
		{
			base.InitWrappers();
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				#if API_GAME_CENTER_P31
				case APIs.AppleGameCenter:
					return GameCenterWrapper.Instance;
				#endif
				#if API_GAME_SERVICES_P31
				case APIs.GoogleGameServices:
					return GooglePlayGameServicesWrapper.Instance;
				#endif
				case APIs.Generic:
					return GenericScoreWrapper.Instance;
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
		#endregion
		
		protected override void CompleteInitialization()
		{
			base.CompleteInitialization();

			EventManager.SendEvent(APIEvents.UploadScore);
			EventManager.SendEvent(APIEvents.UploadAchievements);
		}
	}
}
