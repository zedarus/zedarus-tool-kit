# Zedarus Tool Kit

A bunch or reusable classes, utilities and tools that I'm using in my games in Unity.

## Important

Since this project ignores Unity's `.meta` files, you should **NEVER** add any `MonoBehaviour` classes from this project directly to your scene objects as components, because as `GUID` for those scripts change between projects, you might have "Missing Behaviour" error.

Instead, just create a new class in your local project, extend the required class from ZTK and then add it as a component.

## Integration Guides (iOS)

### Ads (HeyZap)
- Follow integration guide on HeyZap website to setup all the IDs
- Make sure you take **age rating** into account when setting up ads in all networks + mediation:
  - **Chartboost**: go to publishing campaigns, open Advanced Targeting and check Block 17+ http://d.pr/i/1cvAb. Remember to do this for all campaigns
  - **AdMob**: unfortunatelly, AdMob does not have an age restriction setting, but you can filter ads categories using this guide: https://support.google.com/admob/answer/3150235?hl=en
  - **AppLovin**: go to managing apps, select your app and then check or uncheck Children's App setting, then check settings under Ad Filtering
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
- If you want to use cross-promo placement: 
  - Add call `AppController.Instance.API.Ads.ShowPromo(IDs.Ads.Interstitials.PromoOnPlayButton, OnPromoEnd, _adBlock)` where needed. Also
  - Enable promo ads in API Settings in game data and set delay in minutes

### IAPs

- Setup IAPs in iTunesConnect
- Enable UnityPurchases in Services tab
- Add `API.Store.Use(APIs.IAPs.Unity, 0f)` to `AppController`
- Add `API_IAP_UNITY` to project build settings
- Expand `IAPProduct` class in `GameData` to create your own products
- Override `protected override StoreProduct[] ProductList` getter in `AppController`, but don’t forget to include `base.ProductList` in your new list!
- Add `protected override void OnProductPurchaseFinished(string productID, bool success)` override to `AppController`
- Remember to add `AppController.Instance.API.Store.RestorePurchases()` to options screen
- If you support Remove Ads, add `AppController.Instance.PurchaseAdsRemoval()` to options screen
- :exclamation: **Xcode**: Remember to enable IAPs support in Xcode

Troubleshooting:
- Sometimes sandbox requests take too long to process during testing, that’s okay

### Achievements and Leaderboards

- Setup achievements and leaderboards in iTunesConnect
- Create achievements generic conditions in `GameData` in Unity
- Add achievements and leaderboards data to `GameData` in Unity
- Add `API.Score.Use(APIs.Score.GameCenter, 0f)` to `AppController`
- Add Prime31 GameCenter plugin
- Add `API_SCORE_GAMECENTER` to project build settings
- Add all achievements conditions IDs to `Settings.IDs` file for easier reference
- Add `AppController.Instance.API.Score.SubmitScore(<score>,  AppController.Instance.Data.Game.DefaultLeaderboard.CurrentPlatformID)` calls where needed, replace leaderboard ID if needed
- Add `AppController.Instance.Data.Player.AchievementsTracker.UpdateConditionParameter()` calls where needed
- Add `protected override bool OnCheckCustomAchievementCondition(int achievementID, object parameterValue)` override to `AppController` if any custom conditions for achievements are requered
- Add `AppController.Instance.API.Score.DisplayLeaderboardsList()` and `AppController.Instance.API.Score.DisplayAchievementsList()` calls where needed
- :exclamation: **Xcode**: Remember to enable GameCenter support in Xcode

Troubleshooting:
- Achievements and leaderboards sometimes updated slow in sandbox 

### iCloud Sync

- Add Prime31 iCloud plugin to the project
- Add `API_SYNC_ICLOUD` to build settings
- Add `API.Sync.Use(APIs.Sync.iCloud, 0f, "<filename>.dat")` (change the filename to the one you like though)
- Make sure all your custom player data model classes have `Merge()` method implemented
- Override `protected virtual bool DisplaySyncConfirmUI(System.Action syncConfirmedHandler, System.Action syncDeniedHandler)` in your `AppController`. If UI is not ready to display popup right now, return false. In that case, API will try to display this popup in 0.5 sec again.
- Add sync button to options screen and change sync state using `AppController.Instance.Data.Player.APIState.ChangeSyncState(!AppController.Instance.Data.Player.APIState.SyncEnabled)`
- Don’t forget to call `AppController.Instance.API.Sync.ApplyLoadedData()` if sync is enabled again in options screen. Here's a sample code for options screen flow:

 ```c#
 public void OnSyncButtonPress()
 {
 	AppController.Instance.Data.Player.APIState.ChangeSyncState(!AppController.Instance.Data.Player.APIState.SyncEnabled);
	if (AppController.Instance.Data.Player.APIState.SyncEnabled)
	{
		AppController.Instance.API.Sync.ApplyLoadedData();
	}
	UpdateSyncButtonState();
 }
 
 private void UpdateSyncButtonState()
 {
	if (_icloudButtonText != null)
	{
		_icloudButtonText.text = AppController.Instance.Data.Player.APIState.SyncEnabled ? AppController.Instance.Localisation.Localise(IDs.Localisation.Buttons.ICLOUD_ON) : AppController.Instance.Localisation.Localise(IDs.Localisation.Buttons.ICLOUD_OFF);
	}
 }
 ```

- If you need to know when new data was applied from cloud, listen to `Zedarus.ToolKit.Settings.IDs.Events.CloudSyncFinished` event
- :exclamation: **Xcode**: Remember to enable iCloud support in Xcode with iCloud Documents support
- :exclamation: **Xcode**: Sometimes Xcode switches off document support for iCloud, even though iCloud is still turned on. Make sure to check and correct that!

### Remote Data

- Make sure HeyZap plugin is already in and implemented, including Ads controller from above
- Make sure `API_ADS_HEYZAP` is added to build settings
- Add `API.RemoteData.Use(APIs.RemoteData.HeyZap, 0f)` to `AppController`
- To test locally, you can add `OnRemoteDataReceived(Resources.Load<TextAsset>("Data/RemoteData").text)` in `PostInit()` in `AppController` class
- You can use http://jsonlint.com to validate your JSON
- Remember to remove test JSON from HeyZap dashboard after done testing

### Unity Crash Reporting

- If using version of Unity prior to 5.4, add `API_CRASH_UNITY` to build settings and follow this guide: http://d.pr/i/14nPZ
- If using Unity 5.4, just enalbe reporting in Services

### Native Sharing

- Add Prime31 social plugin, but delete all the Twitter and Facebook related stuff. Basically, you need `SharingBinding` and `SharingManager` files + some files in Editor folder for Prime31
- Add `API_SHARE_NATIVE` to build settings
- Add `API.Share.Use(APIs.Sharing.Native, 0f)` to `AppController`
- Add `AppController.Instance.API.Share.Share()` call where needed.

### Analytics

- Enable Unity Analytics in Unity project's services tab
- Add `API_ANALYTICS_UNITY` to build settings
- Add `API.Analytics.Use(APIs.Analytics.Unity, 0f)` to `AppController`
- Add `AppController.Instance.API.Analytics.LogEvent()` calls where needed

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
  
- Add `LocaliseText` component to UI `Text` components that you want to localise without code
- Add localisation ids to your `IDs` class
- Use `AppController.Instance.Localisation.Localise()` method to localise strings in code
- Add `API_LOC_M2H` to build settings
- Language change is logged automatically in analytics (just search for `IDs.Events.SetLanguage` event in ZTK code)
- Remember to add lproj files to Xcode project for all supported langauges before final build
- TODO: create localisation packages wrappers system

### Push Notifications

- Setup your app on Batch.com using this guide: https://batch.com/doc/unity/prerequisites.html
- Generate push certificate and provision profiles: https://batch.com/doc/ios/prerequisites.html#_creating-a-certificate
- Download and add Bathc.com plugin to project. Make sure you put the C# files into Plugin folder, not root folder
- Add `API_PROMO_BATCH` to project's build settings
- Add `API.Promo.Use(APIs.Promo.Batch, 0f, "<your_dev_or_live_key>")` to your `AppController`
- Call `AppController.Instance.API.Promo.RequestNotificationsPermission()` somewhere at the start of the game. For example, when player click “Play” button or with some delay in main menu
- Test remote push notifications on this page: https://dashboard.batch.com/app/7265/settings#/push-settings
- :exclamation: REMEMBER to use live API key before making final build!
- :exclamation: **Xcode**: Make sure you make build using correct certificate and provision profile
- :exclamation: **Xcode**: Enable push notifications in Xcode
- :exclamation: **Xcode**: if you are planning to add promo codes support or redeem links, add Batch's custom scheme in
 
### Remote Rewards

- Setup unlock inventory in your app settings on batch.com and follow documentation: https://batch.com/doc/unlock/overview.html
- Override `OnProcessRemoteUnlockFeature(string feature, string value)`, `OnProcessRemoteUnlockResource(string resource, int quantity)` and `OnProcessRemoteUnlockParams(Dictionary<string, string> parameters)` in your `AppController` to correctly process remote unlocks. **important**: `UIManager` might not be available at the time of this call, so make sure you cache the message and display it later if it doesn't. TODO: handle this the same way as in iCloud permission popup
- If you are planning to use redeem links from Bathc, make sure to add special URL scheme in Xcode, more on this here: https://batch.com/doc/unlock/overview.html
- Add `AppController.Instance.API.Promo.RestoreRewards()` call where needed (along with IAPs restore?)
- Add `AppController.Instance.API.Promo.RedeemCode(code.text)` if requered, but read this:
- REMEMBER: We recommend against using promocodes on iOS. Apple guidelines indicate that promocodes are not permitted. Though some apps do successfully integrate this functionality, Batch does not recommend the practice.

### Local Push Notifications

- Add and setup local notifications in game data
- Note: local notifications are canceled, scheduled and rescheduled in base AppController class everytime app is started or comes back from background
- Override `OnProcessRewardFromLocalNotification(int rewardID)` in your `AppController` to handle rewards from local notifications

### Helpers

#### `AppController.Instance.Helpers.Promo`

- `SendContactsEmail()` - opens email composition window with email and subject specified in settings in game data
- `OnMoreLevelsPress()` - opens URL that was specified in settings in game data and return `true`. Returns `false` otherwise so you can do something else, like display rate me popup
- `OnMoreGamesPress()` - either opens URL or display interstitial ad with custom placement (all specified in settings in game data)
- `OnFacebookButtonPress()` - opens URL that was specified in settings in game data
- `OpenRateAppPage()` - open rate URL from game data Rate Popup settings and also logs opening in analytics API

### Extentions

- Override `InitExtentions()` method in your `AppController`

#### One Tap Games

##### Second Chance Popup

- Add `AddExtention(new SecondChancePopup(Data.Game, API, "<video ad>"));` in your `InitExtentions()` override in `AppController`
- Add this to your `GameData` class and set setting you like in game data editor in Unity

  ```c#
  [SerializeField]
  [DataTable(15, "Second Chance Popup Settings", typeof(SecondChancePopupData))]
  private SecondChancePopupData _seconChancePopupSettings;
  ```

- Subscribe (and also remember to unsubscribe!) to second chance popup events:

  ```c#
  SecondChancePopup secondChancePopup = AppController.Instance.GetExtention<SecondChancePopup>();
  if (secondChancePopup != null)
  {
  	secondChancePopup.PayForSecondChance += OnPayForSecondChance;
  	secondChancePopup.UseSecondChance += OnUseSecondChance;
  	secondChancePopup.DeclineSecondChance += OnDeclineSecondChance;
  }
  ```

- Call `AppController.Instance.GetExtention<SecondChancePopup>().RegisterSessionStart()` and `AppController.Instance.GetExtention<SecondChancePopup>().RegisterSessionEnd()` when game session starts and end. *Important* to note, that both should only be called when actual session starts or ends, not when second chance was used. So `RegisterSessionStart()` should not be called when player's character was resurrected for example, and  `RegisterSessionEnd()` should not be called exactly on player's death, because he might use second chance. Instead, you need first to check if player used second chance, and only then call this method
- Call this before you enter game over state:

  ```c#
  AppController.Instance.GetExtention<SecondChancePopup>().DisplayPopup(
  UIManager.Instance, IDs.UI.Popups.GenericPopup, "Second chance message", 
  <player's score>, <player's wallet balance>,
  "Free", "{0:n0} coins", "No"
  )
  ```

  This returns `true` if second chance popup is pesented (and so you need to wait for player's choice before finally entering game over state), and `false` if not and you can safely enter final game over state in that case

##### Double Coins Popup

- Add `AddExtention(new Zedarus.ToolKit.Extentions.OneTapGames.DoubleCoinsPopup.DoubleCoinsPopup(Data.Game, API, "<double_coins_video_ad_id>"));` in your `InitExtentions()` override in `AppController`
- Add this to your `GameData` class and set setting you like in game data editor in Unity

  ```c#
  [SerializeField]
  [DataTable(5, "Double Coins Popup Settings", typeof(DoubleCoinsPopupData))]
  private DoubleCoinsPopupData _doubleCoinsPopupData;
  ```

- Subscribe (and also remember to unsubscribe!) to popup events:

  ```c#
  DoubleCoinsPopup doubleCoinsPopup = AppController.Instance.GetExtention<DoubleCoinsPopup>();
  if (doubleCoinsPopup != null)
  {
  	doubleCoinsPopup.DoubleCoinsConfirm += OnDoubleEarnedCoinsConfirm;
  	doubleCoinsPopup.DoubleCoinsCancel += OnDoubleEarnedCoinsCancel;
  }
  ```

- Call `AppController.Instance.GetExtention<DoubleCoinsPopup>().RegisterSessionStart()` and `AppController.Instance.GetExtention<DoubleCoinsPopup>().RegisterSessionEnd()` when game session starts and end. *Important* to note, that both should only be called when actual session starts or ends, not when second chance was used, etc. So `RegisterSessionStart()` should *not* be called when player's character was resurrected for example, and  `RegisterSessionEnd()` should not be called exactly on player's death, because he might use second chance. Instead, you need first to check if player used second chance, and only then call this method
- Call this before you enter game over state:

  ```c#
  AppController.Instance.GetExtention<DoubleCoinsPopup>().DisplayPopup();
  ```

  This returns `true` if popup is pesented (and so you need to wait for player's choice before finally entering game over state), and `false` if not and you can safely enter final game over state in that case

##### Free Remove Ads

- Add `AddExtention(new Zedarus.ToolKit.Extentions.OneTapGames.FreeRemoveAds.FreeRemoveAds(Data.Game, Data.Player, API, "<double_coins_video_ad_id>"));` in your `InitExtentions()` override in `AppController`
- Set correct settings in API settings section in Game Data DB
- Call `DisplayPopup()` method on `FreeRemoveAds` instance and pass all required parameters
