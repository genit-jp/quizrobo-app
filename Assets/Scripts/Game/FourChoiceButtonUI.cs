using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public class FourChoiceButtonUI : QuizUI
{
    private QuizData _quizData;
    private Action<bool, string> _answeredByUser;
    public override async void Setup(QuizData quizData, Action<bool, string> answeredByUser)
    {
        _quizData = quizData;
        _answeredByUser = answeredByUser;
        SetupChoiceAndJudge();
    }
    
    public override void DestroyGameUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async void SetupChoiceAndJudge()
    {
        var choices = _quizData.choices.ToList();
        
        Random random = new Random();
        
        choices = choices.OrderBy(x => random.Next()).ToList();
        
        var prefabPath = "Prefabs/Game/FourChoiceButton";
        var resource = (GameObject)await Resources.LoadAsync(prefabPath);
        if (resource == null)
        {
            Debug.LogError($"Prefab Resources load failed(Prefabs/Game/FourChoiceButton)");
        }
        foreach (var choice in choices)
        {
            var gameObj = Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (transform)
            {
                gameObj.transform.SetParent(transform, false);
            }
            var fourChoiceButton = gameObj.GetComponent<FourChoiceButton>();
            fourChoiceButton.Setup(choice, (userAnswer) =>
            {
                _answeredByUser(userAnswer == _quizData.answer, userAnswer);
            });
        }
    }
}