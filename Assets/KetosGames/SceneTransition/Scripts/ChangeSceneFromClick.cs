using UnityEngine;
using System.Collections;
using KetosGames.SceneTransition;

namespace KetosGames.SceneTransition
{
    /**
     * Attach this scrict to a gameobject and set the ToScene property to the scene you want to load.
     * Set the click handler of a button to point to the GoToNextScene method.
     */
    public class ChangeSceneFromClick : MonoBehaviour
    {
        public string ToScene;

        public void GoToNextScene()
        {
            SceneLoader.LoadScene(ToScene);
        }
    }
}
