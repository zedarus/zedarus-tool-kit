using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Localisation;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public class GooglePlayGameServicesWrapper : APIWrapper<GooglePlayGameServicesWrapper>, IScoreWrapperInterface
	{
		#region Parameters
		private int _defaultView = 0;
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings)
		{
			#if UNITY_ANDROID
			if (!PlayGameServices.isSignedIn() && !PlayerDataManager.Instance.GameCenterLoginRequested)
			{
				ZedLogger.Log("trying to authenticate local player (silently)");
				PlayGameServices.attemptSilentAuthentication();
			}
			#endif
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion
		
		#region Controls
		public void Login()
		{
			#if UNITY_ANDROID
			if (!PlayGameServices.isSignedIn())
			{
				ZedLogger.Log("trying to authenticate local player");
				PlayGameServices.authenticate();
			}
			#endif
		}
		
		public void UnlockAchievement(string achievementID)
		{
			#if UNITY_ANDROID
			if (!Enabled)
				return;
			
			string achievement = GetAchivementID(achievementID);
			if (achievement != null)
				PlayGameServices.unlockAchievement(achievement, true);
			#endif
		}
		
		public void RestoreAchievement(string achievementID)
		{
			#if UNITY_ANDROID
			if (!Enabled)
				return;
			
			string achievement = GetAchivementID(achievementID);
			if (achievement != null)
				PlayGameServices.unlockAchievement(achievement, false);
			#endif
		}
		
		public void SubmitScore(int score, string leaderboardID)
		{
			#if UNITY_ANDROID
			if (!Enabled)
				return;
			
			string leaderboard = GetLeaderboardID(leaderboardID);
			if (leaderboard != null)
				PlayGameServices.submitScore(leaderboard, (long)score);
			#endif
		}
		
		public void DisplayAchievementsList()
		{
			#if UNITY_ANDROID
			if (Enabled)
				PlayGameServices.showAchievements();
			else
				Login();
			#endif
		}
		
		public void DisplayLeaderboardsList()
		{
			#if UNITY_ANDROID
			if (Enabled)
				PlayGameServices.showLeaderboards();
			else
				Login();
			#endif
		}
		
		public void DisplayDefaultView()
		{
			if (_defaultView == 0)
				DisplayLeaderboardsList();
			else
				DisplayAchievementsList();

			_defaultView++;
			if (_defaultView > 1)
				_defaultView = 0;
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
		private bool Enabled
		{
			get
			{
				#if UNITY_ANDROID
				return PlayGameServices.isSignedIn(); 
				#else
				return false;
				#endif
			}
		}
		
		private string GetAchivementID(int achievementID)
		{
			/*AchievementData achievement = GameDataManager.Instance.GetAchievementWithID(achievementID);
			if (achievement != null)
				return achievement.GooglePlayGameServiceID;*/
			return null;
		}
		
		private string GetLeaderboardID(int leaderboardID)
		{
			/*LeaderboardData leaderboard = GameDataManager.Instance.GetLeaderboardWithID(leaderboardID);	
			if (leaderboard != null)
				return leaderboard.GooglePlayGameServiceID;*/
			return null;
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners()
		{
			#if UNITY_ANDROID
			GPGManager.authenticationSucceededEvent += authenticationSucceededEvent;
			GPGManager.authenticationFailedEvent += authenticationFailedEvent;
			GPGManager.licenseCheckFailedEvent += licenseCheckFailedEvent;
			GPGManager.profileImageLoadedAtPathEvent += profileImageLoadedAtPathEvent;
			GPGManager.finishedSharingEvent += finishedSharingEvent;
			GPGManager.userSignedOutEvent += userSignedOutEvent;
			
			GPGManager.reloadDataForKeyFailedEvent += reloadDataForKeyFailedEvent;
			GPGManager.reloadDataForKeySucceededEvent += reloadDataForKeySucceededEvent;
			/*GPGManager.loadCloudDataForKeyFailedEvent += loadCloudDataForKeyFailedEvent;
			GPGManager.loadCloudDataForKeySucceededEvent += loadCloudDataForKeySucceededEvent;
			GPGManager.updateCloudDataForKeyFailedEvent += updateCloudDataForKeyFailedEvent;
			GPGManager.updateCloudDataForKeySucceededEvent += updateCloudDataForKeySucceededEvent;
			GPGManager.clearCloudDataForKeyFailedEvent += clearCloudDataForKeyFailedEvent;
			GPGManager.clearCloudDataForKeySucceededEvent += clearCloudDataForKeySucceededEvent;
			GPGManager.deleteCloudDataForKeyFailedEvent += deleteCloudDataForKeyFailedEvent;
			GPGManager.deleteCloudDataForKeySucceededEvent += deleteCloudDataForKeySucceededEvent;*/
			
			GPGManager.unlockAchievementFailedEvent += unlockAchievementFailedEvent;
			GPGManager.unlockAchievementSucceededEvent += unlockAchievementSucceededEvent;
			GPGManager.incrementAchievementFailedEvent += incrementAchievementFailedEvent;
			GPGManager.incrementAchievementSucceededEvent += incrementAchievementSucceededEvent;
			GPGManager.revealAchievementFailedEvent += revealAchievementFailedEvent;
			GPGManager.revealAchievementSucceededEvent += revealAchievementSucceededEvent;
			
			GPGManager.submitScoreFailedEvent += submitScoreFailedEvent;
			GPGManager.submitScoreSucceededEvent += submitScoreSucceededEvent;
			GPGManager.loadScoresFailedEvent += loadScoresFailedEvent;
			GPGManager.loadScoresSucceededEvent += loadScoresSucceededEvent;
			#endif
		}
		
		protected override void RemoveEventListeners()
		{
			#if UNITY_ANDROID
			GPGManager.authenticationSucceededEvent -= authenticationSucceededEvent;
			GPGManager.authenticationFailedEvent -= authenticationFailedEvent;
			GPGManager.licenseCheckFailedEvent -= licenseCheckFailedEvent;
			GPGManager.profileImageLoadedAtPathEvent -= profileImageLoadedAtPathEvent;
			GPGManager.finishedSharingEvent -= finishedSharingEvent;
			GPGManager.userSignedOutEvent -= userSignedOutEvent;
			
			GPGManager.reloadDataForKeyFailedEvent -= reloadDataForKeyFailedEvent;
			GPGManager.reloadDataForKeySucceededEvent -= reloadDataForKeySucceededEvent;
			/*GPGManager.loadCloudDataForKeyFailedEvent -= loadCloudDataForKeyFailedEvent;
			GPGManager.loadCloudDataForKeySucceededEvent -= loadCloudDataForKeySucceededEvent;
			GPGManager.updateCloudDataForKeyFailedEvent -= updateCloudDataForKeyFailedEvent;
			GPGManager.updateCloudDataForKeySucceededEvent -= updateCloudDataForKeySucceededEvent;
			GPGManager.clearCloudDataForKeyFailedEvent -= clearCloudDataForKeyFailedEvent;
			GPGManager.clearCloudDataForKeySucceededEvent -= clearCloudDataForKeySucceededEvent;
			GPGManager.deleteCloudDataForKeyFailedEvent -= deleteCloudDataForKeyFailedEvent;
			GPGManager.deleteCloudDataForKeySucceededEvent -= deleteCloudDataForKeySucceededEvent;*/
			
			GPGManager.unlockAchievementFailedEvent -= unlockAchievementFailedEvent;
			GPGManager.unlockAchievementSucceededEvent -= unlockAchievementSucceededEvent;
			GPGManager.incrementAchievementFailedEvent -= incrementAchievementFailedEvent;
			GPGManager.incrementAchievementSucceededEvent -= incrementAchievementSucceededEvent;
			GPGManager.revealAchievementFailedEvent -= revealAchievementFailedEvent;
			GPGManager.revealAchievementSucceededEvent -= revealAchievementSucceededEvent;
			
			GPGManager.submitScoreFailedEvent -= submitScoreFailedEvent;
			GPGManager.submitScoreSucceededEvent -= submitScoreSucceededEvent;
			GPGManager.loadScoresFailedEvent -= loadScoresFailedEvent;
			GPGManager.loadScoresSucceededEvent -= loadScoresSucceededEvent;
			#endif
		}
		#endregion
		
		#region Event Handlers
		#if UNITY_ANDROID
		private void authenticationSucceededEvent(string param)
		{
			SendInitializedEvent();
//			ZedLogger.Log("authenticationSucceededEvent: " + param);
		}
		
		private void authenticationFailedEvent(string error)
		{
			SendInitializedEvent();
			Debug.Log("Google Play Game services login failed: " + error);
			PlayerDataManager.Instance.RequestGameCenterLogin();
			PlayerDataManager.Instance.Save();
//			ZedLogger.Log("authenticationFailedEvent: " + error);
		}
		
		private void licenseCheckFailedEvent()
		{
//			ZedLogger.Log("licenseCheckFailedEvent");
		}
		
		private void profileImageLoadedAtPathEvent(string path)
		{
//			ZedLogger.Log("profileImageLoadedAtPathEvent: " + path);
		}
		
		private void finishedSharingEvent(string errorOrNull)
		{
//			ZedLogger.Log("finishedSharingEvent. errorOrNull param: " + errorOrNull);
		}
		
		private void userSignedOutEvent()
		{
//			ZedLogger.Log("userSignedOutEvent");
		}
		
		private void reloadDataForKeyFailedEvent(string error)
		{
//			ZedLogger.Log("reloadDataForKeyFailedEvent: " + error);
		}
		
		private void reloadDataForKeySucceededEvent(string param)
		{
//			ZedLogger.Log("reloadDataForKeySucceededEvent: " + param);
		}
		
		private void loadCloudDataForKeyFailedEvent(string error)
		{
//			ZedLogger.Log("loadCloudDataForKeyFailedEvent: " + error);
		}
		
		private void loadCloudDataForKeySucceededEvent(int key, string data)
		{
//			ZedLogger.Log("loadCloudDataForKeySucceededEvent:" + data);
		}
		
		private void updateCloudDataForKeyFailedEvent(string error)
		{
//			ZedLogger.Log("updateCloudDataForKeyFailedEvent: " + error);
		}
		
		private void updateCloudDataForKeySucceededEvent(int key, string data)
		{
//			ZedLogger.Log("updateCloudDataForKeySucceededEvent: " + data);
		}
		
		private void clearCloudDataForKeyFailedEvent(string error)
		{
//			ZedLogger.Log("clearCloudDataForKeyFailedEvent: " + error);
		}
		
		private void clearCloudDataForKeySucceededEvent(string param)
		{
//			ZedLogger.Log("clearCloudDataForKeySucceededEvent: " + param);
		}
		
		private void deleteCloudDataForKeyFailedEvent(string error)
		{
//			ZedLogger.Log("deleteCloudDataForKeyFailedEvent: " + error);
		}
		
		private void deleteCloudDataForKeySucceededEvent(string param)
		{
//			ZedLogger.Log("deleteCloudDataForKeySucceededEvent: " + param);
		}
		
		private void unlockAchievementFailedEvent(string achievementId, string error)
		{
//			ZedLogger.Log("unlockAchievementFailedEvent. achievementId: " + achievementId + ", error: " + error);
		}
		
		private void unlockAchievementSucceededEvent(string achievementId, bool newlyUnlocked)
		{
//			ZedLogger.Log("unlockAchievementSucceededEvent. achievementId: " + achievementId + ", newlyUnlocked: " + newlyUnlocked);
		}
		
		private void incrementAchievementFailedEvent(string achievementId, string error)
		{
//			ZedLogger.Log("incrementAchievementFailedEvent. achievementId: " + achievementId + ", error: " + error);
		}
		
		private void incrementAchievementSucceededEvent(string achievementId, bool newlyUnlocked)
		{
//			ZedLogger.Log("incrementAchievementSucceededEvent. achievementId: " + achievementId + ", newlyUnlocked: " + newlyUnlocked);
		}
		
		private void revealAchievementFailedEvent(string achievementId, string error)
		{
//			ZedLogger.Log("revealAchievementFailedEvent. achievementId: " + achievementId + ", error: " + error);
		}
		
		private void revealAchievementSucceededEvent(string achievementId)
		{
//			ZedLogger.Log("revealAchievementSucceededEvent: " + achievementId);
		}
		
		private void submitScoreFailedEvent(string leaderboardId, string error)
		{
//			ZedLogger.Log("submitScoreFailedEvent. leaderboardId: " + leaderboardId + ", error: " + error);
		}
		
		private void submitScoreSucceededEvent(string leaderboardId, Dictionary<string,object> scoreReport)
		{
//			ZedLogger.Log("submitScoreSucceededEvent");
			Prime31.Utils.logObject(scoreReport);
		}
		
		private void loadScoresFailedEvent(string leaderboardId, string error)
		{
//			ZedLogger.Log("loadScoresFailedEvent. leaderboardId: " + leaderboardId + ", error: " + error);
		}
		
		private void loadScoresSucceededEvent(List<GPGScore> scores)
		{
			//ZedLogger.Log("loadScoresSucceededEvent");
			Prime31.Utils.logObject(scores);
		}
		#endif
		#endregion

		#region Event Handlers
		#endregion
	}
}
