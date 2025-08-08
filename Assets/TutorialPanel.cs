using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public void Setup()
    {
        // Initialize the tutorial panel here if needed
        // For example, you can set up UI elements or load resources
        Debug.Log("Tutorial Panel Initialized");
    }

    public void OnTapClose()
    {
        Destroy(gameObject);
    }
}
