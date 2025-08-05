using System;
using System.Collections.Generic;
using Balaso;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RubyTextMeshProUGUI _tapToStartText;
    [SerializeField] private RubyTextMeshProUGUI _loadingText;
    [SerializeField] private Text _UserIdText;
    [SerializeField] private Text _versionText;
    private bool _isFetchComplete;
    private FirebaseAuth auth;

    private async void Start()
    {
        // データの取得
        await MasterData.Fetch();

#if UNITY_IOS
        AppTrackingTransparency.RequestTrackingAuthorization();
#endif

        AdjustComponent.Initialize();
        AdManager.Instance.Initialize();

        // Firebaseの初期化とRemoteConfigの取得
        var defaultRemoteConfigs = new Dictionary<string, object>();
        defaultRemoteConfigs.Add(Const.MasterVersion, "0.6.0");
        await FirebaseManager.Instance.Initialize(defaultRemoteConfigs);
        var firebaseUser = await FirebaseManager.Instance.LoginAsAnonymous();

        // ユーザーデータの取得
        var userDataManager = UserDataManager.GetInstance();
        var userData = await userDataManager.FetchUserData();

        _UserIdText.text = "UID:" + userDataManager.UserId;
        Debug.Log(UserDataManager.USER_DATA_KEY_STAGE.GetType());
        Debug.Log(userDataManager.GetUserDataValue(UserDataManager.USER_DATA_KEY_STAGE, 1));

        var nowTime = Clock.GetInstance().Now();
        var timeLimit = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0);
        if (nowTime >= timeLimit) timeLimit = timeLimit.AddDays(1);

        UserDataManager.GetInstance().TimeLimit = timeLimit;

        _isFetchComplete = true;
        _loadingText.gameObject.SetActive(false);
        _tapToStartText.gameObject.SetActive(true);
        
        // Resources フォルダからタイムスタンプファイルを読み込み
        TextAsset timeStampAsset = Resources.Load<TextAsset>("BuildNumber");

        if (timeStampAsset != null)
        {
            this._versionText.text = $"version: {Application.version}({timeStampAsset.text})";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isFetchComplete) SceneManager.LoadScene("SelectScene");
    }
}