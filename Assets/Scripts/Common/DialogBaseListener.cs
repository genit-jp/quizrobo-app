using UnityEngine;

public abstract class DialogBaseListener : MonoBehaviour
{
    public abstract bool OnClickBlocker();

    public void Close()
    {
        var baseDialog = GetComponentInParent<DialogBase>();
        if (baseDialog != null)
        {
            baseDialog.Close();
        }
        else
        {
            Debug.LogWarning("DialogBase が親に存在しません。自分を削除します。");
            Destroy(this.gameObject);
        }
    }

    public void CloseNow()
    {
        GetComponentInParent<DialogBase>().CloseNow();
    }
}
