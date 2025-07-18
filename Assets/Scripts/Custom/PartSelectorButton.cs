using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelectorButton : MonoBehaviour
{
    [SerializeField] private Text partTitle;
    [SerializeField] private Image backGround;
    [SerializeField] private Sprite notSelected,selected;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartSelectorButton(string part)
    {
        partTitle.text = part;
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
    
    public void OnButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
