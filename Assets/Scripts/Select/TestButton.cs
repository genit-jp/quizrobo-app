
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    Action<int> _onClicked;
    private int _grade;

    public void Setup(Action<int> onClicked)
    {
        _grade = 3;
        _onClicked = onClicked;
    }
    
    public void OnClick()
    {
        _onClicked?.Invoke(_grade);
    }
}
