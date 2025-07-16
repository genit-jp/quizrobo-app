using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurretPartButton : MonoBehaviour
{
    [SerializeField] private Image partImage;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartButton(string partId)
    {
        string resourcePath = $"Images/Robo/{partId}";
        var sprite = Resources.Load<Sprite>(resourcePath);

        partImage.sprite = sprite;
          
    }
    
    public void OnPartButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
