using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectButton : MonoBehaviour
{
   [SerializeField] private Text subjectName;
   
   public Action onClicked;
   
   public void Setup(string subjectName, Action onClicked)
   {
       this.onClicked = onClicked;
       this.subjectName.text = subjectName;
   }
   
   public void OnClick()
    {
         onClicked?.Invoke();
    }
}
