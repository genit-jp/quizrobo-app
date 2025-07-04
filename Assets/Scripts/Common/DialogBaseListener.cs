using UnityEngine;

public abstract class DialogBaseListener : MonoBehaviour
{
    public abstract bool OnClickBlocker();

    public void Close()
    {
        GetComponentInParent<DialogBase>().Close();
    }

    public void CloseNow()
    {
        GetComponentInParent<DialogBase>().CloseNow();
    }
}
