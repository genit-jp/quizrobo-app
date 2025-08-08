using System;
using UnityEngine.SceneManagement;

public class PauseDialog : DialogBaseListener
{
    private Action _onClickContinue;

    public override bool OnClickBlocker()
    {
        return false;
    }

    public void OnClickContinue()
    {
        Close();
        _onClickContinue();
    }

    public void OnClickQuit()
    {
        Close();
        AdManager.Instance.ShowInterstitialAd(() => SceneManager.LoadScene("SelectScene"));
    }

    public void Setup(Action onClickContinue)
    {
        _onClickContinue = onClickContinue;
    }
}