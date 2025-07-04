using System;
using UnityEngine;

[Serializable]
public class QuizData
{
    public string question = "";
    public string imgPath = "";
    public string answer = "";
    public string subject = "";
    public string[] forbiddenLetters;
    public string compliment = "";
    public string explanation = "";
    public int targetGrade = 0;
    public string id = "";
    public int quizKind = 0;
    public string[] choices;
    public string[] quizTags;
    public int quizLevel;
    public bool isGood = false;
    public Texture2D imgTexture;
}
