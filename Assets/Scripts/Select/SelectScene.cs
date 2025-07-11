using Genit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectScene : MonoBehaviour
{
    [SerializeField] private GameLoadingPanel gameLoadingScene;

    [SerializeField] private GameObject _blocker;

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
    }

    private async void OnEnable()
    {
        // UserDataManager.GetInstance().AddUserDataUpdateListener(UpdateSelectSceneUI);

        if (UserDataManager.GetInstance().GetUserData().totalMedal <= 0)
            await UserDataManager.GetInstance().SetUserData(UserDataManager.USER_DATA_KEY_TOTAL_MEDAL, 0);

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

        _blocker.SetActive(false);
    }

    private void OnDisable()
    {
        // UserDataManager.GetInstance().RemoveUserDataUpdateListener(UpdateSelectSceneUI);
    }

    public async void OnClickSubjectSelectPanelButton()
    {
        var selectSubject = await Utils.InstantiatePrefab("Prefabs/Select/SubjectSelect/SubjectSelectPanel", this.transform);
        var selectSubjectPanel = selectSubject.GetComponent<SubjectSelectPanel>();
        selectSubjectPanel.Setup();
        selectSubjectPanel.StartGame = StartGame;
        // QuizSelectManager.GetInstance().SetSelectQuizzes(_selectedGrade, 10, Const.PlayMode.Normal);
    }

    public async void OnClickCategoryButton()
    {
        Vector4 blockerColor = new Color(255f / 255f, 246f / 255f, 230f / 255f, 1.0f);
        var categoryDialogObj = await Utils.OpenDialog("Prefabs/Select/CategoryDialog", transform, blockerColor);
        var categoryDialog = categoryDialogObj.GetComponent<CategoryDialog>();
        categoryDialog.Setup(0, (grade, subject) =>
        {
            Debug.Log("grade: " + grade + " subject: " + subject);
            // QuizSelectManager.GetInstance().SetSelectQuizzes(grade, 10, Const.PlayMode.Subject, subject);
            StartGame();
        });
    }

    public async void OnClickCalculationButton()
    {
        var calculationStartDialogObj = await Utils.OpenDialog("Prefabs/Select/CalculationStartDialog", transform);
        var calculationStartDialog = calculationStartDialogObj.GetComponent<CalculationStartDialog>();
        calculationStartDialog.Setup(async () =>
        {
            calculationStartDialog.CloseNow();
            // QuizSelectManager.GetInstance().SetSelectQuizzes(_selectedGrade, 10, Const.PlayMode.Calculation);
            StartGame();
        });
    }

    public async void OnClickMedalDescriptionButton()
    {
        await Utils.OpenDialog("Prefabs/Select/ContinueDialog", transform);
    }

    // public async void OnClickRankingButton()
    // {
    //     var rankingDialogObj = await Utils.OpenDialog("Prefabs/Select/RankingDialog", transform);
    //     var rankingDialog = rankingDialogObj.GetComponent<RankingDialog>();
    //     rankingDialog.Setup();
    // }

    public void OnTappedGoToLaboButton()
    {
        GoToLabo();
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

    private void GoToLabo()
    {
        SceneManager.sceneLoaded += LaboSceneLoaded;
        // gameLoadingScene.LoadNextScene("LaboScene", LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }
    
    private void LaboSceneLoaded(Scene next, LoadSceneMode mode)
    {
        var gameObjects = next.GetRootGameObjects();
        foreach (var gameObject in gameObjects)
            if (gameObject.name == "Canvas")
            {
                var eventSystem = gameObject.GetComponentInChildren<EventSystem>();
                if (eventSystem != null) EventSystem.current = eventSystem;

                SceneManager.sceneLoaded -= LaboSceneLoaded;
            }
    }
}