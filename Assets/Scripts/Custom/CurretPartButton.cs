using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurretPartButton : MonoBehaviour
{
    [SerializeField] private Image partImage, backGround;
    [SerializeField] private Sprite notSelected, selected;
    [SerializeField] private GameObject mask;
    
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
        if (isSelected)
        {
            backGround.sprite = selected;
        }
        else
        {
            backGround.sprite = notSelected;
        }
    }
    
    public void OnPartButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
