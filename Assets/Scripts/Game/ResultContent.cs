using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultContent: MonoBehaviour
{
    [SerializeField] private RubyTextMeshProUGUI _questionText;
    [SerializeField] private RubyTextMeshProUGUI _answerText;
    [SerializeField] private Image _image;
    
    public void Setup(QuizResultData quizResultData)
    {
        _questionText.uneditedText = GetQuestionRichText(quizResultData.Quiz.question);
        _answerText.uneditedText = quizResultData.Quiz.answer;
        
        Texture2D texture;
        if (quizResultData.IsCorrect)
        {
            texture =  Resources.Load<Texture2D>("Images/maru");
        }
        else
        {
            texture =  Resources.Load<Texture2D>("Images/batsu");
        }
        _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    
    private string GetQuestionRichText(string question)
    {
        var questionRichText = question;
        questionRichText = questionRichText.Replace("[c]", "<color=#E36555>");
        questionRichText = questionRichText.Replace("[/c]", "</color>");

        return questionRichText;
    }
}