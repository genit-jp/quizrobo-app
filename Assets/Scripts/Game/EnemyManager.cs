using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{

    private List<Enemy> _enemies = new List<Enemy>();

    public bool AllEnemiesDefeated => _enemies.TrueForAll(e => e.IsDefeated);
    
    public void SetupEnemies(int chapterNumber, Transform enemyArea)
    {
        _enemies.Clear();
        
        var enemyDataText = Resources.Load<TextAsset>("Data/enemy_data");
        if (enemyDataText == null)
        {
            Debug.LogError("enemy_data.txt not found");
            return;
        }

        string[] enemyPatterns = enemyDataText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        int chapterIndex = chapterNumber - 1;
        if (chapterIndex < 0 || chapterIndex >= enemyPatterns.Length)
        {
            Debug.LogError($"Chapter {chapterNumber} not found in enemy_data");
            return;
        }

        string rawLine = enemyPatterns[chapterIndex].Trim();
        string[] parts = rawLine.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
        string enemyPattern = parts.Length >= 2 ? parts[1] : parts[0];


        // 古い敵を削除
        foreach (Transform child in enemyArea)
            Destroy(child.gameObject);

        int enemyCount = Mathf.Min(enemyPattern.Length, 4);
        Debug.Log($"Chapter {chapterNumber}: Spawning {enemyCount} enemies with pattern '{enemyPattern}'");
        for (int i = 0; i < enemyCount; i++)
        {
            char enemyChar = enemyPattern[i];
            string enemyId = enemyChar.ToString();
            
            
            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Game/Enemy");
            
            // プレハブをインスタンス化
            GameObject enemyObj = Instantiate(enemyPrefab, enemyArea);
            enemyObj.name = $"Enemy_{enemyId}_{i}";
                
            Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
            
            if (enemyComponent == null) continue;
            int maxHp = Const.enemyHpTable.GetValueOrDefault(enemyChar, 100);
            enemyComponent.Setup(enemyId, maxHp);
                
            _enemies.Add(enemyComponent);
        }
    }
    
    public void AttackNextEnemy(int damage)
    {
        foreach (var enemy in _enemies)
        {
            if (!enemy.IsDefeated)
            {
                enemy.TakeDamage(damage);
                break;
            }
        }
    }
}

