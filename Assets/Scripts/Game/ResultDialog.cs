using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResultDialog: DialogBaseListener
{
    [SerializeField] private Text _totalMedalText;
    [SerializeField] private Text _increasedMedalText;
    [SerializeField] private GameObject _scrollViewContent;
    private Action _onOkButtonClicked;
    
    public async void Setup(List<QuizResultData> quizResults, int medalNum, int increasedMedalNum, Action onOkButtonClicked)
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
        
        _onOkButtonClicked = onOkButtonClicked;
        
        _totalMedalText.text = medalNum.ToString();
        _increasedMedalText.text = increasedMedalNum.ToString();
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