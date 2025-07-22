using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusUi : MonoBehaviour
{
    [SerializeField] private Slider hpGauge;
    
    
    public void SetHP(float value)
    {
        hpGauge.value = value;
    }
    
}
