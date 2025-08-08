using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Genit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectScene : MonoBehaviour
{
    [SerializeField] private GameLoadingPanel gameLoadingScene;
    [SerializeField] private Transform chapterButtonParent;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Text levelText;
    [SerializeField] private Image partsImage;
    [SerializeField] private ScrollRect scrollRect;
    

    private int _selectedGrade;
    private TimeDispatcher _timer;
    private int _challengeLevel;

    private async void Start()
    {
#if UNITY_EDITOR //開発用
        if (MasterData.GetInstance().quizzes == null) SceneManager.LoadScene("TitleScene");
#endif

        var masterData = MasterData.GetInstance();
        // var subjects = masterData.AllSubjects();
        _selectedGrade = UserDataManager.GetInstance().GetUserData().grade;

        AdManager.Instance.LoadBannerAd();

        _timer = gameObject.AddComponent<TimeDispatcher>();
        _timer.OnSecond = async dateTime =>
        {
            if (UserDataManager.GetInstance().TimeLimit < dateTime)
            {
                Debug.Log("TimeLimit");
                _timer.OnSecond = null;
                var goToTitleDialogObj = await Utils.OpenDialog("Prefabs/Select/GoToTitleDialog", transform);
                var goToTitleDialog = goToTitleDialogObj.GetComponent<OkDialog>();
                goToTitleDialog.Setup(
                    () => { SceneManager.LoadScene("TitleScene"); });
            }
        };
        
        // SetTopBar();
        SetChapterButtons();
        
    }

    private async void OnEnable()
    {
        UserDataManager.GetInstance().AddUserDataUpdateListener(UpdatePlayerStatusUI);
        UserDataManager.GetInstance().AddUserDataUpdateListener(OnUserDataUpdated);
        UserDataManager.GetInstance().AddOwnedRoboPartsUpdateListener(UpdatePlayerStatusUI);
        
        BgmClips.Play(BgmClips.BgmType.Select);

        var lastLoginDateTime = Utils.UnixTimeToDateTime(UserDataManager.GetInstance().GetUserData().lastLoginDateTime);
        var now = Clock.GetInstance().Now();
        if (lastLoginDateTime.Date < now.Date)
        {
            int consecutiveLoginNum;
            if (lastLoginDateTime.Date == now.Date.AddDays(-1))
                consecutiveLoginNum = UserDataManager.GetInstance().GetUserData().consecutiveLoginNum + 1;
            else
                consecutiveLoginNum = 1;

            await UserDataManager.GetInstance().SetUserData(UserDataManager.USER_DATA_KEY_LAST_LOGIN_DATE, Utils
                .DateTimeToUnixTime(now));
            await UserDataManager.GetInstance().SetUserData(UserDataManager.USER_DATA_KEY_CONSECUTIVE_LOGIN_NUM,
                consecutiveLoginNum);
            await UserDataManager.GetInstance().SetUserData(UserDataManager.USER_DATA_KEY_TOTAL_LOGIN_NUM,
                UserDataManager.GetInstance().GetUserData().totalLoginNum + 1);
            var loginDialogObj = await Utils.OpenDialog("Prefabs/Select/LoginDialog", transform);
            var loginDialog = loginDialogObj.GetComponent<LoginDialog>();
            loginDialog.Setup();
        }

        UpdatePlayerStatusUI();
    }

    private void OnDisable()
    {
        UserDataManager.GetInstance().RemoveUserDataUpdateListener(UpdatePlayerStatusUI);
        UserDataManager.GetInstance().RemoveUserDataUpdateListener(OnUserDataUpdated);
        UserDataManager.GetInstance().RemoveOwnedRoboPartsUpdateListener(UpdatePlayerStatusUI);
    }

    private void UpdatePlayerStatusUI()
    {
        var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
        int currentExp = playerStatus.exp;

        
        // 次のロボパーツを取得
        var ownedRoboId = UserDataManager.GetInstance().OwnedRoboPartsIds();
        Debug.Log($"ownedRoboId: {string.Join(", ", ownedRoboId)}");
        var unclaimedIds = UserDataManager.GetInstance().GetUnclaimedRewardRoboPartIds(currentExp);
        Debug.Log($"unclaimedIds: {string.Join(", ", unclaimedIds)}");
        var nextRobo = MasterData.GetInstance().GetNextUnownedRoboByExp(ownedRoboId);
        
        if (nextRobo != null)
        {
            // 次のロボパーツまでの経験値
            int nextExp = nextRobo.exp_required;
            
            // Sliderの更新
            if (expSlider != null)
            {
                expSlider.value = (float)currentExp / nextExp;
            }

            levelText.text = $"{currentExp}/{nextExp}";
            
            // ロボパーツ画像の更新
            if (partsImage != null && !string.IsNullOrEmpty(nextRobo.id))
            {
                string spritePath = $"Images/Robo/{nextRobo.id}";
                Sprite roboSprite = Resources.Load<Sprite>(spritePath);
                
                if (roboSprite != null)
                {
                    partsImage.sprite = roboSprite;
                }
                else
                {
                    Debug.LogWarning($"Sprite not found at path: {spritePath}");
                }
            }
        }
        else
        {
            // 最大レベルに到達している場合
            if (expSlider != null)
            {
                expSlider.value = 1f; // 満タン表示
            }
            
            Debug.Log("No next robo available - player has reached max level");
        }
    }
    
    private void OnUserDataUpdated()
    {
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject);
        }
        SetChapterButtons();
    }

    private void SetChapterButtons()
    {
        _challengeLevel = UserDataManager.GetInstance().GetChallengeLevel();
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject); // 既存のボタンを削除
        }

        // enemy_data.txt を読み込み
        var enemyDataText = Resources.Load<TextAsset>("Data/enemy_data");
        if (enemyDataText == null)
        {
            Debug.LogError("enemy_data.txt not found");
            return;
        }

        string[] lines = enemyDataText.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int chapterCount = lines.Length;

        float offsetX = 150f;
        float verticalOffset = 10f;
        float buttonHeight = 0f; // ボタンの高さを後で計算するために初期化

        for (int i = 0; i < chapterCount; i++)
        {
            int chapterNumber = i + 1;
            var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/SubjectSelect/ChapterStar"), chapterButtonParent);
            
            // chapterData を自前で構築（subject は固定で "算数"）
            ChapterData chapterData = new ChapterData
            {
                chapterNumber = chapterNumber,
            };

            chapterButton.Setup(chapterData, ActionWhenSelectChapter);

            var rect = chapterButton.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            buttonHeight = rect.sizeDelta.y * 0.7f;
            float y = verticalOffset + i * buttonHeight;
            float x = (i % 2 == 0) ? offsetX : -offsetX;

            rect.anchoredPosition = new Vector2(x, -y);
        }

        // chapterButtonParent の RectTransform を取得
        var contentRect = chapterButtonParent.GetComponent<RectTransform>();

        // 最後のボタンの位置を基に Content の高さを更新
        float totalHeight = verticalOffset + (chapterCount + 2) * buttonHeight; 
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);

        PlaceRobotOnChapter();
        if (_challengeLevel != 1)
        {
            ScrollToChallengeChapter();
        }
    }
    
    private void ScrollToChallengeChapter()
    {
        float offsetY = 100f;
        RectTransform targetRect = null;

        foreach (Transform child in chapterButtonParent)
        {
            var chapterStar = child.GetComponent<ChapterStar>();
            if (chapterStar == null) continue;

            var chapterData = chapterStar.GetChapterData();
            if (chapterData.chapterNumber == _challengeLevel)
            {
                targetRect = child.GetComponent<RectTransform>();
                break;
            }
        }

        if (targetRect == null)
        {
            Debug.LogWarning("スクロール対象のチャプターが見つかりません");
            return;
        }

        // Content全体の高さと目的位置のYから、normalizedPositionを計算
        var contentRect = chapterButtonParent.GetComponent<RectTransform>();
        float contentHeight = contentRect.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        float targetY = Mathf.Abs(targetRect.anchoredPosition.y + offsetY); // anchoredPosition.yはマイナス値
        float scrollRange = contentHeight - viewportHeight;
        float normalizedY = 1f - Mathf.Clamp01(targetY / scrollRange);

        scrollRect.verticalNormalizedPosition = normalizedY;
    }
    
    // private void SetTopBar()
    // {
    //     var userDataManager = UserDataManager.GetInstance();
    //     var userData = userDataManager.GetUserData();
    //     
    //     // レベルと経験値の表示
    //     levelText.text = $"Lv. {userData.level}";
    //     expSlider.value = (float)userData.exp / LevelingSystem.GetExpToLevelUp(userData.level);
    //     
    //     // パーツ画像の設定
    //     if (userData.selectedRoboId != null)
    //     {
    //         var roboCustomDataDict = userDataManager.GetRoboCustomData(userData.selectedRoboId);
    //         if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(userData.selectedRoboId))
    //         {
    //             partsImage.sprite = roboCustomDataDict[userData.selectedRoboId].GetPartSprite();
    //         }
    //     }
    // }
    
    private void ActionWhenSelectChapter(ChapterData data)
    {
        Const.GameSceneParam.ChapterNumber = data.chapterNumber;
        StartGame();
    }

    private async void PlaceRobotOnChapter()
    {
        RectTransform targetRect = null;
        float adjustedY = 130f;
        
        foreach (Transform child in chapterButtonParent)
        {if (child == null) continue;
            
            var chapterStar = child.GetComponent<ChapterStar>();
            if (chapterStar != null)
            {
                var chapterData = chapterStar.GetChapterData(); // ← ChapterData を返す getter を ChapterStar に追加してください
                if (chapterData.chapterNumber == _challengeLevel)
                {
                    targetRect = child.GetComponent<RectTransform>();
                    break;
                }
            }
        }

        Debug.Log($"ChallengeLevel: {_challengeLevel}");
        if (targetRect == null)
        {
            Debug.LogError("指定された challengeLevel に対応する ChapterStar が見つかりませんでした");
            return;
        }
        
        var robotAnchoredPos = targetRect.anchoredPosition;
        var robotAnchorMin = new Vector2(0.5f, 1f);
        var robotAnchorMax = new Vector2(0.5f, 1f);
        var robotPivot = new Vector2(0.5f, 0.5f);
        
        // ロボットプレハブを生成
        var roboPrefab = await Utils.InstantiatePrefab("Prefabs/Robo/RoboPrefab", chapterButtonParent);
        var roboRect = roboPrefab.GetComponent<RectTransform>();

        roboRect.anchorMin = robotAnchorMin;
        roboRect.anchorMax = robotAnchorMax;
        roboRect.pivot = robotPivot;
        
        roboRect.anchoredPosition = new Vector2(robotAnchoredPos.x, robotAnchoredPos.y - adjustedY);
        roboRect.localScale = new Vector3(0.5f, 0.5f, 1f);

        // ユーザーの選択したロボットデータを設定
        var userDataManager = UserDataManager.GetInstance();
        var userData = userDataManager.GetUserData();
        var selectedRoboId = userData.selectedRoboId ?? "default";

        var roboCustomDataDict = userDataManager.GetRoboCustomData(selectedRoboId);
        if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(selectedRoboId))
        {
            var roboPrefabComponent = roboPrefab.GetComponent<RoboPrefab>();
            if (roboPrefabComponent != null)
            {
                roboPrefabComponent.SetRobo(roboCustomDataDict[selectedRoboId]);
            }
        }
        
        foreach (var image in roboPrefab.GetComponentsInChildren<Image>())
        {
            image.raycastTarget = false;
        }
    }

    public void OnTappedGoToCustomSceneButton()
    {
        GoToCustomScene();
    }
    private void StartGame()
    {
        BgmClips.Stop();
        SceneManager.sceneLoaded += GameSceneLoaded;
        gameLoadingScene.LoadNextScene("GameScene", LoadSceneMode.Additive);
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        var gameObjects = next.GetRootGameObjects();
        foreach (var gameObject in gameObjects)
            if (gameObject.name == "Canvas")
            {
                var eventSystem = gameObject.GetComponentInChildren<EventSystem>();
                if (eventSystem != null) EventSystem.current = eventSystem;

                SceneManager.sceneLoaded -= GameSceneLoaded;
            }
    }

    private void GoToCustomScene()
    {
        SceneManager.sceneLoaded += CustomSceneLoaded;
        gameLoadingScene.LoadNextScene("CustomScene", LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }
    
    private void CustomSceneLoaded(Scene next, LoadSceneMode mode)
    {
        var gameObjects = next.GetRootGameObjects();
        foreach (var gameObject in gameObjects)
            if (gameObject.name == "Canvas")
            {
                var eventSystem = gameObject.GetComponentInChildren<EventSystem>();
                if (eventSystem != null) EventSystem.current = eventSystem;

                SceneManager.sceneLoaded -= CustomSceneLoaded;
            }
    }
    
    
}