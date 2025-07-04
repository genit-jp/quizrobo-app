using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNameDialog : DialogBaseListener
{
    [SerializeField] private TMP_InputField _inputField;
    
    private Action _onOkButtonClicked;

    public void Setup(Action onOkButtonClicked, UserDataManager.UserData userData)
    {
        _onOkButtonClicked = onOkButtonClicked;
        
        if(!String.IsNullOrEmpty(userData.playerName))
        {
            _inputField.text = userData.playerName;
        }
        else
        {
            _inputField.text = "ゲスト";
        }
    }
    
    public async void OnChangedInputField()
    {
        // _inputField.text = _inputField.text.Substring(0, 8);
        if (_inputField.text.Length > 8)
        {
            _inputField.text = _inputField.text[..8];
        }
    }

    public async void OnClickOk()
    {
        await UserDataManager.GetInstance().SetUserData(UserDataManager.USER_DATA_KEY_PLAYER_NAME, _inputField.text);
        _onOkButtonClicked?.Invoke();
        Close();
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}