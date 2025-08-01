using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private Slider hpSlider;
    
    private int _currentHp, _maxHp;

    public void Setup(string id, int maxHp, System.Action onEnemyDefeated= null)
    {
        string resourcePath = $"Images/Enemy/{id}";
        var sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            enemyImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Enemy image not found at path: {resourcePath}");
        }
        
        SetSprite(id);
        SetSlider();

        // ここで敵の行動や攻撃のロジックを追加することができます
    }
    
    private void SetSprite(string id)
    {
        string path = $"Images/Enemy/{id}";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            enemyImage.sprite = sprite;
            enemyImage.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning($"Enemy sprite not found: {path}");
        }
    }

    public void SetSlider()
    {
        hpSlider.maxValue = _maxHp;
        hpSlider.value = _currentHp;
    }
    
}
