using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Helpers;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class GenericScoreWrapper : APIWrapper<GenericScoreWrapper>, IScoreWrapperInterface 
	{
		#region Setup
		protected override void Setup() {}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
		
		#region Controls
		public void RequestAuthorisation()
		{
			ZedLogger.Log("RequestAuthorisation");	
		}
		
		public void Login()
		{
			ZedLogger.Log("Login");	
		}
		
		public void UnlockAchievement(int achievementID) 
		{
			ZedLogger.Log("UnlockAchievement");
		}
		
		public void RestoreAchievement(int achievementID) 
		{
			ZedLogger.Log("RestoreAchievement");
		}
		
		public void SubmitScore(int score, int leaderboardID) 
		{
			ZedLogger.Log("SubmitScore to leaderboard: " + leaderboardID);
		}
		
		public void DisplayAchievementsList() 
		{
			ZedLogger.Log("DisplayAchievementsList");
		}
		
		public void DisplayLeaderboardsList() 
		{
			ZedLogger.Log("DisplayLeaderboardsList");
		}
		
		public void DisplayDefaultView() 
		{
			DisplayAchievementsList();
		}
		#endregion
		
		#region Queries
		public bool HasNotifications 
		{ 
			get { return false; }
		}
		#endregion
	}
}
