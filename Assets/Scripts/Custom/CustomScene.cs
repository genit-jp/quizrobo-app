using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaboScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTappedGoToSelectScene()
    {
        var scene = SceneManager.GetSceneByName("SelectScene");
        foreach (var go in scene.GetRootGameObjects())
            if (go.name == "Canvas")
                go.SetActive(true);

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LaboScene"));
    }
}
