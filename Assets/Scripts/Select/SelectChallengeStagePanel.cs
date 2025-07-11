using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectChallengeStagePanel : MonoBehaviour
{
    [SerializeField] private Text title, level;
    [SerializeField] private Transform chapterButtonParent;
    [SerializeField] private GameObject dropDownMenu;

    private Action _onStartGame;
    private ChapterData[] _chapters;
    private string _subject;
    
    private void Start()
    {
        UserDataManager.GetInstance().AddUserDataUpdateListener(OnUserDataUpdated);
    }

    private void OnDestroy()
    {
        UserDataManager.GetInstance().RemoveUserDataUpdateListener(OnUserDataUpdated);
    }

    private void OnUserDataUpdated()
    {
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject);
        }

        var challengeLevel = UserDataManager.GetInstance().GetChallengeLevel(Const.SUBJECT_NAME_MAP[_subject]);
        level.text = Const.DIFFICULTY_NAME_MAP[challengeLevel];
        _chapters = MasterData.GetInstance().GetChaptersBySubjectAndLevel(_subject, challengeLevel);
        SetChapterButtons();
    }

    
    public void Setup(string subject, Action startGame)
    {
        _subject = subject;
        title.text = _subject;
        var challengeLevel = UserDataManager.GetInstance().GetChallengeLevel(Const.SUBJECT_NAME_MAP[_subject]);
        level.text = Const.DIFFICULTY_NAME_MAP[challengeLevel];
        _onStartGame = startGame;
        _chapters = MasterData.GetInstance().GetChaptersBySubjectAndLevel(subject, challengeLevel);
        SetChapterButtons();
    }

    private void SetChapterButtons()
    {
        int chapterCount = _chapters.Length;

        float offsetX = 200f;            // 左右のずれ幅
        float verticalOffset = 250f;     // 上端からのオフセット

        for (int i = 0; i < chapterCount; i++)
        {
            var chapterData = _chapters[i];
            var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/ChapterStar"), chapterButtonParent);
            chapterButton.Setup(chapterData, ShowChallengeDialog);

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
        var options = new List<string>(Const.DIFFICULTY_NAME_MAP.Keys);
        dropDownMenu.GetComponent<DropDownMenu>().Setup(options, OnSelectLevel);
    }

    private async void OnSelectLevel(string challengeLevel)
    {
        await UserDataManager.GetInstance().SetChallengeLevel(Const.SUBJECT_NAME_MAP[_subject], challengeLevel);
        level.text = Const.DIFFICULTY_NAME_MAP[challengeLevel];
    }
    
    private async void ShowChallengeDialog(ChapterData data)
    {
        var go = await Genit.Utils.OpenDialog("Prefabs/Common/CommonDialog", this.gameObject.transform);
        var cd = go.GetComponent<CommonDialog>();
        var challengeTitle = $"{data.chapterNumber}にチャレンジする？";
        cd.Setup(challengeTitle, challengeTitle, result => 
        {
            if (result == CommonDialog.Result.OK)
            {
                _onStartGame();
            }
        }, CommonDialog.Mode.OK_CANCEL);
    }
    
    public void OnClickCloseButton()
    {
        Destroy(this.gameObject);
    }
}
