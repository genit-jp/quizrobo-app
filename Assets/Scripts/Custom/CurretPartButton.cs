using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurretPartButton : MonoBehaviour
{
    [SerializeField] private Image partImage, backGround;
    [SerializeField] private GameObject mask, selectedImage;
    
    public string PartId { get; private set; }
    public Action onPartSelected { get; set; }
    
    public void SetPartButton(string partId)
    {
        PartId = partId;
        string resourcePath = $"Images/Robo/{partId}";
        var sprite = Resources.Load<Sprite>(resourcePath);
        bool isOwned = UserDataManager.GetInstance().IsRoboPartOwned(partId);
        mask.SetActive(!isOwned);

        partImage.sprite = sprite;
          
    }
    
    public void SetSelected(bool isSelected)
    {
       selectedImage.SetActive(isSelected);
    }
    
    public void OnPartButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
