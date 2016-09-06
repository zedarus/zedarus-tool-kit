using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IScoreWrapperInterface
	{
		#region Events
		event Action<List<ScoreData>> ScoreLoaded;
		#endregion
		
		#region Controls
		void Login();
		void UnlockAchievement(string achievementID);
		void SubmitScore(int score, string leaderboardID);
		void RestoreAchievement(string achievementID);
		void DisplayAchievementsList();
		void DisplayLeaderboardsList();
		void DisplayDefaultView();
		void RequestScore(string leaderboardID, ScoreController.TimeScope timeScope, bool friendsOnly, int start, int end);
		#endregion
		
		#region Queries
		bool HasNotifications { get; }
		bool LoggedIn { get; }
		#endregion
	}
}
