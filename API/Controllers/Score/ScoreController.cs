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
		public event Action<List<ScoreData>> ScoreLoaded;
		#endregion

		#region Settings
		public enum TimeScope
		{
			Today = 1,
			Week = 2,
			AllTime = 3,
		}	
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

		public void RequestScore(string leaderboardID, ScoreController.TimeScope timeScope, bool friendsOnly, int start, int end)
		{
			if (Wrapper != null) Wrapper.RequestScore(leaderboardID, timeScope, friendsOnly, start, end);
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

			foreach (IScoreWrapperInterface wrapper in Wrappers)
			{
				wrapper.ScoreLoaded += OnScoreLoaded;
			}

			EventManager.AddListener<string>(IDs.Events.AchievementUnlocked, OnAchievementUnlocked);
			EventManager.AddListener<string>(IDs.Events.AchievementRestored, OnAchievementRestored);
		}

		protected override void RemoveEventListeners()
		{
			base.RemoveEventListeners();

			foreach (IScoreWrapperInterface wrapper in Wrappers)
			{
				wrapper.ScoreLoaded -= OnScoreLoaded;
			}

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

		private void OnScoreLoaded(List<ScoreData> score)
		{
			if (ScoreLoaded != null)
			{
				ScoreLoaded(score);
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

	public class ScoreData
	{
		public string category;
		public string formattedValue;
		public long value;
		public UInt64 context;
		public double rawDate;
		public DateTime date
		{
			get
			{
				var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
				return intermediate.AddSeconds( rawDate );
			}
		}
		public string playerId;
		public int rank;
		public bool isFriend;
		public string alias;
		public int maxRange; // this is only properly set when retrieving all scores without limiting by playerId

		public ScoreData()
		{}

		public override string ToString()
		{
			return string.Format( "<Score> category: {0}, formattedValue: {1}, date: {2}, rank: {3}, alias: {4}, maxRange: {5}, value: {6}, context: {7}",
				category, formattedValue, date, rank, alias, maxRange, value, context );
		}

	}
}
