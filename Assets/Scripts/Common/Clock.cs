using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Functions;
using UnityEngine;


public class Clock
{
    private static Clock _instance;
    private DateTime lastFetchTime;
    private bool updateNow;
    private float updateRealtimeSinceStartup = -1;

    private Clock()
    {
    }

    public static Clock GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Clock();
        }

        return _instance;
    }

    // public async UniTask<DateTime> Now()
    // {
    //     if ((updateRealtimeSinceStartup < 0 ||
    //          Time.realtimeSinceStartup - updateRealtimeSinceStartup > 60.0f * 5.0f) && !updateNow)
    //     {
    //         updateNow = true;
    //         try
    //         {
    //             // // リクエスト作成
    //             var functions =
    //                 FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance, "asia-northeast1");
    //             var function = functions.GetHttpsCallable("now");
    //             var now = await function.CallAsync().ContinueWith(task => { return task.Result.Data; });
    //             
    //             var now = DateTime.Now;
    //             lastFetchTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(now.ToString()))
    //                 .LocalDateTime;
    //             Debug.Log($"lastFetchTime{lastFetchTime.ToLongTimeString()}");
    //             updateRealtimeSinceStartup = Time.realtimeSinceStartup;
    //             updateNow = false;
    //         }
    //         catch (Exception exception)
    //         {
    //             Debug.LogError(exception);
    //             lastFetchTime = new DateTime().ToLocalTime();
    //         }
    //     }
    //
    //     return lastFetchTime.AddSeconds(Time.realtimeSinceStartup - updateRealtimeSinceStartup);
    // }
    
    public DateTime Now()
    {
        return DateTime.Now;
    }
}
