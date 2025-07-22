using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArea : MonoBehaviour
{
    [SerializeField] private BattleStatusUi battleStatusUi;
    [SerializeField] private Image enemyImage;
    public int HP { get; private set; } = 100;
    public void SetEnemyArea(EnemyData data)
    {
        HP = data.defaultHP;
        Debug.Log("敵エリアが設定されました。");
        
        // 敵の画像を設定
        if (enemyImage != null && data != null)
        {
            string resourcePath = $"Images/Enemy/{data.id}";
            var sprite = Resources.Load<Sprite>(resourcePath);
            
            if (sprite != null)
            {
                enemyImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Enemy sprite not found at path: {resourcePath}");
            }
        }
        
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
