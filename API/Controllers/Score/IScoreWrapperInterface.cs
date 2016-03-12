using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IScoreWrapperInterface
	{
		#region Events
		#endregion
		
		#region Controls
		void RequestAuthorisation();
		void Login();
		void UnlockAchievement(int achievementID);
		void SubmitScore(int score, int leaderboardID);
		void RestoreAchievement(int achievementID);
		void DisplayAchievementsList();
		void DisplayLeaderboardsList();
		void DisplayDefaultView();
		#endregion
		
		#region Queries
		bool HasNotifications { get; }
		bool LoggedIn { get; }
		#endregion
	}
}
