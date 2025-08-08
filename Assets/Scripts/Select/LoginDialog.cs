using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoginDialog : DialogBaseListener
{
    [SerializeField] private RubyTextMeshProUGUI _consecutiveLoginNumText;
    [SerializeField] private RubyTextMeshProUGUI _totalLoginNumText;
    [SerializeField] private RubyTextMeshProUGUI _commentText;
    
    public void Setup()
    {
        var consecutiveLoginNum = UserDataManager.GetInstance().GetUserData().consecutiveLoginNum;
        var totalLoginNum = UserDataManager.GetInstance().GetUserData().totalLoginNum;
        _consecutiveLoginNumText.uneditedText = consecutiveLoginNum.ToString() + "日目";
        _totalLoginNumText.uneditedText = "ごうけいログイン "+ totalLoginNum.ToString() + "日目";
        if(consecutiveLoginNum == 1)
        {
            _commentText.uneditedText = "<r=まいにち>毎日</r>あそんでロボをゲットしよう！";
        }
        else
        {
            _commentText.uneditedText = "おかえりなさい！今日もがんばろう！！";
        }
    }

    public void OnClickOk()
    {
        Close();
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}