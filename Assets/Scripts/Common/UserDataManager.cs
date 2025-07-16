using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Genit;
using UnityEngine;

public class UserDataManager
{
    // 上記のclass UserData変数名と一致させること
    public static readonly string USER_DATA_KEY_PLAYER_NAME = "playerName";
    public static readonly string USER_DATA_KEY_GRADE = "grade";
    public static readonly string USER_DATA_KEY_TOTAL_MEDAL = "totalMedal";
    public static readonly string USER_DATA_KEY_MAX_TOTAL_MEDAL = "maxTotalMedal";
    public static readonly string USER_DATA_KEY_STAGE = "stage";
    public static readonly string USER_DATA_KEY_RATING = "rating";
    public static readonly string USER_DATA_KEY_CREATED_AT = "createdAt";
    public static readonly string USER_DATA_KEY_IS_PLAYED_TUTORIAL = "isPlayedTutorial";
    public static readonly string USER_DATA_KEY_LAST_LOGIN_DATE = "lastLoginDateTime";
    public static readonly string USER_DATA_KEY_CONSECUTIVE_LOGIN_NUM = "consecutiveLoginNum";
    public static readonly string USER_DATA_KEY_TOTAL_LOGIN_NUM = "totalLoginNum";
    public static readonly string USER_DATA_KEY_CHALLENGE_LEVELS_KEY = "challengeLevels";
    private static UserDataManager _instance;

    private readonly List<Action> _userDataActions = new();
    private List<AnswerData> _answerDataList = new();
    private readonly Dictionary<string, RoboCustomData> _roboCustomData = new Dictionary<string, RoboCustomData>();
    private DocumentSnapshot _userData;
    private SynchronizationContext mainThread;

    public string UserId => FirebaseManager.Instance.Auth.CurrentUser.UserId;
    public DateTime TimeLimit { get; set; }

    private async void SetDefaultValue()
    {
        await SetDefaultValue(USER_DATA_KEY_PLAYER_NAME, "");
        await SetDefaultValue(USER_DATA_KEY_GRADE, 0);
        await SetDefaultValue(USER_DATA_KEY_TOTAL_MEDAL, 0);
        await SetDefaultValue(USER_DATA_KEY_MAX_TOTAL_MEDAL, 0);
        await SetDefaultValue(USER_DATA_KEY_STAGE, 1);
        await SetDefaultValue(USER_DATA_KEY_RATING, 0);
        await SetDefaultValue(USER_DATA_KEY_IS_PLAYED_TUTORIAL, false);
        await SetDefaultValue(USER_DATA_KEY_LAST_LOGIN_DATE, 0);
        await SetDefaultValue(USER_DATA_KEY_CONSECUTIVE_LOGIN_NUM, 0);
        await SetDefaultValue(USER_DATA_KEY_TOTAL_LOGIN_NUM, 0);
        await SetDefaultDictionaryValue(USER_DATA_KEY_CHALLENGE_LEVELS_KEY, Const.DEFAULT_CHALLENGE_LEVELS);
    }


    // Singleton Pattern
    public static UserDataManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new UserDataManager();
            _instance.mainThread = SynchronizationContext.Current;
        }

        return _instance;
    }

    public async UniTask<DocumentSnapshot> FetchUserData()
    {
        _userData = await FirebaseFirestore.DefaultInstance.Collection("users").Document(UserId).GetSnapshotAsync();
        if (!_userData.Exists)
        {
            var now = Clock.GetInstance().Now();
            await SetUserData(USER_DATA_KEY_CREATED_AT, Utils.DateTimeToUnixTime(now));
        }

        FirebaseFirestore.DefaultInstance.Collection("users").Document(UserId).Listen(snapshot =>
        {
            Debug.Log("UserDataManager: Listen");
            _userData = snapshot;
            SetDefaultValue();
            mainThread.Post(__ =>
            {
                foreach (var action in _userDataActions) action();
            }, null);
        });
        FetchAnswerDataList();
        FetchRoboCustomDataList();

        return _userData;
    }

    private async void FetchAnswerDataList()
    {
        var answerDataList = new List<AnswerData>();
        var snapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("answers")
            .GetSnapshotAsync();

        foreach (var documentSnapshot in snapshot.Documents)
            answerDataList.Add(documentSnapshot.ConvertTo<AnswerData>());
        _answerDataList = answerDataList;
    }

    private async void FetchRoboCustomDataList()
    {
        var roboCustomDataDict = new Dictionary<string, RoboCustomData>();
        var snapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("roboCustomData")
            .GetSnapshotAsync();

        foreach (var documentSnapshot in snapshot.Documents)
        {
            var roboData = documentSnapshot.ConvertTo<RoboCustomData>();
            roboCustomDataDict[documentSnapshot.Id] = roboData;
        }
        _roboCustomData.Clear();
        foreach (var kvp in roboCustomDataDict)
        {
            _roboCustomData[kvp.Key] = kvp.Value;
        }
        
        if (_roboCustomData.Count == 0)
        {
            var defaultRoboData = new RoboCustomData
            {
                headId = "default_head",
                bodyId = "default_body",
                armsId = "default_arms",
                legsId = "default_legs",
                tailId = "default_tail"
            };
        
            await SaveRoboCustomData("default", defaultRoboData);
        }
    }

    public List<AnswerData> GetAnswerDataList()
    {
        return _answerDataList;
    }

    public Dictionary<string, RoboCustomData> GetRoboCustomData(string roboId)
    {
        if (_roboCustomData.ContainsKey(roboId))
        {
            return new Dictionary<string, RoboCustomData> { { roboId, _roboCustomData[roboId] } };
        }
        else
        {
            Debug.LogWarning($"RoboCustomData not found for roboId: {roboId}");
            return new Dictionary<string, RoboCustomData>();
        }
    }

    public async UniTask SaveRoboCustomData(string roboId, RoboCustomData roboData)
    {
        await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("roboCustomData")
            .Document(roboId)
            .SetAsync(roboData);
        
        _roboCustomData[roboId] = roboData;
    }

    public void AddUserDataUpdateListener(Action action)
    {
        if (!_userDataActions.Contains(action)) _userDataActions.Add(action);

        action();
    }

    public void RemoveUserDataUpdateListener(Action action)
    {
        if (_userDataActions.Contains(action)) _userDataActions.Remove(action);
    }

    public UserData GetUserData()
    {
        return _userData.ConvertTo<UserData>();
    }


    public async UniTask SetUserData(Dictionary<string, object> data)
    {
        await _userData.Reference.SetAsync(data, SetOptions.MergeAll);
        await FetchUserData();
        await ConfirmMaxTotalMedal();
    }

    public async UniTask SetUserData<Type>(string key, Type value)
    {
        var data = new Dictionary<string, Type> { { key, value } };
        await _userData.Reference.SetAsync(data, SetOptions.MergeAll);
        await FetchUserData();
        await ConfirmMaxTotalMedal();
    }

    public async UniTask<List<UserData>> GetRankingUserData()
    {
        var querySnapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .OrderByDescending("totalMedal") // 降順にソート
            .Limit(300)
            .GetSnapshotAsync();

        var rankingData = querySnapshot.Documents
            .Select(doc => doc.ConvertTo<UserData>())
            .ToList();

        // デバッグログの出力
        Debug.Log($"Fetched {rankingData.Count} ranking entries.");
        foreach (var userData in rankingData)
            Debug.Log($"Ranking - Grade: {userData.grade}, Total Medals: {userData.totalMedal}");

        return rankingData;
    }

    private async UniTask SetDefaultValue<Type>(string key, Type defaultValue)
    {
        if (!_userData.ContainsField(key)) await SetUserData(key, defaultValue);
    }

    private async UniTask SetDefaultDictionaryValue(string key, Dictionary<string, string> defaultValue)
    {
        if (!_userData.ContainsField(key))
        {
            await SetUserData(key, defaultValue);
        }
    }

    public Type GetUserDataValue<Type>(string key, Type defaultValue)
    {
        if (_userData.ContainsField(key)) return _userData.GetValue<Type>(key);

        return defaultValue;
    }
    
    public async UniTask SetChallengeLevel(string subject, string level)
    {
        var userData = GetUserData();
        var challengeLevels = userData.challengeLevels ?? new Dictionary<string, string>();
        
        challengeLevels[subject] = level;
        
        await SetUserData(USER_DATA_KEY_CHALLENGE_LEVELS_KEY, challengeLevels);
    }

    public string GetChallengeLevel(string subject)
    {
        var userData = GetUserData();
        return userData.challengeLevels[subject];
    }


    // 新しい回答データを追加するメソッド
    public async UniTask AddUserAnswerData(AnswerData answer)
    {
        _userData = await FirebaseFirestore.DefaultInstance.Collection("users").Document(UserId).GetSnapshotAsync();

        // DatetimeとQuestionIdを使用してドキュメントIDを生成
        var documentId = $"{Utils.DateTimeToUnixTime(Clock.GetInstance().Now())}-{answer.quizId}";

        await FirebaseFirestore.DefaultInstance
            .Collection("users").Document(UserId)
            .Collection("answers")
            .Document(documentId)
            .SetAsync(answer);

        FetchAnswerDataList();
    }

    private async UniTask ConfirmMaxTotalMedal()
    {
        var totalMedal = GetUserData().totalMedal;
        var maxTotalMedal = GetUserData().maxTotalMedal;
        if (totalMedal > maxTotalMedal) await SetUserData(USER_DATA_KEY_MAX_TOTAL_MEDAL, totalMedal);
    }

    [FirestoreData]
    public class RoboCustomData
    { 
        [FirestoreProperty] public string headId { get; set; }
        [FirestoreProperty] public string bodyId { get; set; }
        [FirestoreProperty] public string armsId { get; set; }
        [FirestoreProperty] public string legsId { get; set; }
        [FirestoreProperty] public string tailId { get; set; }
    }
    
    // AnswerDataクラスの定義
    [FirestoreData]
    public class AnswerData
    {
        [FirestoreProperty] public string onePlayId { get; set; }

        [FirestoreProperty] public Const.PlayMode playMode { get; set; }

        [FirestoreProperty] public string quizId { get; set; }

        [FirestoreProperty] public long dateTime { get; set; }

        [FirestoreProperty] public bool isCorrect { get; set; }

        [FirestoreProperty] public int medalNum { get; set; }

        [FirestoreProperty] public string answerWord { get; set; }
    }

    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty] public string playerName { get; set; }
        [FirestoreProperty] public int grade { get; set; }

        [FirestoreProperty] public int totalMedal { get; set; }

        [FirestoreProperty] public int maxTotalMedal { get; set; }

        [FirestoreProperty] public int stage { get; set; }
        
        [FirestoreProperty] public long createdAt { get; set; }

        [FirestoreProperty] public int rating { get; set; }
        [FirestoreProperty] public bool isPlayedTutorial { get; set; }
        [FirestoreProperty] public long lastLoginDateTime { get; set; }
        [FirestoreProperty] public int consecutiveLoginNum { get; set; }
        [FirestoreProperty] public int totalLoginNum { get; set; }
        [FirestoreProperty] public List<AnswerData> answers { get; set; }
        
        [FirestoreProperty] public Dictionary<string, string> challengeLevels { get; set; } = new();
    }
}