using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArea : MonoBehaviour
{
    [SerializeField] private BattleStatusUi battleStatusUi;
    [SerializeField] private Image enemyImage;
    public int Hp { get; private set; } = 100;
    public void SetEnemyArea(EnemyData data, int hp)
    {
        Hp = hp;
        Debug.Log("敵エリアが設定されました。");
       
        string resourcePath = $"Images/Enemy/{data.id}";
        var sprite = Resources.Load<Sprite>(resourcePath);
            
        if (sprite != null)
        {
            enemyImage.sprite = sprite;
        }
        
        UpdateHpDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0) Hp = 0;
        Debug.Log($"敵が {damage} ダメージを受けた。残りHP: {Hp}");
        UpdateHpDisplay();
    }
    
    private void UpdateHpDisplay()
    {
        if (battleStatusUi != null)
        {
            // HPを0-1の範囲に正規化して表示
            float normalizedHP = Hp / 100f;
            battleStatusUi.SetHP(normalizedHP, BattleStatusUi.CharacterType.Enemy);
        }
    }
}
