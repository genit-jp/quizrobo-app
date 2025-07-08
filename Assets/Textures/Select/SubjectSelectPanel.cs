using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubjectSelectPanel : MonoBehaviour
{
    public void Setup()
    {
        this.gameObject.SetActive(true);
    }
    
    public void OnClickGoBackButton()
    {
        Destroy(this.gameObject);
    }
}
