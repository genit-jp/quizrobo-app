using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LetterChoiceButtonUI : QuizUI
{

    private List<string> hiragana = new List<string>
    {
        "あ", "い", "う", "え", "お",
        "か", "き", "く", "け", "こ",
        "さ", "し", "す", "せ", "そ",
        "た", "ち", "つ", "て", "と",
        "な", "に", "ぬ", "ね", "の",
        "は", "ひ", "ふ", "へ", "ほ",
        "ま", "み", "む", "め", "も",
        "や", "ゆ", "よ",
        "ら", "り", "る", "れ", "ろ",
        "わ"
    };

    [SerializeField] private RubyTextMeshProUGUI _selectedChoicesText;
    private int _answerIndex = 0;
    private int _correctCount = 0;
    private string _selectedChoices = "";
    private QuizData _quizData;
    private Action<bool, string> _answeredByUser;
    

    public override void Setup(QuizData quizData, Action<bool, string> answeredByUser)
    {
        _quizData = quizData;
        _answeredByUser = answeredByUser;
        _selectedChoicesText.uneditedText = "<r=ただ>正</r>しい<r=もじ>文字</r>を<r=せんたく>選択</r>しよう!";
        SetupCharacterChoiceAndJudge();
    }

    public override void DestroyGameUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    private async void SetupCharacterChoiceAndJudge()
    {
        //y軸のChildForceExpandを擬似的に実行
        Canvas.ForceUpdateCanvases(); 
        var gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(450, (gameObject.GetComponent<RectTransform>().rect.height - 165) / 2);
        
        foreach (Transform child in transform)
        {
            if (child != _selectedChoicesText.transform)
            {
                Destroy(child.gameObject);
            }
        }

        System.Random random = new System.Random();
        
        var correctChar = _quizData.answer[_answerIndex];

        var choices = new List<string> { correctChar.ToString() };
        while (choices.Count < 4)
        {
            var randomChoice = hiragana[random.Next(hiragana.Count)];
            if (!choices.Contains(randomChoice) && !randomChoice.Contains(_quizData.forbiddenLetters.ToString()))
            {
                choices.Add(randomChoice);
            }
        }

        choices = choices.OrderBy(x => random.Next()).ToList();
        
        var prefabPath = "Prefabs/Game/LetterChoiceButton";
        var resource = (GameObject)await Resources.LoadAsync(prefabPath);
        if (resource == null)
        {
            Debug.LogError($"Prefab Resources load failed({prefabPath})");
        }
        foreach (var choice in choices)
        {
            var gameObj = Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (transform)
            {
                gameObj.transform.SetParent(transform, false);
            }
            var letterChoiceButton = gameObj.GetComponent<LetterChoiceButton>();

            letterChoiceButton.Setup(choice, (selectedChoice) =>
            {
                _selectedChoices += selectedChoice;
                if (selectedChoice == correctChar.ToString())
                {
                    _correctCount++;
                    if (_correctCount > 0)
                    {
                        _selectedChoicesText.fontSize = 80;
                    }
                    _selectedChoicesText.uneditedText = "<u>"+_selectedChoices+"</u>";
                    if (_correctCount < _quizData.answer.Length)
                    {
                        _answerIndex++;
                        SetupCharacterChoiceAndJudge();
                    }
                    else
                    {
                        _answeredByUser(true, _selectedChoices);
                    }
                }
                else
                {
                    _answeredByUser(false, _selectedChoices);
                }
            });
        }
    }
}