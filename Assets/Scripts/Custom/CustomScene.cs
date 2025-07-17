using System.Collections;
using System.Collections.Generic;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomScene : MonoBehaviour
{
    [SerializeField] private GameObject roboContainer, partSelectorPanel, currentPartsPanel;
    [SerializeField] private Text selectedPartText;
    
    private UserDataManager.RoboCustomData _customizingRoboData;
    private string _currentRoboId;

    private async void Start()
    { 
        var userDataManager = UserDataManager.GetInstance();
        var userData = userDataManager.GetUserData();
        _currentRoboId = userData.selectedRoboId ?? "default";
        
        var roboCustomDataDict = userDataManager.GetRoboCustomData(_currentRoboId);
        var original = roboCustomDataDict[_currentRoboId];
        _customizingRoboData = new UserDataManager.RoboCustomData
        {
            headId = original.headId,
            bodyId = original.bodyId,
            armsId = original.armsId,
            legsId = original.legsId,
            tailId = original.tailId
        };
        
       await RoboSettingManager.DisplayRobo(roboContainer, _customizingRoboData);
       
       CreatePartSelectButtons();
    }
    
    private async void CreatePartSelectButtons()
    {
        // 既存の子オブジェクトをクリア
        foreach (Transform child in partSelectorPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        // PART_NAMESの各パーツに対してボタンを作成
        foreach (var partEntry in Const.PART_NAMES)
        {
            var partKey = partEntry.Key; 
            var partJapaneseName = partEntry.Value;  
            var buttonObj = await Utils.InstantiatePrefab("Prefabs/Custom/PartSlectorButton", partSelectorPanel.transform);
            
            var partSelectorButton = buttonObj.GetComponent<PartSelectorButton>();
            partSelectorButton.SetPartSelectorButton(partJapaneseName);
            
            // Actionを設定
            string capturedPartKey = partKey;  // クロージャー用にコピー
            partSelectorButton.onPartSelected = () => OnPartButtonClicked(capturedPartKey);
            
        }
    }
    
    private void OnPartButtonClicked(string partKey)
    {
        if (Const.PART_NAMES.TryGetValue(partKey, out string japaneseName))
        {
            selectedPartText.text = japaneseName;
        }
        
        // 選択されたパーツタイプに応じてcurrentPartButtonsを作成
        CreateCurrentPartButtons(partKey);
    }
    
    private async void CreateCurrentPartButtons(string partType)
    {
        if (currentPartsPanel == null)
        {
            Debug.LogWarning("currentPartsPanel is not set in CustomScene");
            return;
        }
        
        // 既存の子オブジェクトをクリア
        foreach (Transform child in currentPartsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        // MasterDataから該当するパーツタイプのRoboDataを取得
        var masterData = MasterData.GetInstance();
        var roboDataArray = masterData.GetRoboDataByPartType(partType);
        
        if (roboDataArray == null || roboDataArray.Length == 0)
        {
            Debug.LogWarning($"No RoboData found for part type: {partType}");
            return;
        }
        
        // 各RoboDataに対してCurrentPartButtonを作成
        foreach (var roboData in roboDataArray)
        {
            var buttonObj = await Utils.InstantiatePrefab("Prefabs/Custom/CurrentPartButton", currentPartsPanel.transform);
            
            if (buttonObj != null)
            {
                var currentPartButton = buttonObj.GetComponent<CurretPartButton>();
                if (currentPartButton != null)
                {
                    // roboDataのidをpartIdとして設定（画像パスに使用）
                    currentPartButton.SetPartButton(roboData.id);
                    
                    // ボタンクリック時の処理を設定
                    string capturedRoboId = roboData.id;
                    currentPartButton.onPartSelected = () => OnCurrentPartSelected(partType, capturedRoboId);
                    
                    // ボタンのクリックイベントを設定
                    var button = buttonObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => currentPartButton.OnPartButtonClicked());
                    }
                }
                else
                {
                    Debug.LogWarning("CurretPartButton component not found on instantiated prefab");
                }
            }
        }
    }
    
    private async void OnCurrentPartSelected(string partType, string roboId)
    {
        if (_customizingRoboData == null)
        {
            Debug.LogWarning("customizingRoboData is not initialized.");
            return;
        }

        // パーツタイプに応じて更新
        switch (partType)
        {
            case "Head":
                _customizingRoboData.headId = roboId;
                break;
            case "Body":
                _customizingRoboData.bodyId = roboId;
                break;
            case "Arms":
                _customizingRoboData.armsId = roboId;
                break;
            case "Legs":
                _customizingRoboData.legsId = roboId;
                break;
            case "Tail":
                _customizingRoboData.tailId = roboId;
                break;
        }

        // ロボの表示だけ反映（保存はしない）
        await RoboSettingManager.DisplayRobo(roboContainer, _customizingRoboData);
    }

 
    public void OnTappedGoToSelectScene()
    {
        var scene = SceneManager.GetSceneByName("SelectScene");
        foreach (var go in scene.GetRootGameObjects())
            if (go.name == "Canvas")
                go.SetActive(true);

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CustomScene"));
    }
    
    public async void OnTappedSaveRoboButton()
    {
        var userDataManager = UserDataManager.GetInstance();
        await userDataManager.SaveRoboCustomData(_currentRoboId, _customizingRoboData);
    }
}
