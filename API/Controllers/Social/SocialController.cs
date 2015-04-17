using UnityEngine;
using System;
using System.Collections;
using Zedarus.Traffico.Settings;
using Zedarus.Traffico.Data.PlayerData;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	public enum SocialMedia
	{
		Facebook,
		Twitter,
		Email,
		None
	}
	
	public enum SocialSharingReward
	{
		Coins,
		Stars,
		None
	}
	
	public enum SocialAction
	{
		ShareLevelScreenshot,
		ShareGame,
		None
	}
	
	public class SocialController : APIController
	{
		#region Parameters
		private SocialAction _action;
		#endregion
		
		#region Events
		public event Action SharingStarted;
		public event Action<bool> SharingFinished;
		#endregion
		
		#region Initialization
		public SocialController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
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
				#if API_FACEBOOK_P31
				case APIs.Facebook:
					return FacebookWrapper.Instance;
				#endif
				#if API_TWITTER_P31
				case APIs.Twitter:
					return TwitterWrapper.Instance;
				#endif
				#if API_EMAIL_P31
				case APIs.Email:
					return EmailWrapper.Instance;
				#endif
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void PostTextAndImage(string subject, string text, string imagePath, byte[] image, string url, SocialMedia media, SocialAction action)
		{
			SetRewardAction(action);
			ISocialWrapperInterface wrapper = WrapperForMedia(media);
			if (wrapper != null)
				wrapper.PostTextAndImage(subject, text, imagePath, image, url);
			else
				ZedLogger.LogWarning("No wrapper for media: " + media);
		}
		
		public void Share(string link, string name, string caption, string description, string pictureURL, SocialMedia media, SocialAction action)
		{
			SetRewardAction(action);
			ISocialWrapperInterface wrapper = WrapperForMedia(media);
			if (wrapper != null)
				wrapper.Share(link, name, caption, description, pictureURL);
			else
				ZedLogger.LogWarning("No wrapper for media: " + media);
		}
		#endregion
		
		#region Rewards
		private void SetRewardAction(SocialAction action)
		{
			_action = action;
		}
		
		private Reward ApplyReward(bool result)
		{	
			Reward award = new Reward(_action, result);
			_action = SocialAction.None;
			
			if (!result)
				return award;
			
			switch (award.Currency)
			{
				case SocialSharingReward.Coins:
					PlayerDataManager.Instance.Wallet.AddCoins(award.Amount, true);
					break;
			}
			
			return award;
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();
			
			foreach (ISocialWrapperInterface wrapper in Wrappers)
			{
				wrapper.SharingStarted += OnSharingStarted;
				wrapper.SharingFinished += OnSharingFinished;
			}
		}
		
		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();
			
			foreach (ISocialWrapperInterface wrapper in Wrappers)
			{
				wrapper.SharingStarted -= OnSharingStarted;
				wrapper.SharingFinished -= OnSharingFinished;
			}
		}
		#endregion
		
		#region Event Handlers
		private void OnSharingStarted()
		{	
			// TODO: send event to display processing popup
			//PopupManager.Instance.ShowProcessingPopup();
			if (SharingStarted != null)
				SharingStarted();
		}
		
		private void OnSharingFinished(bool result)
		{
			//Reward reward = ApplyReward(result);
			// TODO: use events to display popup
			//PopupManager.Instance.ShowSocialSharingResultPopup(null, result, reward.Currency, reward.Amount);
			
			if (SharingFinished != null)
				SharingFinished(result);
		}
		#endregion
		
		#region Getters
		protected ISocialWrapperInterface WrapperForMedia(SocialMedia media)
		{
			APIs api = ConvertMediaToAPI(media);
			return (ISocialWrapperInterface) WrapperWithAPI(api);
		}
		#endregion
		
		#region Helpers
		private APIs ConvertMediaToAPI(SocialMedia media)
		{
			switch (media)
			{
				case SocialMedia.Facebook:
					return APIs.Facebook;
				case SocialMedia.Twitter:
					return APIs.Twitter;
				case SocialMedia.Email:
					return APIs.Email;
				default:
					return APIs.None;
			}
		}
		#endregion
	}
	
	internal class Reward
	{
		private SocialSharingReward _currency;
		private int _amount;
		
		public Reward(SocialAction action, bool result)
		{
			_currency = GetRewardCurrency(action, result);
			_amount = GetRewardAmount(action, result);
		}
		
		private int GetRewardAmount(SocialAction action, bool result)
		{
			if (!result)
				return 0;
			
			switch (action)
			{
				case SocialAction.ShareGame:
					return GlobalSettings.Instance.RewardForSharingTheGame.Amount;
				case SocialAction.ShareLevelScreenshot:
					return GlobalSettings.Instance.RewardForSharingTheLevel.Amount;
				default:
					return 0;
			}
		}
		
		private SocialSharingReward GetRewardCurrency(SocialAction action, bool result)
		{
			if (!result)
				return SocialSharingReward.None;
			
			switch (action)
			{
				case SocialAction.ShareGame:
					return GlobalSettings.Instance.RewardForSharingTheGame.Currency;
				case SocialAction.ShareLevelScreenshot:
					return GlobalSettings.Instance.RewardForSharingTheLevel.Currency;
				default:
					return SocialSharingReward.None;
			}
		}
		
		public SocialSharingReward Currency
		{
			get { return _currency; }
		}
		
		public int Amount
		{
			get { return _amount; }
		}
	}
}
