using System;
using System.Collections.Generic;
using com.adjust.sdk;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdjustComponent
{
    public enum EventType
    {
        FirstPlay,
        GetReward,
        Login,
        PurchaseSubscription,
        CompleteTutorial,
        ShowRewardedAd,
        StartGame
    }

    private const string AppToken = "oibl3od9wruo";

    private static readonly Dictionary<EventType, String> EventKeys =
        new()
        {
            { EventType.FirstPlay, "9969i0" },
            { EventType.GetReward, "oxdiod" },
            { EventType.Login, "14t90u" },
            { EventType.ShowRewardedAd, "45wwtx" },
            { EventType.StartGame, "fhmx0b" }
        };

    private static bool _isInitialized;

    public static void Initialize()
    {
        if (!_isInitialized)
        {
            var environment = Const.IsDebug ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production;

            var config = new AdjustConfig(AppToken, environment, true);
            config.setLogLevel(Const.IsDebug ? AdjustLogLevel.Debug : AdjustLogLevel.Warn);

            Adjust.start(config);
            Debug.Log("AdjustComponent.Start()");
            _isInitialized = true;
        }
    }

    public static void SendEvent(EventType eventType)
    {
        Debug.Log($"AdjustComponent.SendEvent({eventType})");
        Adjust.trackEvent(new AdjustEvent(EventKeys[eventType]));
    }

    public static void HandlePaidEvent(AdValue adValue)
    {
        var revenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        revenue.setRevenue(adValue.Value / 1000000.0, adValue.CurrencyCode);
        Adjust.trackAdRevenue(revenue);
    }
}
