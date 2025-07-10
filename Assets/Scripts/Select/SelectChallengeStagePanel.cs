using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectChallengeStagePanel : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Transform chapterButtonParent;
    [SerializeField] private GameObject dropDownMenu;
    
    
    private ChapterData[] _chapters;
    public void Setup(string subject)
    {
        title.text = subject;
        
        _chapters = MasterData.GetInstance().GetAvailableChaptersBySubject(subject);
        SetChapterButtons();
    }

    private void SetChapterButtons()
    {
        float panelHeight = ((RectTransform)this.transform).rect.height;
        int chapterCount = _chapters.Length;

        float offsetX = 200f;            // 左右のずれ幅
        float verticalOffset = 250f;     // 上端からのオフセット

        for (int i = 0; i < chapterCount; i++)
        {
            var chapterData = _chapters[i];
            var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/ChapterStar"), chapterButtonParent);
            chapterButton.Setup(chapterData, ShowChapterDialog);

            var rect = chapterButton.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            // ボタンの高さの半分を使って縦に等間隔に並べる
            float buttonHeight = rect.sizeDelta.y;
            float y = verticalOffset + i * (buttonHeight / 2f);
            float x = (i % 2 == 0) ? offsetX : -offsetX;

            rect.anchoredPosition = new Vector2(x, -y);
        }
    }
    
    public void OnClickLevelChangeButton()
    {
        dropDownMenu.gameObject.SetActive(true);

        // 選択肢を設定
        var options = new List<string> { "Easy", "Normal", "Hard" };
        dropDownMenu.GetComponent<DropDownMenu>().Setup(options, OnSelectLevel);
    }

    private void OnSelectLevel(string level)
    {
        Debug.Log($"選択されたレベル: {level}");
        // レベル変更の処理をここに追加
        // 例: GameManager.GetInstance().SetChallengeLevel(level);
    }
    
    private void ShowChapterDialog(ChapterData data)
    {
        Debug.Log($"チャプター {data.chapterNumber} を選択");
        // 任意でダイアログ表示を呼ぶ
        // e.g. DialogManager.Show(data);
    }
    public void OnClickCloseButton()
    {
        Destroy(this.gameObject);
    }
}
