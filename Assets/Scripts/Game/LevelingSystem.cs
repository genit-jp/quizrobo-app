using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingSystem
{
    public static int CalculateExpFromCorrectAnswers(int correctAnswerCount)
    {
        // 仮のルール：1問正解につき10EXP
        return correctAnswerCount * 10;
    }
    
    //レベルの計算もここで行う
}
