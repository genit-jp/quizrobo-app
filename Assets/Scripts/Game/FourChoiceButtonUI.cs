using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = System.Random;

public class FourChoiceButtonUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> buttonSprites;
        
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
        // 選択肢をシャッフル
        var choices = _quizData.choices.ToList();
        Random random = new Random();
        choices = choices.OrderBy(x => random.Next()).ToList();

        var prefab = await LoadChoiceButtonPrefab();
        
        for (int i = 0; i < choices.Count; i++)
        {
            var gameObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            gameObj.transform.SetParent(transform, false);

            var fourChoiceButton = gameObj.GetComponent<FourChoiceButton>();
            fourChoiceButton.Setup(choices[i], userAnswer =>
            {
                _answeredByUser(userAnswer == _quizData.answer, userAnswer);
            });
            var image = gameObj.GetComponent<UnityEngine.UI.Image>();
            if (i < buttonSprites.Count && buttonSprites[i] != null)
            {
                image.sprite = buttonSprites[i];
            }
            else
            {
                Debug.Log($"buttonSprites[{i}] が未設定です。デフォルトのスプライトを使います。");
            }
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