using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using KoganeUnityLib;

public class Quiz : MonoBehaviour
{
    [SerializeField] private RubyTextMeshProUGUI _quizText;
    [SerializeField] private RectTransform _quizTextRect;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _choiceButtonContainer;
    [SerializeField]private GameObject _skipButton;
    [SerializeField] private Text timerText;
    [SerializeField] private float timeLimit = 10f;
    
    private float _remainingTime;
    private bool _isTiming;

    private MonoBehaviour _quizUI; // Can be either FourChoiceButtonUI or NumpadUI
    private Action<bool, string> _answeredByUser;

    void Start()
    {
        StartTimer();
    }
    
    void Update()
    {
        if (_isTiming)
        {
            _remainingTime -= Time.deltaTime;
            timerText.text = $"{Mathf.CeilToInt(_remainingTime)}";

            if (_remainingTime <= 0f)
            {
                _isTiming = false;
                OnTimeUp();
            }
        }
    }
    
    public async void Setup(QuizData quizData, int quizIndex, Action<bool, string> answeredByUser)
    {
        // if (QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Calculation)
        // {
        //     _skipButton.SetActive(false);
        // }

        string question = quizData.question;
        
        _answeredByUser = (isCorrect, answer) =>
        {
            StopTimer(); // ⏱タイマー停止！
            answeredByUser?.Invoke(isCorrect, answer);
        };

        // 問題文の高さを調整
        string planeText = GetQuestionPlainText(question); // マークアップを除去した問題文
        int textLength = TextToTextLength(planeText); // 問題文の文字数(空白行がある時は20文字加算)
        int textHeight = TextLengthToHeight(textLength); // 問題文の高さ
        _quizTextRect.sizeDelta = new Vector2(_quizTextRect.sizeDelta.x, textHeight);
        
        // if(quizData.quizTags.Contains("計算"))
        // {
        //     //textのautosizeをオフ
        //     _quizText.enableAutoSizing = false;
        //     _quizText.fontSize = 90;
        // }
        _quizText.uneditedText = GetQuestionRichText(question);
        
        // Check if this is a numeric answer quiz (assumes numeric answers are required for NumpadUI)
        bool isNumericQuiz = IsNumericAnswer(quizData.answer);
        
        if (isNumericQuiz)
        {
            Debug.Log("Using NumpadUI for numeric quiz");
            var instantiatedObj = await Genit.Utils.InstantiatePrefab("Prefabs/Game/NumpadUI", _choiceButtonContainer.transform);
            var numpadUI = instantiatedObj.GetComponent<NumpadUI>();
            numpadUI.Setup(quizData, _answeredByUser);
            _quizUI = numpadUI;
        }
        else
        {
            Debug.Log("Using FourChoiceButtonUI for non-numeric quiz");
            var instantiatedObj = await Genit.Utils.InstantiatePrefab("Prefabs/Game/FourChoiceButtonUI", _choiceButtonContainer.transform);
            var fourChoiceUI = instantiatedObj.GetComponent<FourChoiceButtonUI>();
            fourChoiceUI.Setup(quizData, _answeredByUser);
            _quizUI = fourChoiceUI;
        }

        StartTimer();
    }

    public void DestroyGameUI()
    {
        _skipButton.SetActive(false);
        if (_quizUI is FourChoiceButtonUI fourChoiceUI)
        {
            fourChoiceUI.DestroyGameUI();
        }
        else if (_quizUI is NumpadUI numpadUI)
        {
            numpadUI.DestroyGameUI();
        }
    }

    private string GetQuestionRichText(string question)
    {
        var questionRichText = question;
        questionRichText = questionRichText.Replace("[c]", "<color=#FFE600>").Replace("[/c]", "</color>").Replace("\\n", "\n");

        return questionRichText;
    }
    
    private string GetQuestionPlainText(string question)
    {
        // <r=...>...</r> マークアップを除去して、内容を保持する
        string patternForReading = @"<r=[^>]+>([^<]+)</r>";
        string result = Regex.Replace(question, patternForReading, "$1");

        // [c]...[/c] マークアップを除去して、内容を保持する
        string patternForColor = @"\[c\](.*?)\[/c\]";
        result = Regex.Replace(result, patternForColor, "$1");

        return result;
    }
    
    public static int TextToTextLength(string text)
    {
        // テキストを行ごとに分割
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        int totalLength = text.Length; // 初期の文字数

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                // 空白行が見つかった場合、20を加算
                totalLength += 20;
            }
        }

        return totalLength;
    }
    
    private int TextLengthToHeight(float textLength)
    {
        if (textLength < 15)
        {
            return 150;
        }
        else if (textLength < 20)
        {
            return 200;
        }
        else if (textLength < 40)
        {
            return 250;
        }
        else if (textLength < 60)
        {
            return 300;
        }
        else if (textLength < 80)
        {
            return 350;
        }
        else 
        {
            return 400;
        }
    }
    
    private void StartTimer()
    {
        _remainingTime = timeLimit;
        _isTiming = true;
    }
    
    public void ResumeTimer()
    {
        _isTiming = true; // 残り時間を変えずに再開
    }

    public void StopTimer()
    {
        _isTiming = false;
    }
    
    private void OnTimeUp()
    {
        _answeredByUser(false, "");
    }
    
    public void OnClickSkipButton()
    {
        _answeredByUser(false, "");
    }
    
    private bool IsNumericAnswer(string answer)
    {
        // Check if the answer string contains only digits
        return !string.IsNullOrEmpty(answer) && answer.All(char.IsDigit);
    }
}