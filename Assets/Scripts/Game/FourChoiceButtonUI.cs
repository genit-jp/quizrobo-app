using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public class FourChoiceButtonUI : MonoBehaviour
{
    private QuizData _quizData;
    private Action<bool, string> _answeredByUser;
    private static GameObject _cachedChoiceButtonPrefab;
    public async void Setup(QuizData quizData, Action<bool, string> answeredByUser)
    {
        _quizData = quizData;
        _answeredByUser = answeredByUser;
        await SetupChoiceAndJudge();
    }
    
    public void DestroyGameUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async UniTask SetupChoiceAndJudge()
    {
        // クイズの基本情報をログ出力（確認用）
        Debug.Log($"Question: {_quizData.question}");
        Debug.Log($"Answer: {_quizData.answer}");
        Debug.Log("Choices (raw): " + string.Join(", ", _quizData.choices));

        // 選択肢をシャッフル
        var choices = _quizData.choices.ToList();
        Random random = new Random();
        choices = choices.OrderBy(x => random.Next()).ToList();

        var prefab = await LoadChoiceButtonPrefab();

        Debug.Log($"Instantiating {choices.Count} buttons");
        foreach (var choice in choices)
        {
            Debug.Log($"Creating button for choice: {choice}");
            var gameObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            gameObj.transform.SetParent(transform, false);

            var fourChoiceButton = gameObj.GetComponent<FourChoiceButton>();
            if (fourChoiceButton == null)
            {
                Debug.LogError("Missing FourChoiceButton component on prefab!");
            }
            fourChoiceButton.Setup(choice, userAnswer =>
            {
                Debug.Log($"User selected: {userAnswer} → Correct answer: {_quizData.answer}");
                _answeredByUser(userAnswer == _quizData.answer, userAnswer);
            });
        }
    }

    
    private async UniTask<GameObject> LoadChoiceButtonPrefab()
    {
        if (_cachedChoiceButtonPrefab == null)
        {
            
            var resource = await Resources.LoadAsync<GameObject>("Prefabs/Game/FourChoiceButton");
            _cachedChoiceButtonPrefab = resource as GameObject;
        }

        return _cachedChoiceButtonPrefab;
    }
}