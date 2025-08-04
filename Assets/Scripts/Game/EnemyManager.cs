using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{

    public void SetupEnemies(int chapterNumber, Transform enemyArea)
    {
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

        string enemyPattern = enemyPatterns[chapterIndex].Trim();

        // 古い敵を削除
        foreach (Transform child in enemyArea)
            Destroy(child.gameObject);

        int enemyCount = Mathf.Min(enemyPattern.Length, 4);
        for (int i = 0; i < enemyCount; i++)
        {
            char enemyChar = enemyPattern[i];
            string enemyId = enemyChar.ToString();
            
            
            GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Game/Enemy");
            
            if (enemyPrefab != null)
            {
                // プレハブをインスタンス化
                GameObject enemyObj = Instantiate(enemyPrefab, enemyArea);
                enemyObj.name = $"Enemy_{enemyId}_{i}";
                
                Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    int maxHp = Const.enemyHpTable.GetValueOrDefault(enemyChar, 100);
                    enemyComponent.Setup(enemyId, maxHp);
                }
                else
                {
                    Debug.LogError("Enemy component not found on the prefab!");
                }
            }
            else
            {
                Debug.LogError("Enemy prefab not found at: Prefabs/Game/Enemy");
            }
        }

        Debug.Log($"Chapter {chapterNumber}: Spawned {enemyCount} enemies from pattern '{enemyPattern}'");
    }
}

