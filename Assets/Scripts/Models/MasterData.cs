using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MasterData
{
    private static MasterData _masterData;
    
    public QuizData[] quizzes;
    public bool IsPrepared { get; set; }
    
    public static MasterData GetInstance()
    {
        if (_masterData == null)
        {
            _masterData = new MasterData();
        }
        
        return _masterData;
    }
    
    public static async UniTask<MasterData> Fetch()
    {
        // リクエスト作成
        string masterVersion = FirebaseManager.Instance.GetRemoteConfigValue(Const.MasterVersion);
        Debug.Log(masterVersion);
        var request = UnityWebRequest.Get(Const.GetMasterJsonUrl(masterVersion));
        Debug.Log(Const.GetMasterJsonUrl(masterVersion));

        // リクエスト送信
        await request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("err");
            Debug.Log(request.error);
        }
        else
        {
            return await ParseText(request.downloadHandler.text);
        }

        return null;
    }
    
    public static async UniTask<MasterData> ParseText(string text)
    {
        // 受信したJSONを変換
        _masterData = JsonUtility.FromJson<MasterData>(text);
        _masterData.IsPrepared = true;
        return _masterData;
    }
    
    public string[] AllSubjects()
    {
        return quizzes.Select(quiz => quiz.subject).Distinct().ToArray();
    }
    
    
    public string[] AllQuizTags(){
        return quizzes.SelectMany(quiz => quiz.quizTags).Distinct().ToArray();
    }
    
    public string[] AllQuizTagsBySubject(string subject)
    {
        return quizzes.Where(quiz => quiz.subject == subject).SelectMany(quiz => quiz.quizTags).Distinct().ToArray();
    }
}