using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class ResultDialog: DialogBaseListener
{
    [SerializeField] private GameObject _scrollViewContent, _roboContent;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Text levelText;
    private Action _onOkButtonClicked;
    
    public async void Setup(List<QuizResultData> quizResults, Action onOkButtonClicked)
    {
        var prefabPath = "Prefabs/Game/ResultContent";
        var resource = (GameObject)await Resources.LoadAsync(prefabPath);
        foreach (var quizResult in quizResults)
        {
            var gameObj = Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            
            gameObj.transform.SetParent(_scrollViewContent.transform, false);
            var resultContent = gameObj.GetComponent<ResultContent>();
            resultContent.Setup(quizResult);
        }
        
        var userDataManager = UserDataManager.GetInstance();
        var roboCustomDataDict = userDataManager.GetRoboCustomData(userDataManager.GetUserData().selectedRoboId);
        if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(userDataManager.GetUserData().selectedRoboId))
        {
            var roboCustomData = roboCustomDataDict[userDataManager.GetUserData().selectedRoboId];
            await RoboSettingManager.DisplayRobo(_roboContent, roboCustomData);
        }
        
        // プレイヤーステータスを表示
        var playerStatus = userDataManager.GetPlayerStatus();
        int currentExp = playerStatus.exp;
        int level = LevelingSystem.CalculateLevelFromExp(currentExp);
        
        // 現在のレベルまでに必要だったEXPを計算
        int expToCurrentLevel = 0;
        for (int i = 1; i < level; i++)
        {
            expToCurrentLevel += LevelingSystem.GetExpToLevelUp(i);
        }
        
        // 次のレベルまでに必要なEXP
        int expForNextLevel = LevelingSystem.GetExpToLevelUp(level);
        int expInCurrentLevel = currentExp - expToCurrentLevel;
        
        // UIを更新
        levelText.text = level.ToString();
        expSlider.value = (float)expInCurrentLevel / expForNextLevel;
        
        _onOkButtonClicked = onOkButtonClicked;
    }

    public void OnClickHomeButton()
    {
        _onOkButtonClicked?.Invoke();
    }
    
    public override bool OnClickBlocker()
    {
        return false;
    }
}