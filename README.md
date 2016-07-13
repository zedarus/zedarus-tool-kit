# Zedarus Tool Kit

A bunch or reusable classes, utilities and tools that I'm using in my games in Unity.

## Important

Since this project ignores Unity's `.meta` files, you should **NEVER** add any `MonoBehaviour` classes from this project directly to your scene objects as components, because as you `GUID` for those scripts change between projects, you might have "Missing Behaviour" error.

Instead, just create a new class in your local project, extend the required class from ZTK and then add it as a component.

## Integration Guides (iOS)

### Ads (HeyZap)
- Follow integration guide on HeyZap website to setup all the IDs
- Make sure you take **age rating** into account when setting up ads in all networks + mediation:
  - **Chartboost**: go to publishing campaigns, open Advanced Targeting and check Block 17+ http://d.pr/i/1cvAb. Remember to do this for all campaigns
  - **AdMob**: unfortunatelly, AdMob does not have an age restriction setting, but you can filter ads categories using this guide: https://support.google.com/admob/answer/3150235?hl=en
  - **AppLovin**: go to managing apps, select your app and then check or uncheck Children's App setting
  - **Unity Ads**: in Unity Editor, go to Services, Age Designation and select appropriate option. Then, go to app setting in Unity Ads dashboar, select Ads Filtering and select option under Ages: http://d.pr/i/171V8
  - **HeyZap**: select your app on dashboard, go to Publisher Setting and select appropriate option under Family Friendly Filter
- Make sure video interstitials are skippable:
  - **Chartboost**: no settings
  - **AdMob**: no settings
  - **AppLovin**: go to app settings and select appropriate option under Video Settings and Playable Ad Settings
  - **Unity Ads**: no settings
  - **HeyZap**: go to publisher settings for the app and select skippable options
- Make sure reward videos rewards are set correctly for ad networks:
  - **Chartboost**: no settings
  - **AdMob**: no settings
  - **AppLovin**: go to app settings and set correct values under Rewarded Video
  - **Unity Ads**: no settings
  - **HeyZap**: no settings
- Import HeyZap, Chartboost, AdMob, UnityAds and AppLovin SDKs to project. You can just copy them from one of the projects that already have them, or download from HeyZap integration guide (make sure to follow the additional instructions there!)
- Add `API_ADS_HEYZAP` to project build settings
- Add `API.Ads.Use(APIs.Ads.HeyZap, 0f, "<your id>");` int `InitAPI()` in your `AppController` class. Replace `<your id>` with the one provided by HeyZap
- Add `AppController.Instance.API.Ads.ShowTestUI(true);` somewhere in options screen button callback
- Add ads placements to `IDs` class, the most generic ones are: `between_level`, `crosspromo` and various placements for incentivized ads: `free_coins`, `xp_boost`, etc
- Add caching calls in `AppController.OnPostInit()` (like this: `API.Ads.CacheIntersitials(IDs.Ads.Interstitials.BetweenLevel)`)
- Between level ads: Just use this call when user presses Restart button in game over UI `AppController.Instance.API.Ads.ShowBetweenLevelAd(IDs.Ads.Interstitials.BetweenLevel, OnInterstitialClosed, _adBlock)`. `_adBlock` should be a UI gameobject that blocks all input. API will set it active and inactive automatically
- Reward calls: just use this call: `AppController.Instance.API.Ads.ShowRewardedVideo(IDs.Ads.RewardVideos.FreeCoins, null, OnVideoRewardCompleted, coinPack.ID)`
- Promo placement: add call `AppController.Instance.API.Ads.ShowPromo(IDs.Ads.Interstitials.PromoOnPlayButton, OnPromoEnd, _adBlock)` where needed. Also, remember to set correct values in API Settings in game data

Troubleshooting:
- Tbd

### IAPs

- Setup IAPs in iTunesConnect
- Enable UnityPurchases in Services tab
- Add API.Store.Use(APIs.IAPs.Unity, 0f); to AppController
- Add API_IAP_UNITY to project build settings
- Expand IAPProduct class in GameData to create your own products
- Override protected override StoreProduct[] ProductList in AppController, but don’t forget to include base.ProductList in your new list!
- Add protected override void OnProductPurchaseFinished(string productID, bool success) override to AppController
- Remember to add AppController.Instance.API.Store.RestorePurchases(); to options
- If you support Remove Ads, add AppController.Instance.PurchaseAdsRemoval(); to options screen
- Remember to enable IAPs support in Xcode

Troubleshooting:
- Sometimes sandbox requests take too long to process during testing, that’s okay

### Achievements and Leaderboards

- Setup achievements and leaderboards in iTunesConnect
- Create achievements generic conditions in GameData in Unity
- Add achievements and leaderboards data to GameData in Unity
- Add API.Score.Use(APIs.Score.GameCenter, 0f); to AppController
- Add Prime31 GameCenter plugin
- Add API_SCORE_GAMECENTER to project build settings
- Add all achievements conditions IDs to Settings.IDs file for easier reference
- Add AppController.Instance.API.Score.SubmitScore(AppController.Instance.Data.Player.Stats.BestScore,  AppController.Instance.Data.Game.DefaultLeaderboard.CurrentPlatformID); calls where needed
- Add AppController.Instance.Data.Player.AchievementsTracker.UpdateConditionParameter() calls where needed
- Add protected override bool OnCheckCustomAchievementCondition(int achievementID, object parameterValue) override to AppController is any custom conditions for achievements are requered
- Add AppController.Instance.API.Score.DisplayLeaderboardsList(); and AppController.Instance.API.Score.DisplayAchievementsList(); calls where needed
- Remember to enable GameCenter support in Xcode

Troubleshooting:
- Achievements and leaderboards sometimes updated slow in sandbox 

### iCloud Sync

- Add Prime31 iCloud plugin to the project
- Add API_SYNC_ICLOUD to build settings
- Add API.Sync.Use(APIs.Sync.iCloud, 0f, "bladecloud002.dat"); (change the filename to the one you like though)
- Make sure all your custom player data model classes have Merge() method implemented
- Override protected virtual bool DisplaySyncConfirmUI(System.Action syncConfirmedHandler, System.Action syncDeniedHandler) in your AppController. If UI is not ready to display popup right now, return false. In that case, API will try to display this popup in 0.5 sec again.
- Add Sync button to options screen and change sync state using AppController.Instance.Data.Player.APIState.ChangeSyncState( !AppController.Instance.Data.Player.APIState.SyncEnabled );
- Don’t forget to call AppController.Instance.API.Sync.ApplyLoadedData(); if sync is enabled again in options screen
- Remember to enable iCloud support in Xcode with iCloud Documents support
- Important: sometimes Xcode switches off document support for iCloud, even though iCloud is still turned on. Make sure to check and correct that!

### Remote Data

- Make sure HeyZap plugin is already in and implemented, including Ads controller from above
- Make sure API_ADS_HEYZAP is added to build settings
- Add API.RemoteData.Use(APIs.RemoteData.HeyZap, 0f); to AppController
- To test locally, you can add `OnRemoteDataReceived(Resources.Load<TextAsset>("Data/RemoteData").text)` in `PostInit()` in AppController class
- You can use http://jsonlint.com to validate your JSON
- Remember to remove test JSON from HeyZap dashboard after done testing

### Unity Crash Reporting

- If using version of Unity prior to 5.4, add API_CRASH_UNITY to build settings
- If using Unity 5.4, just enalbe reporting in Services

### Native Sharing

- Add Prime31 social plugin, but delete all the Twitter and Facebook related stuff. Basically, you need SharingBinding and SharingManager files + some files in Editor folder for Prime31
- Add API_SHARE_NATIVE to build settings
- Add API.Share.Use(APIs.Sharing.Native, 0f); to AppController
- Add AppController.Instance.API.Share.Share(); call where needed.

### Analytics

- Enable Unity Analytics in Unity project's services tab
- Add API_ANALYTICS_UNITY to build settings
- Add API.Analytics.Use(APIs.Analytics.Unity, 0f); to AppController
- Add AppController.Instance.API.Analytics.LogEvent() calls where needed

### Audio

- Make sure to add MasterAudio plugin first
- Setup MasterAudio instances in boot scene and make sure they are kept across scenes
- Then add `AUDIO_MASTER_AUDIO` to build settings
- Use `AppController.Instance.Audio` to get access to audio controller
- Use `AppController.Instance.Audio.ToggleSound()` and `AppController.Instance.Audio.ToggleMusic()` if you need to toggle sounds or music mute
- Listen to `Zedarus.ToolKit.Settings.IDs.Events.AudioStateUpdated` event where needed to update UI button state, etc
- Use `AppController.Instance.Audio.SoundEnabled` and `AppController.Instance.Audio.MusicEnabled` if you need to check if sounds or music are enabled
- Override `Zedarus.ToolKit.UI.Elements.UIButtonAudio` class in your custom class and add it to all game objects with `Button` component you have in your UI

### Localisation

- Add this localisation package to the game: http://www.m2h.nl/files/LocalizationPackage.pdf
- Override `LocaliseText` and `LocaliseTextEditor` in your local scripts to avoid meta files missup
- Override `protected abstract LocalisationManager LocManagerRef { get; }` in `LocaliseText`. It should return reference to `AppContoller.Instance.Localisation` (your **local** controller). But make sure that `AppController` has instance ready first. Here's the code:

  ```c#
  protected override Zedarus.ToolKit.Localisation.LocalisationManager LocManagerRef 
	{ 
		get 
		{ 
			if (AppController.HasInstance)
			{
				return AppController.Instance.Localisation; 
			}
			else
			{
				return null;
			}
		}
	}
  ```
  
- Add `API_LOC_M2H` to build settings
- Language change is logged automatically in analytics (just search for `IDs.Events.SetLanguage` event in ZTK code)
- Remember to add lproj files to Xcode project for all supported langauges before final build
- TODO: create localisation packages wrappers system

### Promo

- Setup your app on Batch.com using this guide: https://batch.com/doc/unity/prerequisites.html
- Generate push certificate and provision profiles: https://batch.com/doc/ios/prerequisites.html#_creating-a-certificate
- Download and add Bathc.com plugin to project. Make sure you put the C# files into Plugin folder, not root folder
- Add `API_PROMO_BATCH` to project's build settings
- Add `API.Promo.Use(APIs.Promo.Batch, 0f, "<your_dev_or_live_key>")` to your `AppController`
- Call `AppController.Instance.API.Promo.RequestNotificationsPermission()` somewhere at the start of the game. For example, when player click “Play” button or with some delay in main menu
- Test remote push notifications on this page: https://dashboard.batch.com/app/7265/settings#/push-settings
- Add and setup local notifications in game data
- Note: local notifications are canceled, scheduled and rescheduled in base AppController class everytime app is started or comes back from background
- Override `OnProcessRewardFromLocalNotification(int rewardID)` in your `AppController` to handle rewards from local notifications
- REDEEM:
- Setup unlock inventory in your app settings on batch.com and follow documentation: https://batch.com/doc/unlock/overview.html
- Override `OnProcessRemoteUnlockFeature(string feature, string value)`, `OnProcessRemoteUnlockResource(string resource, int quantity)` and `OnProcessRemoteUnlockParams(Dictionary<string, string> parameters)` in your `AppController` to correctly process remote unlocks. **important**: `UIManager` might not be available at the time of this call, so make sure you cache the message and display it later if it doesn't. TODO: handle this the same way as in iCloud permission popup
- If you are planning to use redeem links from Bathc, make sure to add special URL scheme in Xcode, more on this here: https://batch.com/doc/unlock/overview.html
- Add `AppController.Instance.API.Promo.RestoreRewards()` call where needed (along with IAPs restore?)
- Add `AppController.Instance.API.Promo.RedeemCode(code.text)` if requered, but read this:
- REMEMBER: We recommend against using promocodes on iOS. Apple guidelines indicate that promocodes are not permitted. Though some apps do successfully integrate this functionality, Batch does not recommend the practice.
- REMEMBER to use live API key before making final build!
- Make sure you make build using correct certificate and provision profile

