using System.Collections;
using System.Collections.Generic;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private MyArea myArea;
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private GameObject goBackToSelectButton;
    
    private readonly BattleCalculator _calculator = new BattleCalculator();
    private EnemyData _enemyDataGot;
    private bool _isGoBackSelectSceneVisible;

    private void Start()
    {
        _enemyDataGot = MasterData.GetInstance().GetRandomEnemyData();
        myArea.Initialize(PlayerAttack);
        myArea.SetMyArea();
        enemyArea.SetEnemyArea(_enemyDataGot);
        _isGoBackSelectSceneVisible = true;
        goBackToSelectButton.SetActive(_isGoBackSelectSceneVisible);
    }
    
    private void PlayerAttack()
    {
        // 攻撃中は戻るボタンを非表示
        _isGoBackSelectSceneVisible = false;
        goBackToSelectButton.SetActive(_isGoBackSelectSceneVisible);
        
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
        
        // 新しい敵が出現したら戻るボタンを再表示
        _isGoBackSelectSceneVisible = true;
        goBackToSelectButton.SetActive(_isGoBackSelectSceneVisible);
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
    
    private async void OnGameOver()
    {
        // リザルト画面に遷移するなど（未実装）
        Debug.Log("ゲームオーバー処理へ");
        
        // ゲームオーバー時は戻るボタンを再表示
        _isGoBackSelectSceneVisible = true;
        goBackToSelectButton.SetActive(_isGoBackSelectSceneVisible);
        
        // CommonDialogを表示
        var dialogObj = await Utils.OpenDialog("Prefabs/Common/CommonDialog", transform);
        var commonDialog = dialogObj.GetComponent<CommonDialog>();
        commonDialog.Setup("ゲームオーバー", "クエストに参加しよう\n攻撃力UP・HPを回復してもう一度挑戦しよう", (result) =>
        {
            GoToSelectScene();
        }, CommonDialog.Mode.OK);
    }

    public void OnTappedGoBackToSelectButton()
    {
        Debug.Log("戻るボタンがタップされました。");
        GoToSelectScene();
    }

    private void GoToSelectScene()
    {
        var scene = SceneManager.GetSceneByName("SelectScene");
        foreach (var go in scene.GetRootGameObjects())
        {
            if (go.name == "Canvas")
            {
                go.SetActive(true);
            }
        }

        // BattleSceneをアンロード
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("BattleScene"));
    }
}
