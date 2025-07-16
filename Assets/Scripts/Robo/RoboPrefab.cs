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

    public void SetRobo()
    {
        SetRobo("default");
    }
    
    public void SetRobo(string roboId)
    {
        var userDataManager = UserDataManager.GetInstance();
        var roboCustomDataDict = userDataManager.GetRoboCustomData(roboId);
        
        if (roboCustomDataDict == null || roboCustomDataDict.Count == 0)
        {
            Debug.LogWarning($"RoboCustomData not found for roboId: {roboId}");
            return;
        }
        
        // Dictionary から RoboCustomData を取得
        UserDataManager.RoboCustomData roboCustomData = null;
        foreach (var kvp in roboCustomDataDict)
        {
            roboCustomData = kvp.Value;
            break;
        }
        
        if (roboCustomData == null)
        {
            Debug.LogWarning($"RoboCustomData is null for roboId: {roboId}");
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
        }
        else
        {
            var spriteRenderer = partObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"No Image or SpriteRenderer component found on {partObject.name}");
            }
        }
    }
}
