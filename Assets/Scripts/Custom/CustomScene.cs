using System.Collections;
using System.Collections.Generic;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomScene : MonoBehaviour
{
    [SerializeField] private GameObject roboContainer;
    
    private async void Start()
    {
       await RoboSettingManager.DisplayRobo(roboContainer);
    }
 
    public void OnTappedGoToSelectScene()
    {
        var scene = SceneManager.GetSceneByName("SelectScene");
        foreach (var go in scene.GetRootGameObjects())
            if (go.name == "Canvas")
                go.SetActive(true);

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CustomScene"));
    }
}
