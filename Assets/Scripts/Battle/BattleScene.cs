using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private MyArea myArea;
    [SerializeField] private EnemyArea enemyArea;
    
    private BattleCalculator calculator = new BattleCalculator();

    private void Start()
    {
        myArea.Initialize(PlayerAttack);
        myArea.SetMyArea();
        enemyArea.SetEnemyArea();
        
    }
    
    private void PlayerAttack()
    {
        int damage = calculator.GetAttackPower();
        enemyArea.TakeDamage(damage);

        if (enemyArea.HP <= 0)
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

        int damage = calculator.GetAttackPower();
        myArea.TakeDamage(damage);

        if (myArea.HP <= 0)
        {
            Debug.Log("自分が倒された...");
        }
    }
}
