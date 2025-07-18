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
    private bool _isDisplayingRobo = false;
    private PartSelectorButton _currentSelectedPartButton;
    private CurretPartButton _currentSelectedCurrentPartButton;


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
        
        RefreshRoboDisplay();
       
       CreatePartSelectButtons();
    }
    
    private async void CreatePartSelectButtons()
    {
        // 既存の子オブジェクトをクリア
        foreach (Transform child in partSelectorPanel.transform)
        {
            Destroy(child.gameObject);
        }

        string defaultPartKey = "Head";
        PartSelectorButton defaultButton = null;

        // PART_NAMESの各パーツに対してボタンを作成
        foreach (var partEntry in Const.PART_NAMES)
        {
            var partKey = partEntry.Key; 
            var partJapaneseName = partEntry.Value;  
            var buttonObj = await Utils.InstantiatePrefab("Prefabs/Custom/PartSlectorButton", partSelectorPanel.transform);
        
            var partSelectorButton = buttonObj.GetComponent<PartSelectorButton>();
            partSelectorButton.SetPartSelectorButton(partJapaneseName);
            
            if (partKey == defaultPartKey)
            {
                defaultButton = partSelectorButton;
            }

            partSelectorButton.SetSelected(false); // 全部最初は未選択

            // Actionを設定
            string capturedPartKey = partKey;
            PartSelectorButton capturedButton = partSelectorButton;
            partSelectorButton.onPartSelected = () => OnPartButtonClicked(capturedPartKey, capturedButton);
        }
        
        if (defaultButton != null)
        {
            defaultButton.SetSelected(true);
            _currentSelectedPartButton = defaultButton;
            OnPartButtonClicked(defaultPartKey, defaultButton);
        }
    }

    
    private void OnPartButtonClicked(string partKey, PartSelectorButton clickedButton)
    {
        if (Const.PART_NAMES.TryGetValue(partKey, out string japaneseName))
        {
            selectedPartText.text = japaneseName;
        }
        
        // 前に選択されていたボタンの選択を解除
        if (_currentSelectedPartButton != null)
        {
            _currentSelectedPartButton.SetSelected(false);
        }
        
        // 新しいボタンを選択状態に
        clickedButton.SetSelected(true);
        _currentSelectedPartButton = clickedButton;
        
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
        
        string currentPartId = partType switch
        {
            "Head" => _customizingRoboData.headId,
            "Body" => _customizingRoboData.bodyId,
            "Arms" => _customizingRoboData.armsId,
            "Legs" => _customizingRoboData.legsId,
            "Tail" => _customizingRoboData.tailId,
            _ => null
        };
        
        // 各RoboDataに対してCurrentPartButtonを作成
        foreach (var roboData in roboDataArray)
        {
            var buttonObj = await Utils.InstantiatePrefab("Prefabs/Custom/PartButton", currentPartsPanel.transform);

            if (buttonObj != null)
            {
                var currentPartButton = buttonObj.GetComponent<CurretPartButton>();
                if (currentPartButton != null)
                {
                    currentPartButton.SetPartButton(roboData.id);

                    string capturedRoboId = roboData.id;
                    currentPartButton.onPartSelected = () => OnCurrentPartSelected(partType, capturedRoboId);

                    var button = buttonObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => currentPartButton.OnPartButtonClicked());
                    }

                    // 初期選択を反映
                    if (roboData.id == currentPartId)
                    {
                        currentPartButton.SetSelected(true);
                        _currentSelectedCurrentPartButton = currentPartButton;
                    }
                    else
                    {
                        currentPartButton.SetSelected(false);
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
        
        if (_currentSelectedCurrentPartButton != null)
        {
            _currentSelectedCurrentPartButton.SetSelected(false);
        }

        // 新しく選ばれたボタンを探して SetSelected(true)
        var allButtons = currentPartsPanel.GetComponentsInChildren<CurretPartButton>();
        foreach (var btn in allButtons)
        {
            if (btn.PartId == roboId)
            {
                btn.SetSelected(true);
                _currentSelectedCurrentPartButton = btn;
                break;
            }
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
        RefreshRoboDisplay();
    }

    
    private async void RefreshRoboDisplay()
    {
        if (_isDisplayingRobo) return;

        _isDisplayingRobo = true;
        try
        {
            await RoboSettingManager.DisplayRobo(roboContainer, _customizingRoboData);
        }
        finally
        {
            _isDisplayingRobo = false;
        }
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
