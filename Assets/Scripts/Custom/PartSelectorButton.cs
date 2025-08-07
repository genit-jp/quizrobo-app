using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelectorButton : MonoBehaviour
{
    [SerializeField] private Text partTitle;
    [SerializeField] private Image partImage;
    [SerializeField] private List<Sprite> partSprites;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartSelectorButton(string part)
    {
        partTitle.text = part;
        
        switch (part)
        {
            case "あたま":
                partImage.sprite = partSprites.Count > 0 ? partSprites[0] : null;
                break;
            case "からだ":
                partImage.sprite = partSprites.Count > 1 ? partSprites[1] : null;
                break;
            case "うで":
                partImage.sprite = partSprites.Count > 2 ? partSprites[2] : null;
                break;
            case "あし":
                partImage.sprite = partSprites.Count > 3 ? partSprites[3] : null;
                break;
            case "しっぽ":
                partImage.sprite = partSprites.Count > 4 ? partSprites[4] : null;
                break;
            default:
                Debug.LogWarning($"Unknown part name: {part}");
                partImage.sprite = null;
                break;
        }
    }
    
    public void SetSelected(bool isSelected)
    {
       //選ばれたときの処理
    }
    
    public void OnButtonClicked()
    {
        onPartSelected?.Invoke();
    }
}
