using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelectorButton : MonoBehaviour
{
    [SerializeField] private Text partTitle;
    
    public void SetPartSelectorButton(string part)
    {
        partTitle.text = part;
    }
}
