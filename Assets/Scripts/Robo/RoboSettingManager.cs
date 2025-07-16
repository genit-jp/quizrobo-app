using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Genit;
using Cysharp.Threading.Tasks;

public class RoboSettingManager : MonoBehaviour
{
    /// <summary>
    /// 指定されたコンテナにRoboPrefabを表示する
    /// </summary>
    /// <param name="roboContainer">ロボを表示するコンテナ</param>
    /// <param name="roboCustomData">表示するロボのカスタムデータ</param>
    public static async UniTask DisplayRobo(GameObject roboContainer, UserDataManager.RoboCustomData roboCustomData)
    {
        
        // 既存の子オブジェクトをクリア
        foreach (Transform child in roboContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        // RoboPrefabをインスタンス化
        var roboPrefab = await Utils.InstantiatePrefab("Prefabs/Robo/RoboPrefab", roboContainer.transform);
        
        if (roboPrefab != null)
        {
            var roboComponent = roboPrefab.GetComponent<RoboPrefab>();
            if (roboComponent != null)
            {
                // 指定されたロボカスタムデータで表示
                roboComponent.SetRobo(roboCustomData);
                
                // Prefabのサイズをコンテナの縦幅に合わせる
                AdjustRoboSize(roboContainer, roboPrefab);
            }
            else
            {
                Debug.LogWarning("RoboPrefab component not found on instantiated prefab");
            }
        }
        else
        {
            Debug.LogWarning("Failed to instantiate RoboPrefab");
        }
    }
    
    /// <summary>
    /// ロボのサイズをコンテナに合わせて調整する
    /// </summary>
    private static void AdjustRoboSize(GameObject roboContainer, GameObject roboPrefab)
    {
        var containerRect = roboContainer.GetComponent<RectTransform>();
        var roboRect = roboPrefab.GetComponent<RectTransform>();
        
        if (containerRect != null && roboRect != null)
        {
            // コンテナの高さを取得
            float containerHeight = containerRect.rect.height;
            
            // ロボのアスペクト比を維持しながらスケールを調整
            float currentHeight = roboRect.rect.height;
            if (currentHeight > 0)
            {
                float scale = containerHeight / currentHeight;
                roboRect.localScale = new Vector3(scale, scale, 1f);
            }
            
            // 中央に配置
            roboRect.anchoredPosition = Vector2.zero;
        }
    }
}
