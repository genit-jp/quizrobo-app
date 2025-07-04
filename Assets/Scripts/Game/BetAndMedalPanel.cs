using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BetAndMedalPanel: MonoBehaviour
{
    [SerializeField]private Text _medalNumText;
    [SerializeField]private Text _betNumText;
    [SerializeField]private Text _gradeText;
    [SerializeField]private Text _starText;
    [SerializeField]private Text _countText;
    [SerializeField]private GameObject _betButton;
    [SerializeField]private GameObject _countPanel;
    private Action _onClicked;
    
    public void Setup(Action onClicked)
    {
        // if (QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Calculation)
        // {
        //     _betButton.SetActive(false);
        //     _countPanel.SetActive(true);
        // }
        
        _onClicked = onClicked;
    }
    
    public void SetMedalNum(int medalNum)
    {
        _medalNumText.text = medalNum.ToString();
    }
    
    public void SetBetNum(int betNum)
    {
        _betNumText.text = betNum.ToString();
    }
    
    public void SetTargetGrade(int grade)
    {
        _gradeText.text = grade.ToString() + "年生";
    }
    
    public void SetCount(int count)
    {
        _countText.text = count.ToString();
        if (count <= 3)
        {
            _countPanel.GetComponent<Image>().color = new Color(0.89f, 0.39f, 0.33f, 1);
        }
        else
        {
            _countPanel.GetComponent<Image>().color = new Color(53/255f, 53/255f, 53/255f, 1);

        }
    }
    
    public void SetLevelStar(int levelNum)
    {
        switch (levelNum)
        {
            case 1:
                _starText.text = "★☆☆☆☆";
                break;
            case 2:
                _starText.text = "★★☆☆☆";
                break;
            case 3:
                _starText.text = "★★★☆☆";
                break;
            case 4:
                _starText.text = "★★★★☆";
                break;
            case 5:
                _starText.text = "★★★★★";
                break;
        }
    }
    
    public void OnClick()
    {
        _onClicked?.Invoke();
    }
    
    public void DisableBetButton()
    {
        _betButton.GetComponent<Button>().interactable = false;
    }
    
    public void EnableBetButton()
    {
        _betButton.GetComponent<Button>().interactable = true;
    }
}