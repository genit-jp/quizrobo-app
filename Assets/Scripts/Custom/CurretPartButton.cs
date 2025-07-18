using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurretPartButton : MonoBehaviour
{
    [SerializeField] private Image partImage;
    [SerializeField] private GameObject mask;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartButton(string partId)
    {
        string resourcePath = $"Images/Robo/{partId}";
        var sprite = Resources.Load<Sprite>(resourcePath);
        bool isOwned = UserDataManager.GetInstance().IsRoboPartOwned(partId);
        mask.SetActive(!isOwned);

        partImage.sprite = sprite;
          
    }
    
    public void OnPartButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
