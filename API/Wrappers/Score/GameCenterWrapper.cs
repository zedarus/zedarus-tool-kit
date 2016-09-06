using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Localisation;
using Zedarus.ToolKit;
#if UNITY_IPHONE && API_SCORE_GAMECENTER
using Prime31;
#endif

namespace Zedarus.ToolKit.API
{
	public class GameCenterWrapper : APIWrapper<GameCenterWrapper>, IScoreWrapperInterface 
	{
		#region Events
		public event Action<List<ScoreData>> ScoreLoaded;
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			Login();
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion
		
		#region Controls
		public void Login()
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (!GameCenterBinding.isPlayerAuthenticated())
			{
				ZedLogger.Log("trying to authenticate local player");
				GameCenterBinding.authenticateLocalPlayer(false);
				GameCenterBinding.showCompletionBannerForAchievements();
			}
			#endif
		}
		
		public void UnlockAchievement(string achievementID) 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (!Enabled)
				return;
			
			if (achievementID != null)
			{
				GameCenterBinding.reportAchievement(achievementID, 100f);	
				GameCenterBinding.showCompletionBannerForAchievements();
			}
			#endif
		}

		public void RestoreAchievement(string achievementID)
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (!Enabled)
				return;
			
			if (achievementID != null)
			{
				GameCenterBinding.reportAchievement(achievementID, 100f);
			}
			#endif
		}
		
		public void SubmitScore(int score, string leaderboardID) 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (!Enabled)
				return;
			
			if (leaderboardID != null)
			{
				GameCenterBinding.reportScore(score, leaderboardID);
			}
			#endif
		}
		
		public void DisplayAchievementsList() 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (Enabled)
				GameCenterBinding.showAchievements();
			else
				Login();
			#endif
		}
		
		public void DisplayLeaderboardsList() 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (Enabled)
				GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.Week);
			else
				Login();
			#endif
		}
		
		public void DisplayDefaultView() 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (Enabled)
				GameCenterBinding.showGameCenterViewController(GameCenterViewControllerState.Default);
			else
				Login();
			#endif
		}

		public void RequestScore(string leaderboardID, ScoreController.TimeScope timeScope, bool friendsOnly, int start, int end)
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
			if (Enabled)
			{
				GameCenterLeaderboardTimeScope convertedScope = GameCenterLeaderboardTimeScope.AllTime;

				switch (timeScope)
				{
					case ScoreController.TimeScope.Today:
						convertedScope = GameCenterLeaderboardTimeScope.Today;
						break;
					case ScoreController.TimeScope.Week:
						convertedScope = GameCenterLeaderboardTimeScope.Week;
						break;
					case ScoreController.TimeScope.AllTime:
					default:
						convertedScope = GameCenterLeaderboardTimeScope.AllTime;
						break;
				}

				GameCenterBinding.retrieveScores(friendsOnly, convertedScope, start, end, leaderboardID);
			}
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

		public bool LoggedIn
		{
			get { return Enabled; }
		}
		#endregion
		
		#region Helpers
		private void SendScoreLoadedEvent(GameCenterRetrieveScoresResult scores)
		{
			if (ScoreLoaded != null)
			{
				List<ScoreData> scoresData = new List<ScoreData>();
				ScoreData scoreData = null;

				foreach (GameCenterScore score in scores.scores)
				{
					scoreData = new ScoreData();
					scoreData.category = score.category;
					scoreData.formattedValue = score.formattedValue;
					scoreData.value = score.value;
					scoreData.context = score.context;
					scoreData.rawDate = score.rawDate;
					scoreData.playerId = score.playerId;
					scoreData.rank = score.rank;
					scoreData.isFriend = score.isFriend;
					scoreData.alias = score.alias;
					scoreData.maxRange = score.maxRange;
					scoresData.Add(scoreData);
				}

				ScoreLoaded(scoresData);
				scoresData = null;
			}
		}

		private bool Enabled
		{
			get 
			{
				#if UNITY_IPHONE && API_SCORE_GAMECENTER
				return GameCenterBinding.isPlayerAuthenticated(); 
				#else
				return false;
				#endif
			}
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
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
			GameCenterManager.retrieveScoresForPlayerIdsFailedEvent += retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdsLoadedEvent += scoresForPlayerIdLoaded;
			
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
			#if UNITY_IPHONE && API_SCORE_GAMECENTER
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
			GameCenterManager.retrieveScoresForPlayerIdsFailedEvent -= retrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdsLoadedEvent -= scoresForPlayerIdLoaded;
			
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

		#if UNITY_IPHONE && API_SCORE_GAMECENTER
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

			SendScoreLoadedEvent(scores);
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

			SendScoreLoadedEvent(scores);
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
}
