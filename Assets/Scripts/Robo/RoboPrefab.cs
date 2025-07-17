using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboPrefab : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject arms;
    [SerializeField] private GameObject legs;
    [SerializeField] private GameObject tail;
    
    public void SetRobo(UserDataManager.RoboCustomData roboCustomData)
    {
        if (roboCustomData == null)
        {
            Debug.LogWarning("RoboCustomData is null");
            return;
        }
        
        LoadRoboPart(head, roboCustomData.headId);
        LoadRoboPart(body, roboCustomData.bodyId);
        LoadRoboPart(arms, roboCustomData.armsId);
        LoadRoboPart(legs, roboCustomData.legsId);
        LoadRoboPart(tail, roboCustomData.tailId);
    }
    
    private void LoadRoboPart(GameObject partObject, string partId)
    {
        if (partObject == null || string.IsNullOrEmpty(partId))
        {
            Debug.LogWarning($"Invalid part object or partId: {partId}");
            return;
        }
        
        string resourcePath = $"Images/Robo/{partId}";
        
        
        var sprite = Resources.Load<Sprite>(resourcePath);
        
        if (sprite == null)
        {
            Debug.LogWarning($"Failed to load sprite from path: {resourcePath}");
            return;
        }
        
        var image = partObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = sprite;
            
            // RectTransformを取得してサイズを調整
            var rectTransform = partObject.GetComponent<RectTransform>();
            if (rectTransform != null && sprite != null)
            {
                // スプライトの実際のサイズを取得
                float spriteWidth = sprite.rect.width;
                float spriteHeight = sprite.rect.height;
                
                // 現在のアンカー設定を保持したまま、サイズを変更
                rectTransform.sizeDelta = new Vector2(spriteWidth, spriteHeight);
            }
        }
        else
        {
            var spriteRenderer = partObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                // SpriteRendererの場合は自動的にスプライトサイズに合わせられる
            }
            else
            {
                Debug.LogWarning($"No Image or SpriteRenderer component found on {partObject.name}");
            }
        }
    }
}
