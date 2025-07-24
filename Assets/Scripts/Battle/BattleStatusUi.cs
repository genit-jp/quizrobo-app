using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusUi : MonoBehaviour
{
    [SerializeField] private Slider hpGauge;
    [SerializeField] private GameObject roboLevelText, enemyLevelText;
    
    public enum CharacterType
    {
        Robo,
        Enemy,
    }
    
    public void SetHP(float value, CharacterType characterType, int enemyLevel = 1)
    {
        hpGauge.value = value;
        if (characterType == CharacterType.Robo)
        {
            roboLevelText.SetActive(true);
            // 自分のレベルを表示
            var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
            int playerLevel = LevelingSystem.CalculateLevelFromExp(playerStatus.exp);
            roboLevelText.GetComponentInChildren<Text>().text = $"{playerLevel}";
        }
        else
        {
            enemyLevelText.SetActive(true);
            //敵のレベルをあとできちんと設定
            enemyLevelText.GetComponentInChildren<Text>().text = $"{enemyLevel}";
        }
    }
    
}
