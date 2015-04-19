using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Data.PlayerData;
using Zedarus.Traffico.Localisation;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
#if API_GAME_CENTER_P31
	public class GameCenterWrapper : APIWrapper<GameCenterWrapper>, IScoreWrapperInterface 
	{
		#region Setup
		protected override void Setup() 
		{
			//Login();
		}
		#endregion
		
		#region Controls
		public void RequestAuthorisation()
		{
			#if UNITY_IPHONE
			if (!PlayerDataManager.Instance.GameCenterLoginRequested)
			{
				PopupManager.Instance.ShowUseGameCenterPopup(OnGameCenterUseConfirm, null);
				PlayerDataManager.Instance.RequestGameCenterLogin();
			}
			#endif
		}
		
		public void Login()
		{
			#if UNITY_IPHONE
			if (!GameCenterBinding.isPlayerAuthenticated())
			{
				ZedLogger.Log("trying to authenticate local player");
				GameCenterBinding.authenticateLocalPlayer();
			}
			#endif
		}
		
		public void UnlockAchievement(int achievementID) 
		{
			#if UNITY_IPHONE
			if (!Enabled)
				return;
			
			string achievement = GetAchivementID(achievementID);
			if (achievement != null)
			{
				GameCenterBinding.reportAchievement(achievement, 100f);	
				GameCenterBinding.showCompletionBannerForAchievements();
			}
			#endif
		}
		
		public void RestoreAchievement(int achievementID)
		{
			#if UNITY_IPHONE
			if (!Enabled)
				return;
			
			string achievement = GetAchivementID(achievementID);
			if (achievement != null)
				GameCenterBinding.reportAchievement(achievement, 100f);
			#endif
		}
		
		public void SubmitScore(int score, int leaderboardID) 
		{
			#if UNITY_IPHONE
			if (!Enabled)
				return;
			
			string leaderboard = GetLeaderboardID(leaderboardID);
			if (leaderboard != null)
				GameCenterBinding.reportScore(score, leaderboard);
			#endif
		}
		
		public void DisplayAchievementsList() 
		{
			#if UNITY_IPHONE
			if (Enabled)
				GameCenterBinding.showAchievements();
			else
				Login();
			#endif
		}
		
		public void DisplayLeaderboardsList() 
		{
			#if UNITY_IPHONE
			if (Enabled)
				GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.Week);
			else
				Login();
			#endif
		}
		
		public void DisplayDefaultView() 
		{
			#if UNITY_IPHONE
			if (Enabled)
				GameCenterBinding.showGameCenterViewController(GameCenterViewControllerState.Default);
			else
				Login();
			#endif
		}
		#endregion
		
		#region Queries
		public bool HasNotifications 
		{ 
			get { return true; }
		}
		#endregion
		
		#region Helpers
		private bool Enabled
		{
			get 
			{
				#if UNITY_IPHONE
				return GameCenterBinding.isPlayerAuthenticated(); 
				#else
				return false;
				#endif
			}
		}
		
		private string GetAchivementID(int achievementID)
		{
			AchievementData achievement = GameDataManager.Instance.GetAchievementWithID(achievementID);
			if (achievement != null)
				return achievement.GameCenterIosID;
			return null;
		}
		
		private string GetLeaderboardID(int leaderboardID)
		{
			LeaderboardData leaderboard = GameDataManager.Instance.GetLeaderboardWithID(leaderboardID);	
			if (leaderboard != null)
				return leaderboard.GameCenterIosID;
			return null;
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			#if UNITY_IPHONE
			// Listens to all the GameCenter events.  All event listeners MUST be removed before this object is disposed!
			// Player events
			GameCenterManager.loadPlayerDataFailedEvent += loadPlayerDataFailed;
			GameCenterManager.playerDataLoadedEvent += playerDataLoaded;
			GameCenterManager.playerAuthenticatedEvent += playerAuthenticated;
			GameCenterManager.playerFailedToAuthenticateEvent += playerFailedToAuthenticate;
			GameCenterManager.playerLoggedOutEvent += playerLoggedOut;
			GameCenterManager.profilePhotoLoadedEvent += profilePhotoLoaded;
			GameCenterManager.profilePhotoFailedEvent += profilePhotoFailed;
			
			// Leaderboards and scores
			GameCenterManager.loadCategoryTitlesFailedEvent += loadCategoryTitlesFailed;
			GameCenterManager.categoriesLoadedEvent += categoriesLoaded;
			GameCenterManager.reportScoreFailedEvent += reportScoreFailed;
			GameCenterManager.reportScoreFinishedEvent += reportScoreFinished;
			GameCenterManager.retrieveScoresFailedEvent += retrieveScoresFailed;
			GameCenterManager.scoresLoadedEvent += scoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent += retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoadedEvent += scoresForPlayerIdLoaded;
			
			// Achievements
			GameCenterManager.reportAchievementFailedEvent += reportAchievementFailed;
			GameCenterManager.reportAchievementFinishedEvent += reportAchievementFinished;
			GameCenterManager.loadAchievementsFailedEvent += loadAchievementsFailed;
			GameCenterManager.achievementsLoadedEvent += achievementsLoaded;
			GameCenterManager.resetAchievementsFailedEvent += resetAchievementsFailed;
			GameCenterManager.resetAchievementsFinishedEvent += resetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailedEvent += retrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoadedEvent += achievementMetadataLoaded;
			
			// Challenges
			GameCenterManager.localPlayerDidSelectChallengeEvent += localPlayerDidSelectChallengeEvent;
			GameCenterManager.localPlayerDidCompleteChallengeEvent += localPlayerDidCompleteChallengeEvent;
			GameCenterManager.remotePlayerDidCompleteChallengeEvent += remotePlayerDidCompleteChallengeEvent;
			GameCenterManager.challengesLoadedEvent += challengesLoadedEvent;
			GameCenterManager.challengesFailedToLoadEvent += challengesFailedToLoadEvent;
			GameCenterManager.challengeIssuedSuccessfullyEvent += challengeIssuedSuccessfullyEvent;
			GameCenterManager.challengeNotIssuedEvent += challengeNotIssuedEvent;
			#endif
		}
		
		protected override void RemoveEventListeners() 
		{
			#if UNITY_IPHONE
			// Remove all the event handlers
			// Player events
			GameCenterManager.loadPlayerDataFailedEvent -= loadPlayerDataFailed;
			GameCenterManager.playerDataLoadedEvent -= playerDataLoaded;
			GameCenterManager.playerAuthenticatedEvent -= playerAuthenticated;
			GameCenterManager.playerLoggedOutEvent -= playerLoggedOut;
			GameCenterManager.profilePhotoLoadedEvent -= profilePhotoLoaded;
			GameCenterManager.profilePhotoFailedEvent -= profilePhotoFailed;
			
			// Leaderboards and scores
			GameCenterManager.loadCategoryTitlesFailedEvent -= loadCategoryTitlesFailed;
			GameCenterManager.categoriesLoadedEvent -= categoriesLoaded;
			GameCenterManager.reportScoreFailedEvent -= reportScoreFailed;
			GameCenterManager.reportScoreFinishedEvent -= reportScoreFinished;
			GameCenterManager.retrieveScoresFailedEvent -= retrieveScoresFailed;
			GameCenterManager.scoresLoadedEvent -= scoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent -= retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoadedEvent -= scoresForPlayerIdLoaded;
			
			// Achievements
			GameCenterManager.reportAchievementFailedEvent -= reportAchievementFailed;
			GameCenterManager.reportAchievementFinishedEvent -= reportAchievementFinished;
			GameCenterManager.loadAchievementsFailedEvent -= loadAchievementsFailed;
			GameCenterManager.achievementsLoadedEvent -= achievementsLoaded;
			GameCenterManager.resetAchievementsFailedEvent -= resetAchievementsFailed;
			GameCenterManager.resetAchievementsFinishedEvent -= resetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailedEvent -= retrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoadedEvent -= achievementMetadataLoaded;
			
			// Challenges
			GameCenterManager.localPlayerDidSelectChallengeEvent -= localPlayerDidSelectChallengeEvent;
			GameCenterManager.localPlayerDidCompleteChallengeEvent -= localPlayerDidCompleteChallengeEvent;
			GameCenterManager.remotePlayerDidCompleteChallengeEvent -= remotePlayerDidCompleteChallengeEvent;
			GameCenterManager.challengesLoadedEvent -= challengesLoadedEvent;
			GameCenterManager.challengesFailedToLoadEvent -= challengesFailedToLoadEvent;
			GameCenterManager.challengeIssuedSuccessfullyEvent -= challengeIssuedSuccessfullyEvent;
			GameCenterManager.challengeNotIssuedEvent -= challengeNotIssuedEvent;
			#endif
		}
		#endregion
		
		#region Event Handlers
		private void OnGameCenterUseConfirm()
		{
			Login();
		}
		#endregion

		#if UNITY_IPHONE
		#region Player Events
		private void playerAuthenticated()
		{
			ZedLogger.Log("playerAuthenticated");
			GameCenterBinding.retrieveAchievementMetadata();
			SendInitializedEvent();
		}
		
		private void playerFailedToAuthenticate(string error)
		{
			ZedLogger.Log("playerFailedToAuthenticate: " + error);
		}
		
		private void playerLoggedOut()
		{
			ZedLogger.Log("playerLoggedOut");
		}

		private void playerDataLoaded(List<GameCenterPlayer> players)
		{
			ZedLogger.Log("playerDataLoaded");
			foreach(GameCenterPlayer p in players)
				ZedLogger.Log(p);
		}
		
		private void loadPlayerDataFailed(string error)
		{
			ZedLogger.Log("loadPlayerDataFailed: " + error);
		}
		
		private void profilePhotoLoaded(string path)
		{
			ZedLogger.Log("profilePhotoLoaded: " + path);
		}
		
		private void profilePhotoFailed(string error)
		{
			ZedLogger.Log("profilePhotoFailed: " + error);
		}
		#endregion
		
		#region Leaderboard Events
		private void categoriesLoaded(List<GameCenterLeaderboard> leaderboards)
		{
			ZedLogger.Log("categoriesLoaded");
			foreach(GameCenterLeaderboard l in leaderboards)
				ZedLogger.Log(l);
		}
		
		private void loadCategoryTitlesFailed(string error)
		{
			ZedLogger.Log("loadCategoryTitlesFailed: " + error);
		}
		#endregion
		
		#region Score Events
		private void scoresLoaded(GameCenterRetrieveScoresResult scores)
		{
			ZedLogger.Log("scoresLoaded");
			foreach(GameCenterScore s in scores.scores)
				ZedLogger.Log(s);
		}
		
		private void retrieveScoresFailed(string error)
		{
			ZedLogger.Log("retrieveScoresFailed: " + error);
		}
		
		private void retrieveScoresForPlayerIdFailed(string error)
		{
			ZedLogger.Log("retrieveScoresForPlayerIdFailed: " + error);
		}
		
		private void scoresForPlayerIdLoaded(GameCenterRetrieveScoresResult scores)
		{
			ZedLogger.Log("scoresForPlayerIdLoaded");
			foreach(GameCenterScore s in scores.scores)
				ZedLogger.Log(s);
		}
		
		private void reportScoreFinished(string category)
		{
			ZedLogger.Log("reportScoreFinished for category: " + category);
		}
		
		private void reportScoreFailed(string error)
		{
			ZedLogger.Log("reportScoreFailed: " + error);
		}
		#endregion
		
		#region Achievement Events
		private void achievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementMetadata)
		{
			ZedLogger.Log("achievementMetadatLoaded");
			foreach(GameCenterAchievementMetadata s in achievementMetadata)
				ZedLogger.Log(s);
		}
		
		private void retrieveAchievementMetadataFailed(string error)
		{
			ZedLogger.Log("retrieveAchievementMetadataFailed: " + error);
		}
		
		private void resetAchievementsFinished()
		{
			ZedLogger.Log("resetAchievmenetsFinished");
		}
		
		private void resetAchievementsFailed(string error)
		{
			ZedLogger.Log("resetAchievementsFailed: " + error);
		}
		
		private void achievementsLoaded(List<GameCenterAchievement> achievements)
		{
			ZedLogger.Log("achievementsLoaded");
			foreach(GameCenterAchievement s in achievements)
				ZedLogger.Log(s);
		}
		
		private void loadAchievementsFailed(string error)
		{
			ZedLogger.Log("loadAchievementsFailed: " + error);
		}
		
		private void reportAchievementFinished(string identifier)
		{
			ZedLogger.Log("reportAchievementFinished: " + identifier);
		}
		
		private void reportAchievementFailed(string error)
		{
			ZedLogger.Log("reportAchievementFailed: " + error);
		}
		#endregion
		
		#region Challenges Events
		private void localPlayerDidSelectChallengeEvent(GameCenterChallenge challenge)
		{
			ZedLogger.Log("localPlayerDidSelectChallengeEvent : " + challenge);
		}
		
		private void localPlayerDidCompleteChallengeEvent(GameCenterChallenge challenge)
		{
			ZedLogger.Log("localPlayerDidCompleteChallengeEvent : " + challenge);
		}
		
		private void remotePlayerDidCompleteChallengeEvent(GameCenterChallenge challenge)
		{
			ZedLogger.Log("remotePlayerDidCompleteChallengeEvent : " + challenge);
		}
		
		private void challengesLoadedEvent(List<GameCenterChallenge> list)
		{
			ZedLogger.Log("challengesLoadedEvent");
			Prime31.Utils.logObject(list);
		}
		
		private void challengesFailedToLoadEvent(string error)
		{
			ZedLogger.Log("challengesFailedToLoadEvent: " + error);
		}
		
		private void challengeIssuedSuccessfullyEvent(List<object> playerIds)
		{
			ZedLogger.Log("challengeIssuedSuccessfullyEvent");
			Prime31.Utils.logObject(playerIds);
		}
		
		private void challengeNotIssuedEvent()
		{
			ZedLogger.Log("challengeNotIssuedEvent");
		}
		#endregion
		#endif
	}
#endif
}
