using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class QuizData
{
    public string Id = "";
    public bool available = true;
    public string question = "";
    public string[] choices;  // カンマ区切りの文字列として受け取る
    public string answer; 
}


