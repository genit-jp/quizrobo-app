using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArea : MonoBehaviour
{
    [SerializeField] private BattleStatusUi battleStatusUi;
    [SerializeField] private GameObject myRoboArea;
    
    public int HP { get; private set; } = 100;
    private Action onAttack;

    public void Initialize(Action onAttackAction)
    {
        onAttack = onAttackAction;
    }


    public void SetMyArea()
    {
        SetRobo();
        HP = 100;
        Debug.Log("自分のHP: " + HP);
        UpdateHPDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP < 0) HP = 0;
        Debug.Log($"自分が {damage} ダメージを受けた。残りHP: {HP}");
        UpdateHPDisplay();
    }

    public void OnTappedAttackButton()
    {
        Debug.Log("攻撃ボタンがタップされました。");
        
        // 攻撃アクションを実行
        onAttack?.Invoke();
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
    private async void SetRobo()
    {
        // ユーザーの選択したロボットデータを取得
        var userDataManager = UserDataManager.GetInstance();
        var userData = userDataManager.GetUserData();
        var selectedRoboId = userData.selectedRoboId ?? "default";
        
        var roboCustomDataDict = userDataManager.GetRoboCustomData(selectedRoboId);
        if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(selectedRoboId))
        {
            var roboCustomData = roboCustomDataDict[selectedRoboId];
            
            // RoboSettingManagerを使用してロボットを表示
            await RoboSettingManager.DisplayRobo(myRoboArea, roboCustomData);
        }
        else
        {
            Debug.LogWarning($"RoboCustomData not found for selectedRoboId: {selectedRoboId}");
        }
    }
}
