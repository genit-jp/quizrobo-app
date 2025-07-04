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
    private QuizUI _quizUI;
    private Action<bool, string> _answeredByUser;
    private Dictionary<int,string> _quizUIs = new Dictionary<int, string>()
    {
        {0, "Prefabs/Game/FourChoiceButtonUI"},
        {1, "Prefabs/Game/LetterChoiceButtonUI"}
    };

    public async void Setup(QuizData quizData, int quizIndex, Action<bool, string> answeredByUser)
    {
        if (QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Calculation)
        {
            _skipButton.SetActive(false);
        }
        
        string question = quizData.question;
        string imgPath = quizData.imgPath;
        _answeredByUser = answeredByUser;

        // 問題文の高さを調整
        string planeText = GetQuestionPlainText(question); // マークアップを除去した問題文
        int textLength = TextToTextLength(planeText); // 問題文の文字数(空白行がある時は20文字加算)
        int textHeight = TextLengthToHeight(textLength); // 問題文の高さ
        _quizTextRect.sizeDelta = new Vector2(_quizTextRect.sizeDelta.x, textHeight);
        
        if (imgPath != "")
        {
            Texture2D texture = quizData.imgTexture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    
            // 指定された縦幅
            float desiredHeight = 300f;
            // アスペクト比を計算 (元の画像の幅/高さ)
            float aspectRatio = (float)texture.width / (float)texture.height;
            // 横幅を計算 (縦幅 * アスペクト比)
            float calculatedWidth = desiredHeight * aspectRatio;
    
            // ImageコンポーネントのRectTransformサイズを調整してアスペクト比を保持する
            _image.rectTransform.sizeDelta = new Vector2(calculatedWidth, desiredHeight);
            _image.sprite = sprite;
    
            _image.gameObject.SetActive(true);
        }
        
        if(quizData.quizTags.Contains("計算"))
        {
            //textのautosizeをオフ
            _quizText.enableAutoSizing = false;
            _quizText.fontSize = 90;
        }
        _quizText.uneditedText = GetQuestionRichText(question);

        var instantiatedObj = await Genit.Utils.InstantiatePrefab(_quizUIs[quizData.quizKind], _choiceButtonContainer.transform);
        _quizUI = instantiatedObj.GetComponent<QuizUI>();
        _quizUI.Setup(quizData, _answeredByUser);
    }

    public void DestroyGameUI()
    {
        _skipButton.SetActive(false);
        _quizUI.DestroyGameUI();
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
    
    public void OnClickSkipButton()
    {
        _answeredByUser(false, "");
    }
}