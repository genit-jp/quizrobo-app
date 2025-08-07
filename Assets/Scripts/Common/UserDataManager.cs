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
    public static readonly string USER_DATA_KEY_CHALLENGE_LEVEL = "challengeLevel";
    public static readonly string USER_DATA_KEY_SELECTED_ROBO_ID = "selectedRoboId";
    public static readonly string USER_DATA_KEY_PLAYER_STATUS = "playerStatus";
    private static UserDataManager _instance;

    private readonly List<Action> _userDataActions = new();
    private readonly List<Action> _chapterProgressDataActions = new();
    private readonly List<Action> _roboCustomDataActions = new();
    private readonly List<Action> _ownedRoboPartsUpdateActions = new();
    private readonly Dictionary<string, RoboCustomData> _roboCustomData = new Dictionary<string, RoboCustomData>();
    private readonly Dictionary<string, ChapterProgressData> _chapterProgressData = new Dictionary<string, ChapterProgressData>();
    private readonly Dictionary<string, OwnedRoboPart> ownedRoboPartsMeta = new();
    private DocumentSnapshot _userData;
    private SynchronizationContext mainThread;
    
    public List<string> ownedRoboPartsIds { get; } = new List<string>();

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
        await SetDefaultValue(USER_DATA_KEY_CHALLENGE_LEVEL, 1);
        await SetDefaultValue(USER_DATA_KEY_SELECTED_ROBO_ID, "default");
        await SetDefaultValue(USER_DATA_KEY_PLAYER_STATUS, new PlayerStatus());
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
        FetchRoboCustomDataList();
        FetchChapterProgressData();
        FetchOwnedRoboPartsIds();

        return _userData;
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
        
        // リスナーを設定
        FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("roboCustomData")
            .Listen(snapshot =>
            {
                _roboCustomData.Clear();
                foreach (var documentSnapshot in snapshot.Documents)
                {
                    var roboData = documentSnapshot.ConvertTo<RoboCustomData>();
                    _roboCustomData[documentSnapshot.Id] = roboData;
                }
                
                // RoboCustomData更新リスナーを呼び出す
                mainThread.Post(__ =>
                {
                    foreach (var action in _roboCustomDataActions) action();
                }, null);
            });
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
    
    private async void FetchOwnedRoboPartsIds()
    {
        var snapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("ownedRoboPartsIds")
            .GetSnapshotAsync();

        ownedRoboPartsIds.Clear();
        ownedRoboPartsMeta.Clear();

        foreach (var documentSnapshot in snapshot.Documents)
        {
            bool isPurchased = false;

            if (documentSnapshot.ContainsField("purchased"))
                isPurchased = documentSnapshot.GetValue<bool>("purchased");

            var partId = documentSnapshot.Id;

            // メタ情報にすべて追加
            ownedRoboPartsMeta[partId] = new OwnedRoboPart { purchased = isPurchased };

            // true のみ別途 ID リストに保持
            if (isPurchased)
                ownedRoboPartsIds.Add(partId);
        }

        // 初回デフォルト（すべて true 扱いで追加）
        if (ownedRoboPartsIds.Count == 0)
        {
            var defaultParts = new List<string> { "default_head", "default_body", "default_arms", "default_legs", "default_tail" };
            foreach (var partId in defaultParts)
            {
                await AddOwnedRoboPart(partId, true);
            }
        }
    }

    
    public async UniTask AddOwnedRoboPart(string partId, bool forcedAdd)
    {
        bool shouldNotify = false;

        // すでにメタ情報に存在するかチェック（false → true 更新の可能性）
        if (ownedRoboPartsMeta.ContainsKey(partId))
        {
            var meta = ownedRoboPartsMeta[partId];
            if (!meta.purchased && forcedAdd)
            {
                // purchased: false → true に変更
                meta.purchased = true;
                if (!ownedRoboPartsIds.Contains(partId))
                    ownedRoboPartsIds.Add(partId);

                shouldNotify = true;
            }
        }
        else
        {
            // 初めてのパーツ → 追加＆通知
            ownedRoboPartsMeta[partId] = new OwnedRoboPart { purchased = forcedAdd };
            if (forcedAdd)
            {
                ownedRoboPartsIds.Add(partId);
                shouldNotify = true;
            }
        }

        // Firestoreに保存（上書き or 新規）
        await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("ownedRoboPartsIds")
            .Document(partId)
            .SetAsync(new Dictionary<string, object> { { "purchased", forcedAdd } });

        // リスナー通知
        if (shouldNotify)
        {
            mainThread?.Post(_ =>
            {
                foreach (var action in _ownedRoboPartsUpdateActions) action();
            }, null);
        }
    }

    
    public bool IsRoboPartOwned(string partId)
    {
        return ownedRoboPartsIds.Contains(partId);
    }

    public List<string> OwnedRoboPartsIds()
    {
        return ownedRoboPartsMeta
            .Where(kvp => kvp.Value.purchased)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    public List<string> GetUnclaimedRewardRoboPartIds(int currentExp)
    {
        return ownedRoboPartsMeta
            .Where(kvp =>
                MasterData.GetInstance().robos.Any(robo =>
                    robo.id == kvp.Key &&
                    currentExp >= robo.exp_required &&
                    kvp.Value.purchased == false
                ))
            .Select(kvp => kvp.Key)
            .ToList();
    }

    
    private async void FetchChapterProgressData()
    {
        var snapshot = await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("chapterProgress")
            .GetSnapshotAsync();

        _chapterProgressData.Clear();
        foreach (var documentSnapshot in snapshot.Documents)
        {
            var progressData = documentSnapshot.ConvertTo<ChapterProgressData>();
            string subjectName = documentSnapshot.Id; // ← ドキュメントIDをsubject名とする

            if (!string.IsNullOrEmpty(subjectName))
            {
                _chapterProgressData[subjectName] = progressData;
            }
        }

        // ChapterProgressData更新リスナーを呼び出す
        mainThread.Post(__ =>
        {
            foreach (var action in _chapterProgressDataActions) action();
        }, null);
    }


    public async UniTask SaveChapterProgress(ChapterProgressData progressData)
    {
        // ランダムなドキュメントIDを生成
        var documentId = Utils.GenerateRandomString(20);
        
        await FirebaseFirestore.DefaultInstance
            .Collection("users")
            .Document(UserId)
            .Collection("chapterProgress")
            .Document(documentId)
            .SetAsync(progressData);
        
        // ChapterProgressData更新リスナーを呼び出す
        mainThread.Post(__ =>
        {
            foreach (var action in _chapterProgressDataActions) action();
        }, null);
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
    
    public void AddChapterProgressDataUpdateListener(Action action)
    {
        if (!_chapterProgressDataActions.Contains(action)) _chapterProgressDataActions.Add(action);
        
        action();
    }
    
    public void RemoveChapterProgressDataUpdateListener(Action action)
    {
        if (_chapterProgressDataActions.Contains(action)) _chapterProgressDataActions.Remove(action);
    }
    
    public void AddRoboCustomDataUpdateListener(Action action)
    {
        if (!_roboCustomDataActions.Contains(action)) _roboCustomDataActions.Add(action);
        
        action();
    }
    
    public void RemoveRoboCustomDataUpdateListener(Action action)
    {
        if (_roboCustomDataActions.Contains(action)) _roboCustomDataActions.Remove(action);
    }
    
    public void AddOwnedRoboPartsUpdateListener(Action action)
    {
        if (!_ownedRoboPartsUpdateActions.Contains(action)) _ownedRoboPartsUpdateActions.Add(action);
        
        action();
    }
    
    public void RemoveOwnedRoboPartsUpdateListener(Action action)
    {
        if (_ownedRoboPartsUpdateActions.Contains(action)) _ownedRoboPartsUpdateActions.Remove(action);
    }

    public UserData GetUserData()
    {
        return _userData.ConvertTo<UserData>();
    }


    public async UniTask SetUserData(Dictionary<string, object> data)
    {
        await _userData.Reference.SetAsync(data, SetOptions.MergeAll);
        await FetchUserData();
    }

    public async UniTask SetUserData<Type>(string key, Type value)
    {
        var data = new Dictionary<string, Type> { { key, value } };
        await _userData.Reference.SetAsync(data, SetOptions.MergeAll);
        await FetchUserData();
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
    
    public async UniTask SetChallengeLevel(int level)
    {
        await SetUserData(USER_DATA_KEY_CHALLENGE_LEVEL, level);
    }

    public int GetChallengeLevel()
    {
        var userData = GetUserData();
        return userData.challengeLevel;
    }
    
    public async UniTask UpdatePlayerStatus(PlayerStatus playerStatus)
    {
        await SetUserData(USER_DATA_KEY_PLAYER_STATUS, playerStatus);
    }
    
    public PlayerStatus GetPlayerStatus()
    {
        var userData = GetUserData();
        return userData.playerStatus ?? new PlayerStatus();
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
    
    [FirestoreData]
    public class OwnedRoboPart
    {
        [FirestoreProperty] public bool purchased { get; set; }
    }
    
    [FirestoreData]
    public class PlayerStatus
    {
        [FirestoreProperty] public int exp { get; set; } = 0;
        
        [FirestoreProperty] public int level { get; set; } = 1;
    }
    
    // ChapterDataクラスの定義
    [FirestoreData]
    public class ChapterProgressData
    {
        [FirestoreProperty] public string chapterId { get; set; }

        [FirestoreProperty] public long dateTime { get; set; }

        [FirestoreProperty] public int correctCount { get; set; }
        
        [FirestoreProperty] public int totalCount { get; set; }
    }

    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty] public string playerName { get; set; }
        [FirestoreProperty] public int grade { get; set; }

        [FirestoreProperty] public int stage { get; set; }
        
        [FirestoreProperty] public long createdAt { get; set; }

        [FirestoreProperty] public int rating { get; set; }
        [FirestoreProperty] public bool isPlayedTutorial { get; set; }
        [FirestoreProperty] public long lastLoginDateTime { get; set; }
        [FirestoreProperty] public int consecutiveLoginNum { get; set; }
        [FirestoreProperty] public int totalLoginNum { get; set; }
        
        [FirestoreProperty] public int challengeLevel { get; set; } = 1; 
        
        [FirestoreProperty] public string selectedRoboId { get; set; } 
        
        [FirestoreProperty] public PlayerStatus playerStatus { get; set; } = new();

    }
}