using UnityEngine;
using KetosGames.SceneTransition;

namespace KetosGames.SceneTransition
{
    /**
     * Attach this scrict to a GameObject that has a collider.
     * Set ChangeToScene to the scene you want to go to.
     * Set WhenTriggeredBy to another GameObject that has a trigger collider.
     */
    public class ChangeSceneFromTrigger : MonoBehaviour
    {
        public string ChangeToScene;
        public GameObject WhenTriggeredBy;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == WhenTriggeredBy)
            {
                SceneLoader.LoadScene(ChangeToScene);
            }
        }
    }
}
