using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [SerializeField] private BattleStatusUi battleStatusUi;
    public int HP { get; private set; } = 100;
    public void SetEnemyArea()
    {
        HP = 100;
        Debug.Log("敵エリアが設定されました。");
        UpdateHPDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP < 0) HP = 0;
        Debug.Log($"敵が {damage} ダメージを受けた。残りHP: {HP}");
        UpdateHPDisplay();
    }
    
    private void UpdateHPDisplay()
    {
        if (battleStatusUi != null)
        {
            // HPを0-1の範囲に正規化して表示
            float normalizedHP = HP / 100f;
            battleStatusUi.SetHP(normalizedHP);
        }
    }
}
