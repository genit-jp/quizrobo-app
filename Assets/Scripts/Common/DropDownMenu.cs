using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject dropMenuButtonPrefab;
    [SerializeField] private Button backgroundButton;
    
    private Action<string> _onSelect;
    
    public void Setup(List<string> options, Action<string> onSelectCallback)
    {
        _onSelect = onSelectCallback;
        
        backgroundButton.onClick.RemoveAllListeners();
        backgroundButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        
        foreach (Transform child in content)
            Destroy(child.gameObject);
        
        foreach (var option in options)
        {
            var btn = Instantiate(dropMenuButtonPrefab, content);
            btn.GetComponentInChildren<Text>().text = Const.DIFFICULTY_NAME_MAP[option];
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                _onSelect?.Invoke(option);
                gameObject.SetActive(false);
            });
        }
    }
    
}
