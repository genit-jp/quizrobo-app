using System;
using System.Linq;
using UnityEngine;

public class CalculationStartDialog: DialogBaseListener
{
    [SerializeField] private GameObject _panelOnAvailable;
    [SerializeField] private GameObject _panelOnUnavailable;
    private Action _onClickStartButton;
    
    public void Setup(Action onClickStartButton)
    {
        _onClickStartButton = onClickStartButton;
        Debug.Log("IsPlayCalculationMode: " + CanPlayCalculationMode());
        if (CanPlayCalculationMode())
        {
            _panelOnAvailable.SetActive(true);
            _panelOnUnavailable.SetActive(false);
        }
        else
        {
            _panelOnAvailable.SetActive(false);
            _panelOnUnavailable.SetActive(true);
        }
    }
    
    public void OnClickStartButton()
    {
        _onClickStartButton?.Invoke();
    }
    
    private bool CanPlayCalculationMode()
    {
        DateTime today = Clock.GetInstance().Now().Date;
        var answerDataList = UserDataManager.GetInstance().GetAnswerDataList();
        
        return !answerDataList.Any(answerData =>
        {
            DateTime answerDate = Genit.Utils.UnixTimeToDateTime(answerData.dateTime).Date;
            return answerDate == today && answerData.playMode == Const.PlayMode.Calculation;
        });
    }
    
    
    public override bool OnClickBlocker()
    {
        return false;
    }
}