using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private MyArea myArea;
    [SerializeField] private EnemyArea enemyArea;
    
    private BattleCalculator calculator = new BattleCalculator();
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
        int damage = calculator.GetMyAttackPower();
        enemyArea.TakeDamage(damage);

        if (enemyArea.Hp <= 0)
        {
            Debug.Log("敵を倒した！");
        }
        else
        {
            StartCoroutine(EnemyAttack());
        }
    }
    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);

        int damage = calculator.GetThereAttackPower(_enemyDataGot);
        myArea.TakeDamage(damage);

        if (myArea.Hp <= 0)
        {
            Debug.Log("自分が倒された...");
        }
    }
}
