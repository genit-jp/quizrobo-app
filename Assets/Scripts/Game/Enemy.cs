using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Image enemyImage;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;
    [SerializeField] private GameObject hpBar;

    private System.Action _onEnemyDefeated;
    private int _currentHp, _maxHp;
    public bool IsDefeated => _currentHp <= 0;
    public int GetMaxHp() => _maxHp;
    private Animator _animator;

    public void Setup(string id, int maxHp,　System.Action onEnemyDefeated = null)
    {
        _animator = GetComponentInChildren<Animator>();
        _maxHp = maxHp;
        _currentHp = maxHp;
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
        UpdateHpUI();

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

    public void TakeDamage(int damage)
    {
        if (IsDefeated) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);
        UpdateHpUI();

        if (_currentHp == 0)
        {
            _onEnemyDefeated?.Invoke();
            _animator.SetTrigger("Defeated");
            hpBar.SetActive(false);
        }
    }

    private void UpdateHpUI()
    {
        hpSlider.maxValue = _maxHp;
        hpSlider.value = _currentHp;
        hpText.text = $"{_currentHp.ToString()}/{_maxHp.ToString()}";
    }
    
}
