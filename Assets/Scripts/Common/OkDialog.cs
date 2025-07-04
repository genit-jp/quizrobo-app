using System;

public class OkDialog : DialogBaseListener
{
    private Action _onClickOk;

    public void Setup(Action onClickOk)
    {
        _onClickOk = onClickOk;
    }

    public void OnClickOk()
    {
        _onClickOk?.Invoke();
        Close();
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}