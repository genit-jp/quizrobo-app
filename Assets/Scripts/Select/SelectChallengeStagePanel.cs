using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectChallengeStagePanel : MonoBehaviour
{
    [SerializeField] private Text title;
    public void Setup(string subject)
    {
        title.text = subject;
    }
    
    public void OnClickCloseButton()
    {
        Destroy(this.gameObject);
    }
}
