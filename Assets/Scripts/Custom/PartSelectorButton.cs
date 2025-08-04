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
    [SerializeField] private Image partImage;
    
    public Action onPartSelected { get; set; }
    
    public void SetPartSelectorButton(string part)
    {
        partTitle.text = part;
        
        if (Const.DefaultPartImageMap.TryGetValue(part, out var spriteName))
        {
            var sprite = Resources.Load<Sprite>($"Images/Robo/{spriteName}");
            if (sprite != null)
            {
                partImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Sprite not found: Images/{spriteName}");
            }
        }
        else
        {
            Debug.LogWarning($"Part not found in map: {part}");
        }
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
