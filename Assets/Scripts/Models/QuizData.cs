using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class QuizData
{
    public string Id = "";
    public bool available = true;
    public string subject = "";
    public string question = "";
    public string[] choices;  // カンマ区切りの文字列として受け取る
    public string answer; 
    public string difficultyLevels = "";  // JSONではdifficultyLevelsだが、levelでも受け取れるようにする
    public string imgPath = "";
    
    // Unity側で使用するプロパティ（JSONシリアライズ対象外）
    [NonSerialized]
    public Texture2D imgTexture;
}


