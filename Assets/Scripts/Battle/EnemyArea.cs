using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    public int HP { get; private set; } = 100;
    public void SetEnemyArea()
    {
        HP = 100;
        Debug.Log("敵エリアが設定されました。");
    }
    
    public void TakeDamage(int damage)
    {
        HP -= damage;
        Debug.Log($"敵が {damage} ダメージを受けた。残りHP: {HP}");
    }
}
