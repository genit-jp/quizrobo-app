using System;
using System.Collections;
using System.Collections.Generic;
using Genit;
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
    private ChapterStar _currentChapterStar;
    
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
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject); // 既存のボタンを削除
        }

        int currentChapter = 2;
        int chapterCount = _chapters.Length;
        
        float offsetX = 200f;            // 左右のずれ幅
        float verticalOffset = 250f;     // 上端からのオフセット

        for (int i = 0; i < chapterCount; i++)
        {
            var chapterData = _chapters[i];
            var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/SubjectSelect/ChapterStar"), chapterButtonParent);
            chapterButton.Setup(chapterData, ShowChallengeDialog, _subject);
            
            var rect = chapterButton.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            // ボタンの高さのを使って縦に等間隔に並べる
            float buttonHeight = rect.sizeDelta.y;
            float y = verticalOffset + i * buttonHeight;
            float x = (i % 2 == 0) ? offsetX : -offsetX;

            rect.anchoredPosition = new Vector2(x, -y);
            
            if (chapterData.chapterNumber == currentChapter)
            {
                _currentChapterStar = chapterButton;
            }
        }
        PlaceRobotOnChapter();
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
                Const.GameSceneParam.Subject = data.subject;
                Const.GameSceneParam.DifficultyLevel = data.difficultyLevel;
                Const.GameSceneParam.ChapterNumber = data.chapterNumber;
                
                
                _onStartGame();
            }
        }, CommonDialog.Mode.OK_CANCEL);
    }

    private async void PlaceRobotOnChapter()
    {
        if (_currentChapterStar == null) return;
        
        // ロボットプレハブを生成
        var roboPrefab = await Utils.InstantiatePrefab("Prefabs/Robo/RoboPrefab", chapterButtonParent);
        var roboRect = roboPrefab.GetComponent<RectTransform>();
        
        // 現在のチャプターボタンの位置を取得
        var chapterRect = _currentChapterStar.GetComponent<RectTransform>();
        
        // ロボットをチャプターボタンの上に配置
        roboRect.anchorMin = roboRect.anchorMax = new Vector2(0.5f, 1f);
        roboRect.pivot = new Vector2(0.5f, 0.5f); // 下部中央を基準点に
        
        // チャプターボタンからY=-50の位置に配置
        roboRect.anchoredPosition = new Vector2(
            chapterRect.anchoredPosition.x, 
            chapterRect.anchoredPosition.y - 50f
        );
        
        // ロボットのサイズを調整（必要に応じて）
        roboRect.localScale = new Vector3(0.5f, 0.5f, 1f);
        
        // ユーザーの選択したロボットデータを設定
        var userDataManager = UserDataManager.GetInstance();
        var userData = userDataManager.GetUserData();
        var selectedRoboId = userData.selectedRoboId ?? "default";
        
        var roboCustomDataDict = userDataManager.GetRoboCustomData(selectedRoboId);
        if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(selectedRoboId))
        {
            var roboPrefabComponent = roboPrefab.GetComponent<RoboPrefab>();
            if (roboPrefabComponent != null)
            {
                roboPrefabComponent.SetRobo(roboCustomDataDict[selectedRoboId]);
            }
        }
    }
    
    public void OnClickCloseButton()
    {
        Destroy(this.gameObject);
    }
}
