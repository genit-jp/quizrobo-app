using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private MyArea myArea;
    [SerializeField] private EnemyArea enemyArea;
    
    private readonly BattleCalculator _calculator = new BattleCalculator();
    private EnemyData _enemyDataGot;

    private void Start()
    {
        _enemyDataGot = MasterData.GetInstance().GetRandomEnemyData();
        myArea.Initialize(PlayerAttack);
        myArea.SetMyArea();
        enemyArea.SetEnemyArea(_enemyDataGot);
    }
    
    private void PlayerAttack()
    {
        int damage = _calculator.GetMyAttackPower();
        enemyArea.TakeDamage(damage);

        if (enemyArea.Hp <= 0)
        {
            SaveCurrentHp(); // ✅ 敵を倒したら保存
            SpawnNextEnemy(); // 仮処理（後述）
        }
        else
        {
            StartCoroutine(EnemyAttack());
        }
    }
    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);

        int damage = _calculator.GetThereAttackPower(_enemyDataGot);
        myArea.TakeDamage(damage);

        if (myArea.Hp <= 0)
        {
            SaveCurrentHp(); 
            OnGameOver();    
            Debug.Log("自分が倒された...");
        }
    }
    
    private void SpawnNextEnemy()
    {
        // 次の敵に差し替える処理（未実装）
        _enemyDataGot = MasterData.GetInstance().GetRandomEnemyData();
        enemyArea.SetEnemyArea(_enemyDataGot);
    }
    
    private async void SaveCurrentHp()
    {
        int currentHp = myArea.Hp;
        Debug.Log($"HP保存: {currentHp}");

        // UserDataManagerにHPを保存する処理
        var userDataManager = UserDataManager.GetInstance();
        var playerStatus = userDataManager.GetPlayerStatus();
        
        playerStatus.hp = currentHp;
        
        await userDataManager.UpdatePlayerStatus(playerStatus);
        
        Debug.Log($"PlayerStatus saved - HP: {playerStatus.hp}");
    }
    
    private void OnGameOver()
    {
        // リザルト画面に遷移するなど（未実装）
        Debug.Log("ゲームオーバー処理へ");
    }
}
