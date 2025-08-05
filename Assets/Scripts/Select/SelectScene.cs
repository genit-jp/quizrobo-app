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
    
    private ChapterStar _currentChapterStar;
    

    private int _selectedGrade;
    private TimeDispatcher _timer;

    private async void Start()
    {
#if UNITY_EDITOR //開発用
        if (MasterData.GetInstance().quizzes == null) SceneManager.LoadScene("TitleScene");
#endif

        var masterData = MasterData.GetInstance();
        var subjects = masterData.AllSubjects();
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
    }

    private void UpdatePlayerStatusUI()
    {
        var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
        int currentExp = playerStatus.exp;
        int level = LevelingSystem.CalculateLevelFromExp(currentExp);

        int expToCurrentLevel = 0;
        for (int i = 1; i < level; i++)
        {
            expToCurrentLevel += LevelingSystem.GetExpToLevelUp(i);
        }

        int expForNextLevel = LevelingSystem.GetExpToLevelUp(level);
        int expInCurrentLevel = currentExp - expToCurrentLevel;
        
    }
    
    private void OnUserDataUpdated()
    {
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject);
        }
        SetChapterButtons();
    }

    private async void SetChapterButtons()
    {
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject); // 既存のボタンを削除
        }
        

        // StartPoint を配置
        var startPointObj = await Utils.InstantiatePrefab("Prefabs/Select/SubjectSelect/StartPoint", chapterButtonParent);
        var startPointRect = startPointObj.GetComponent<RectTransform>();
        startPointRect.anchorMin = startPointRect.anchorMax = new Vector2(0.5f, 1f);
        startPointRect.pivot = new Vector2(0.5f, 1f);
        startPointRect.anchoredPosition = new Vector2(0, -300f);

        // enemy_data.txt を読み込み
        var enemyDataText = Resources.Load<TextAsset>("Data/enemy_data");
        if (enemyDataText == null)
        {
            Debug.LogError("enemy_data.txt not found");
            return;
        }

        string[] lines = enemyDataText.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int chapterCount = lines.Length;
        int maxClearedChapterNumber = UserDataManager.GetInstance().GetMaxChapterNumber("算数");

        float offsetX = 150f;
        float verticalOffset = 450f;

        for (int i = 0; i < chapterCount; i++)
        {
            int chapterNumber = i + 1;

            var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/SubjectSelect/ChapterStar"), chapterButtonParent);
            
            // chapterData を自前で構築（subject は固定で "算数"）
            ChapterData chapterData = new ChapterData
            {
                chapterNumber = chapterNumber,
                subject = "算数"
            };

            chapterButton.Setup(chapterData, ShowChallengeDialog, "算数");

            var rect = chapterButton.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            float buttonHeight = rect.sizeDelta.y * 0.7f;
            float y = verticalOffset + i * buttonHeight;
            float x = (i % 2 == 0) ? offsetX : -offsetX;

            rect.anchoredPosition = new Vector2(x, -y);

            if (chapterNumber == maxClearedChapterNumber)
            {
                _currentChapterStar = chapterButton;
            }
        }

        // chapterButtonParent の RectTransform を取得
        var contentRect = chapterButtonParent.GetComponent<RectTransform>();

        // 最後のボタンの位置を基に Content の高さを更新
        float totalHeight = verticalOffset + chapterCount * 100f; // 100f はボタンの高さとマージンの目安
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);

        
        PlaceRobotOnChapter();
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
    
    private async void ShowChallengeDialog(ChapterData data)
    {
        
        var go = await Genit.Utils.OpenDialog("Prefabs/Common/CommonDialog", this.gameObject.transform);
        var cd = go.GetComponent<CommonDialog>();
        var challengeTitle = $"{data.chapterNumber}にチャレンジする？";
        cd.Setup(challengeTitle, challengeTitle, async result => 
        {
            if (result == CommonDialog.Result.OK)
            {
                Const.GameSceneParam.Subject = data.subject;
                Const.GameSceneParam.DifficultyLevel = data.difficultyLevel;
                Const.GameSceneParam.ChapterNumber = data.chapterNumber;
                
                // ダイアログのアニメーションが完了するのを待つ
                await UniTask.Delay(150);
                
                StartGame();
            }
        }, CommonDialog.Mode.OK_CANCEL);
    }

    private async void PlaceRobotOnChapter()
    {
        RectTransform targetRect = null;
        float adjustedY = 0f;

        if (_currentChapterStar != null)
        {
            targetRect = _currentChapterStar.GetComponent<RectTransform>();
            adjustedY = 100f;
        }
        else
        {
            // StartPoint にロボを配置（最初に生成されていると仮定）
            foreach (Transform child in chapterButtonParent)
            {
                if (child.name.Contains("StartPoint"))
                {
                    targetRect = child.GetComponent<RectTransform>();
                    adjustedY = 0f;
                    break;
                }
            }

            if (targetRect == null)
            {
                Debug.LogWarning("StartPoint が見つかりませんでした");
                return;
            }
        }

        // ロボットプレハブを生成
        var roboPrefab = await Utils.InstantiatePrefab("Prefabs/Robo/RoboPrefab", chapterButtonParent);
        var roboRect = roboPrefab.GetComponent<RectTransform>();

        // アンカー・ピボット設定
        roboRect.anchorMin = roboRect.anchorMax = new Vector2(0.5f, 1f);
        roboRect.pivot = new Vector2(0.5f, 0.5f); // 下部中央を基準点に

        // ロボットの位置を targetRect の下に配置
        roboRect.anchoredPosition = new Vector2(
            targetRect.anchoredPosition.x,
            targetRect.anchoredPosition.y - adjustedY
        );

        // ロボットのサイズを調整（必要に応じて）
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
    }

    public void OnTappedGoToCustomSceneButton()
    {
        GoToCustomScene();
    }
    private void StartGame()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;
        gameLoadingScene.LoadNextScene("GameScene", LoadSceneMode.Additive);
        gameObject.SetActive(false);
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