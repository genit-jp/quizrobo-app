
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private RubyTextMeshProUGUI _text;
    Action _onClicked;

    public void Setup(string text, Action onClicked)
    {
        _text.uneditedText = text;
        _onClicked = onClicked;
    }
    
    public void OnClick()
    {
        _onClicked?.Invoke();
    }
}
