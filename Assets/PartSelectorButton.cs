using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelectorButton : MonoBehaviour
{
    [SerializeField] private Text partTitle;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartSelectorButton(string part)
    {
        partTitle.text = part;
    }
    
    public void OnButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
