using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelingSystem
{
    private const int ExpPerCorrectAnswer = 5;
    public const int RecoveryHpPerQuest = 30;
    
    public static int CalculateExpFromCorrectAnswers(int correctAnswers)
    {
        return correctAnswers * ExpPerCorrectAnswer;
    }

    // レベルアップに必要なEXP（例：逓減倍率式）
    public static int GetExpToLevelUp(int level)
    {
        if (level < 10)
            return Mathf.FloorToInt(40 * Mathf.Pow(1.25f, level - 1));
        else
            return Mathf.FloorToInt(40 * Mathf.Pow(1.15f, level - 1));
    }

    // 現在の累積EXPからレベルを再計算
    public static int CalculateLevelFromExp(int totalExp)
    {
        int level = 1;
        int expToNext = GetExpToLevelUp(level);

        while (totalExp >= expToNext)
        {
            totalExp -= expToNext;
            level++;
            expToNext = GetExpToLevelUp(level);
        }

        return level;
    }
}
