using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject dropMenuButtonPrefab;
    
    private Action<string> onSelect;
    
    public void Setup(List<string> options, Action<string> onSelectCallback)
    {
        onSelect = onSelectCallback;
        
        foreach (Transform child in content)
            Destroy(child.gameObject);
        
        foreach (var option in options)
        {
            var btn = Instantiate(dropMenuButtonPrefab, content);
            btn.GetComponentInChildren<Text>().text = option;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                onSelect?.Invoke(option);
                gameObject.SetActive(false);
            });
        }
    }
    
}
