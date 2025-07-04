using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CommonDialog : DialogBaseListener
{
    public enum Mode
    {
        OK,
        OK_CANCEL
    }

    #region 列挙型

    public enum Result
    {
        OK,
        Cancel
    }

    #endregion

    [SerializeField] private Text _title;
    [SerializeField] private Text _text;
    [SerializeField] private Button _cancel;


    public string text
    {
        set => _text.text = value;
    }

    public string title
    {
        set => _title.text = value;
    }

    #region プロパティ

    /// <summary>ダイアログが操作されたときに発生するイベント</summary>
    public Action<Result> OnClose { get; set; }

    #endregion

    public static async UniTask Open(Transform parent, string title, string text, Action<Result> onClose,
        Mode mode = Mode.OK)
    {
        var go = await Genit.Utils.OpenDialog("Prefabs/Common/CommonDialog", parent);
        var cd = go.GetComponent<CommonDialog>();
        cd.Setup(title, text, onClose, mode);
    }

    private void Setup(string title, string text, Action<Result> onClose, Mode mode)
    {
        this.title = title;
        this.text = text;
        OnClose = onClose;
        _cancel.gameObject.SetActive(mode == Mode.OK_CANCEL);
    }


    /// <summary>
    ///     OKボタンが押されたとき
    /// </summary>
    public void OnClickOK()
    {
        OnClose?.Invoke(Result.OK);
        Close();
    }

    /// <summary>
    ///     Cancelボタンが押されたとき/ダイアログの外側をクリックした時
    /// </summary>
    public void OnClickCancel()
    {
        // イベント通知先があれば通知してダイアログを破棄
        OnClose?.Invoke(Result.Cancel);
        Close();
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}
