using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    public static event System.Action<bool> RequestShare;
    public static event Action<bool> RecordGame;

    [Header("Object References")]
    public GameObject mainCanvas;
    public GameObject characterSelectionUI;
    public GameObject header;
    public GameObject title;
    public Text score;
    public Text bestScore;
    public Text coinText;
    public GameObject newBestScore;
    public GameObject playBtn;
    public GameObject restartBtn;
    public GameObject menuButtons;
    public GameObject dailyRewardBtn;
    public Text dailyRewardBtnText;
    public GameObject rewardUI;
    public GameObject settingsUI;
    public GameObject soundOnBtn;
    public GameObject soundOffBtn;
    public GameObject musicOnBtn;
    public GameObject musicOffBtn;
    public GameObject changeGameModeBtn;
    public GameObject goToMainScreenBtn;
    public GameObject getBackToPreviousGameModeBtn;
    public GameObject toolBar;

    [Header("Premium Features Buttons")]
    public GameObject watchRewardedAdBtn;
    public GameObject leaderboardBtn;
    public GameObject achievementBtn;
    public GameObject shareBtn;
    public GameObject iapPurchaseBtn;
    public GameObject removeAdsBtn;
    public GameObject restorePurchaseBtn;

    [Header("In-App Purchase Store")]
    public GameObject storeUI;

    [Header("Sharing-Specific")]
    public GameObject shareUI;
    public ShareUIController shareUIController;

    CameraController camController;
    Animator scoreAnimator;
    Animator dailyRewardAnimator;
    bool isWatchAdsForCoinBtnActive;

    public static bool isTouchShareUI = false;

    void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
        GameManager.StartRecord += StartRecord;
        ScoreManager.ScoreUpdated += OnScoreUpdated;
        ScoreManager.ShowNewBest += ShowNewBest;
    }

    void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
        GameManager.StartRecord -= StartRecord;
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
        ScoreManager.ShowNewBest -= ShowNewBest;
    }

    // Use this for initialization
    void Start()
    {
        camController = Camera.main.GetComponent<CameraController>();
        scoreAnimator = score.GetComponent<Animator>();
        dailyRewardAnimator = dailyRewardBtn.GetComponent<Animator>();

        Reset();
        ShowStartUI();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        coinText.text = CoinManager.Instance.Coins.ToString();

        if (!DailyRewardController.Instance.disable && dailyRewardBtn.gameObject.activeInHierarchy)
        {
            if (DailyRewardController.Instance.CanRewardNow())
            {
                dailyRewardBtnText.text = "GRAB YOUR REWARD!";
                dailyRewardAnimator.SetTrigger("activate");
            }
            else
            {
                TimeSpan timeToReward = DailyRewardController.Instance.TimeUntilReward;
                dailyRewardBtnText.text = string.Format("REWARD IN {0:00}:{1:00}:{2:00}", timeToReward.Hours, timeToReward.Minutes, timeToReward.Seconds);
                dailyRewardAnimator.SetTrigger("deactivate");
            }
        }

        if (settingsUI.activeSelf)
        {
            UpdateMuteButtons();
            UpdateMusicButtons();
        }

        CheckShareUI();
    }

    void ShowNewBest()
    {
        StartCoroutine(ShowNewBest_IEnum());
    }

    IEnumerator ShowNewBest_IEnum()
    {
        newBestScore.SetActive(true);
        yield return new WaitForSeconds(1);
        newBestScore.SetActive(false);
    }

    void CheckShareUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CheckShareUI_IEnum());
        }
    }

    IEnumerator CheckShareUI_IEnum()
    {
        yield return new WaitForSeconds(0.02f);
        if (shareUI.activeSelf && !isTouchShareUI)
        {
            shareUI.SetActive(false);
            toolBar.SetActive(true);
            if (RecordGame != null)
                RecordGame(true);
        }
    }

    void GameManager_GameStateChanged(GameState newState, GameState oldState)
    {
        if (newState == GameState.Playing)
        {              
            ShowGameUI();
        }
        else if (newState == GameState.PreGameOver)
        {
            // Before game over, i.e. game potentially will be recovered
        }
        else if (newState == GameState.GameOver)
        {
            //Invoke("ShowGameOverUI", 1f);
        }
        else if (newState == GameState.Prepare)
        {
            Reset();
            ShowStartUI();
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    void Reset()
    {
        mainCanvas.SetActive(true);
        characterSelectionUI.SetActive(false);
        header.SetActive(false);
        title.SetActive(false);
        score.gameObject.SetActive(false);
        newBestScore.SetActive(false);
        playBtn.SetActive(false);
        menuButtons.SetActive(false);
        dailyRewardBtn.SetActive(false);
        changeGameModeBtn.SetActive(false);
        goToMainScreenBtn.SetActive(false);
        getBackToPreviousGameModeBtn.SetActive(false);

        // Enable or disable premium stuff
        bool enablePremium = IsPremiumFeaturesEnabled();
        leaderboardBtn.SetActive(enablePremium);
        shareBtn.SetActive(enablePremium);
        iapPurchaseBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);

        // Hidden by default
        storeUI.SetActive(false);
        settingsUI.SetActive(false);
        shareUI.SetActive(false);

        // These premium feature buttons are hidden by default
        // and shown when certain criteria are met (e.g. rewarded ad is loaded)
        watchRewardedAdBtn.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void EndGame()
    {
        //GameManager.Instance.GameOver();
    }

    public void RestartGame()
    {
        float seconds = 0.2f;
        GameManager.Instance.RestartGame(seconds);
    }

    public void StartRecord()
    {
        if (RecordGame != null)
            RecordGame(true);
    }

    public void ShowStartUI()
    {
        settingsUI.SetActive(false);

        header.SetActive(true);
        title.SetActive(true);
        playBtn.SetActive(true);
        restartBtn.SetActive(false);
        menuButtons.SetActive(true);
        shareBtn.SetActive(false);
        toolBar.SetActive(false);
        shareUI.SetActive(false);

        // If first launch: show "WatchForCoins" and "DailyReward" buttons if the conditions are met
        if (GameManager.GameCount == 0)
        {
            ShowWatchForCoinsBtn();
            ShowDailyRewardBtn();
        }
    }

    public void ShowGameUI()
    {
        changeGameModeBtn.SetActive(true);
        goToMainScreenBtn.SetActive(true);
        shareBtn.SetActive(true);
        getBackToPreviousGameModeBtn.SetActive(true);
        if (!shareUI.activeSelf)
            toolBar.SetActive(true);
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        playBtn.SetActive(false);
        menuButtons.SetActive(false);
        dailyRewardBtn.SetActive(false);
        watchRewardedAdBtn.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        header.SetActive(true);
        title.SetActive(false);
        score.gameObject.SetActive(true);
        newBestScore.SetActive(ScoreManager.Instance.HasNewHighScore);

        playBtn.SetActive(false);
        restartBtn.SetActive(true);
        menuButtons.SetActive(true);
        settingsUI.SetActive(false);

        // Show 'daily reward' button
        ShowDailyRewardBtn();

        // Show these if premium features are enabled (and relevant conditions are met)
        if (IsPremiumFeaturesEnabled())
        {
            ShowShareUI();
            ShowWatchForCoinsBtn();
        }
    }

    void ShowWatchForCoinsBtn()
    {
        // Only show "watch for coins button" if a rewarded ad is loaded and premium features are enabled
        #if EASY_MOBILE
        if (IsPremiumFeaturesEnabled() && AdDisplayer.Instance.CanShowRewardedAd() && AdDisplayer.Instance.watchAdToEarnCoins)
        {
            watchRewardedAdBtn.SetActive(true);
            watchRewardedAdBtn.GetComponent<Animator>().SetTrigger("activate");
        }
        else
        {
            watchRewardedAdBtn.SetActive(false);
        }
        #endif
    }

    void ShowDailyRewardBtn()
    {
        // Not showing the daily reward button if the feature is disabled
        if (!DailyRewardController.Instance.disable)
        {
            dailyRewardBtn.SetActive(true);
        }
    }

    public void PressShareBtn()
    {
        if (RequestShare != null)
            RequestShare(true);
        if (RecordGame != null)
            RecordGame(false);
        toolBar.SetActive(false);
    }

    public void ShowSettingsUI()
    {
        settingsUI.SetActive(true);
    }

    public void HideSettingsUI()
    {
        settingsUI.SetActive(false);
    }

    public void ShowStoreUI()
    {
        storeUI.SetActive(true);
    }

    public void HideStoreUI()
    {
        storeUI.SetActive(false);
    }

    public void ShowCharacterSelectionScene()
    {
        mainCanvas.SetActive(false);
        camController.EnableBlur();
        characterSelectionUI.SetActive(true);
    }

    public void CloseCharacterSelectionScene()
    {
        mainCanvas.SetActive(true);
        camController.DisableBlur();
        characterSelectionUI.SetActive(false);
    }

    public void WatchRewardedAd()
    {
        #if EASY_MOBILE
        // Hide the button
        watchRewardedAdBtn.SetActive(false);

        AdDisplayer.CompleteRewardedAdToEarnCoins += OnCompleteRewardedAdToEarnCoins;
        AdDisplayer.Instance.ShowRewardedAdToEarnCoins();
        #endif
    }

    void OnCompleteRewardedAdToEarnCoins()
    {
        #if EASY_MOBILE
        // Unsubscribe
        AdDisplayer.CompleteRewardedAdToEarnCoins -= OnCompleteRewardedAdToEarnCoins;

        // Give the coins!
        ShowRewardUI(AdDisplayer.Instance.rewardedCoins);
        #endif
    }

    public void GrabDailyReward()
    {
        if (DailyRewardController.Instance.CanRewardNow())
        {
            int reward = DailyRewardController.Instance.GetRandomReward();

            // Round the number and make it mutiplies of 5 only.
            int roundedReward = (reward / 5) * 5;

            // Show the reward UI
            ShowRewardUI(roundedReward);

            // Update next time for the reward
            DailyRewardController.Instance.ResetNextRewardTime();
        }
    }

    public void ShowRewardUI(int reward)
    {
        rewardUI.SetActive(true);
        rewardUI.GetComponent<RewardUIController>().Reward(reward);
    }

    public void HideRewardUI()
    {
        rewardUI.GetComponent<RewardUIController>().Close();
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
            #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
            #elif UNITY_ANDROID
            GameServices.Init();
            #endif
        }
        #endif
    }

    public void ShowAchievementsUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
            #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
            #elif UNITY_ANDROID
            GameServices.Init();
            #endif
        }
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #endif
    }

    public void ShowShareUI()
    {
        if (!ScreenshotSharer.Instance.disableSharing)
        {
            Texture2D texture = ScreenshotSharer.Instance.CapturedScreenshot;
            shareUIController.ImgTex = texture;

            #if EASY_MOBILE_PRO
            AnimatedClip clip = ScreenshotSharer.Instance.RecordedClip;
            shareUIController.AnimClip = clip;
            #endif

            shareUI.SetActive(true);
        }
    }

    public void ShowSHareUIAndDelay()
    {
        StartCoroutine(ShowShareUI_IEnum());
    }

    IEnumerator ShowShareUI_IEnum()
    {
        yield return new WaitForSeconds(0.2f);
        ShowShareUI();
    }

    public void HideShareUI()
    {
        shareUI.SetActive(false);
    }

    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }

    public void RateApp()
    {
        Utilities.RateApp();
    }

    public void OpenTwitterPage()
    {
        Utilities.OpenTwitterPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.OpenFacebookPage();
    }

    public void ButtonClickSound()
    {
        Utilities.ButtonClickSound();
    }

    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
            soundOnBtn.gameObject.SetActive(false);
            soundOffBtn.gameObject.SetActive(true);
        }
        else
        {
            soundOnBtn.gameObject.SetActive(true);
            soundOffBtn.gameObject.SetActive(false);
        }
    }

    void UpdateMusicButtons()
    {
        if (SoundManager.Instance.IsMusicOff())
        {
            musicOffBtn.gameObject.SetActive(true);
            musicOnBtn.gameObject.SetActive(false);
        }
        else
        {
            musicOffBtn.gameObject.SetActive(false);
            musicOnBtn.gameObject.SetActive(true);
        }
    }

    bool IsPremiumFeaturesEnabled()
    {
        return PremiumFeaturesManager.Instance != null && PremiumFeaturesManager.Instance.enablePremiumFeatures;
    }

    public void ChangeGameMode()
    {
        GameManager.Instance.ChangeGameMode();
    }

    public void GetBackToPreviousGameMode()
    {
        GameManager.Instance.GetBackToPreviousGameMode();
    }

    public void GetBackToMainScreen()
    {
        if (RecordGame != null)
            RecordGame(false);
        if (SoundManager.Instance.background != null)
        {
            SoundManager.Instance.StopMusic();
        }
        GameManager.Instance.GoToMainScreen();
        ShowWatchForCoinsBtn();
        ShowDailyRewardBtn();
    }
}
