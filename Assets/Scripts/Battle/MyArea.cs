using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArea : MonoBehaviour
{
    [SerializeField] private BattleStatusUi battleStatusUi;
    [SerializeField] private GameObject myRoboArea;
    
    public int Hp { get; private set; }
    private Action _onAttack;

    public void Initialize(Action onAttackAction)
    {
        _onAttack = onAttackAction;
    }


    public void SetMyArea()
    {
        SetRobo();
        
        // UserDataManagerからHPを取得
        var playerStatus = UserDataManager.GetInstance().GetPlayerStatus();
        Hp = playerStatus.hp;
        
        Debug.Log("自分のHP: " + Hp);
        UpdateHPDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0) Hp = 0;
        Debug.Log($"自分が {damage} ダメージを受けた。残りHP: {Hp}");
        UpdateHPDisplay();
    }

    public void OnTappedAttackButton()
    {
        Debug.Log("攻撃ボタンがタップされました。");
        
        // 攻撃アクションを実行
        _onAttack?.Invoke();
    }
    
    private void UpdateHPDisplay()
    {
        if (battleStatusUi != null)
        {
            // HPを0-1の範囲に正規化して表示
            float normalizedHP = Hp / 100f;
            battleStatusUi.SetHP(normalizedHP, BattleStatusUi.CharacterType.Robo);
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
