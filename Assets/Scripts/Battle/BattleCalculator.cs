using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCalculator
{
    public int GetMyAttackPower()
    {
        // UserDataManagerからプレイヤーのEXPを取得
        var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
        int exp = playerStatus.exp;
        
        // EXPの1/10を攻撃力として返す
        int attackPower = exp / 10;
        
        // 最低攻撃力は1を保証
        return Mathf.Max(10, attackPower);
    }
    
    public int GetThereAttackPower(EnemyData data)
    {
        return 8; // 仮の固定値、将来的に計算式に変更可能
    }
    
}
