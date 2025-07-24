using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCalculator
{
    private const float HpGrowthRate = 1.12f;
    private const float AtkGrowthRate = 1.08f;
    
    public int GetMyAttackPower()
    {
        var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
        int level = playerStatus.level;

        // ベース攻撃力と成長率
        const int baseAtk = 10;
        const float atkGrowthRate = 1.10f;

        // 攻撃力計算式
        int attackPower = Mathf.FloorToInt(baseAtk * Mathf.Pow(atkGrowthRate, level));

        // 最低攻撃力を保証
        return Mathf.Max(10, attackPower);
    }
    
    public static int GetEnemyAttackPower(int playerLevel, EnemyData data)
    {
        int atk = Mathf.FloorToInt(data.defaultEXP * Mathf.Pow(AtkGrowthRate, playerLevel));
        return Mathf.Max(10, atk);
    }
    
    public static int GetEnemyHp(int playerLevel,EnemyData data)
    {
        int hp = Mathf.FloorToInt(data.defaultHP * Mathf.Pow(HpGrowthRate, playerLevel));
        return Mathf.Max(1, hp);
    }
    
    public int GetEnemyLevel(EnemyData data)
    {
        // EXPからレベルを計算
        return LevelingSystem.CalculateLevelFromExp(data.defaultEXP);
    }
}
