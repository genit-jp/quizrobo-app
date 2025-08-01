using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectButton : MonoBehaviour
{
   [SerializeField] private Image backgroundImage;
   
   [SerializeField] private Sprite mathSprite;
   [SerializeField] private Sprite japaneseSprite;
   [SerializeField] private Sprite scienceSprite;
   [SerializeField] private Sprite socialStudiesSprite;
   [SerializeField] private Sprite englishSprite;
   
   public Action onClicked;
   
   public void Setup(string subjectName, Action onClicked)
   {
       this.onClicked = onClicked;
       
       switch (subjectName)
       {
           case "算数":
               backgroundImage.sprite = mathSprite;
               break;
           case "国語":
               backgroundImage.sprite = japaneseSprite;
               break;
           case "理科":
               backgroundImage.sprite = scienceSprite;
               break;
           case "社会":
               backgroundImage.sprite = socialStudiesSprite;
               break;
           case "英語":
               backgroundImage.sprite = englishSprite;
               break;
           default:
               Debug.LogWarning($"未対応の教科: {subjectName}");
               break;
       }
   }
   
   public void OnClick()
    {
         onClicked?.Invoke();
    }
}
