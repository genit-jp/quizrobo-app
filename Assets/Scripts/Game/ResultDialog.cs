using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ResultDialog: DialogBaseListener
{
    [SerializeField] private GameObject _scrollViewContent, _roboContent;
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