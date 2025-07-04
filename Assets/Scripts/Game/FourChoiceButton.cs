using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FourChoiceButton : MonoBehaviour
{
    [SerializeField]private RubyTextMeshProUGUI _text;
    private Action<string> _onClicked;
    public void Setup(string choice, Action<string> onClicked)
    {
        _text.uneditedText = choice;
        _onClicked = onClicked;
    }
    
    public void OnClick()
    {
        _onClicked?.Invoke(_text.uneditedText);
    }
}