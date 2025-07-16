using System.Collections;
using System.Collections.Generic;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomScene : MonoBehaviour
{
    [SerializeField] private GameObject roboContainer,partSelectorPanel;
    [SerializeField] private Text selectedPartText;
    
    private async void Start()
    {
       await RoboSettingManager.DisplayRobo(roboContainer);
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
        
        // TODO: パーツ選択時のその他の処理を実装
    }
 
    public void OnTappedGoToSelectScene()
    {
        var scene = SceneManager.GetSceneByName("SelectScene");
        foreach (var go in scene.GetRootGameObjects())
            if (go.name == "Canvas")
                go.SetActive(true);

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CustomScene"));
    }
}
