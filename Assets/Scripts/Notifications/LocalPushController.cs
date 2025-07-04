using System;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif


// ローカルプッシュ通知送信クラス
public static class LocalPushController
{
    // Androidで使用するプッシュ通知用のチャンネルを登録する。    
    public static void RegisterChannel(string channelId, string title, string description)
    {
#if UNITY_ANDROID
        // チャンネルの登録
        var channel = new AndroidNotificationChannel
        {
            Id = channelId,
            Name = title,
            Importance = Importance.High,
            Description = description
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }


    /// 通知をすべてクリアーします。
    public static void AllClear()
    {
#if UNITY_ANDROID
        // Androidの通知をすべて削除します。
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllNotifications();
#endif
#if UNITY_IOS
        // iOSの通知をすべて削除します。
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        // バッジを消します。
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }


    // プッシュ通知を登録します。
    public static void AddSchedule(string title, string message, int badgeCount, DateTime notificationTime,
        string channelId)
    {
#if UNITY_ANDROID
        SetAndroidNotification(title, message, badgeCount, notificationTime, channelId);
#endif
#if UNITY_IOS
        SetIOSNotification(title, message, badgeCount, notificationTime);
#endif
    }


#if UNITY_IOS
    // 通知を登録(iOS)
    private static void SetIOSNotification(string title, string message, int badgeCount, DateTime notificationTime)
    {
        // 通知を作成
        iOSNotificationCenter.ScheduleNotification(new iOSNotification
        {
            //プッシュ通知を個別に取り消しなどをする場合はこのIdentifierを使用します。(未検証)
            Identifier = $"_notification_{badgeCount}",
            Title = title,
            Body = message,
            ShowInForeground = false,
            Badge = badgeCount,
            Trigger = new iOSNotificationCalendarTrigger
            {
                Year = notificationTime.Year,
                Month = notificationTime.Month,
                Day = notificationTime.Day,
                Hour = notificationTime.Hour,
                Minute = notificationTime.Minute,
                Second = 0,
                Repeats = false // この通知を繰り返す場合はtrueに設定
            }
        });
    }
#endif


#if UNITY_ANDROID
    // 通知を登録(Android)   
    private static void SetAndroidNotification(string title, string message, int badgeCount, DateTime notificationTime,
        string channelId)
    {
        // 通知を作成します。
        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            Number = badgeCount,

            //Androidのアイコンを設定
            SmallIcon = "icon_notify_small", //どの画像を使用するかアイコンのIdentifierを指定　指定したIdentifierが見つからない場合アプリアイコンになる。
            LargeIcon = "icon_notify_large", //どの画像を使用するかアイコンのIdentifierを指定　指定したIdentifierが見つからない場合アプリアイコンになる。
            FireTime = notificationTime
        };

        // 通知を送信します。
        AndroidNotificationCenter.SendNotification(notification, channelId);
    }
#endif
}