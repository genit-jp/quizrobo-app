using System;
using UnityEngine;

public class LocalPushManager : MonoBehaviour
{
    //Unityが用意した関数(名前は固定)
    //「アプリを開始した時」、「Homeボタンを押した時」、「Backボタンを押した時(ボタンはAndroidのみに存在)」、「OverViewボタンを押した時(ボタンはAndroidのみに存在)」に実行
    private void Start()
    {
        SettingPush(); //プッシュ通知の設定
        Debug.Log("プッシュ通知の設定完了");
    }


    //プッシュ通知の設定
    private void SettingPush()
    {
        //　Androidチャンネルの登録
        //LocalPushNotification.RegisterChannel(引数1,引数２,引数３);
        //引数１ Androidで使用するチャンネルID なんでもいい LocalPushNotification.AddSchedule()で使用する
        //引数2　チャンネルの名前　なんでもいい　アプリ名でも入れておく
        //引数3　通知の説明 なんでもいい　自分がわかる用に書いておくもの　
        LocalPushController.RegisterChannel("kids_quiz_local", "KidsQuizLocalPush", "キッズクイズのローカル通知");

        //通知のクリア
        LocalPushController.AllClear();

        // 現在の日時を取得
        var now = DateTime.Now;
        // 今日の16時のDateTimeを生成
        var notificationTime = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
        // 現在時刻が16時を過ぎている場合、次の日の16時に設定
        if (now > notificationTime) notificationTime = notificationTime.AddDays(1);

        // プッシュ通知の登録
        //LocalPushNotification.AddSchedule(引数１,引数2,引数3,引数4,引数5);
        //引数１ プッシュ通知のタイトル
        //引数2　通知メッセージ
        //引数3　表示するバッジの数(バッジ数はiOSのみ適用の様子 Androidで数値を入れても問題無い)
        //引数4　何秒後に表示させるか？
        //引数5　Androidで使用するチャンネルID　「Androidチャンネルの登録」で登録したチャンネルIDと合わせておく
        //注意　iOSは45秒経過後からしかプッシュ通知が表示されない        
        LocalPushController.AddSchedule("今日もたのしくクイズタイム！", "クイズ博士であそんじゃおう！", 1, notificationTime, "kids_quiz_local");
        Debug.Log("プッシュ通知の登録完了");
    }
}