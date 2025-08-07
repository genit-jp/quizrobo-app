using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace KetosGames.SceneTransition
{
    public class LoadingSpinner : MonoBehaviour
    {
        public float halfRotationTime = 0.5f;

        private Quaternion startRotation;
        private Quaternion targetRotation;
        private float timeCount = 0;

        void Start()
        {
            startRotation = Quaternion.AngleAxis(0, Vector3.forward);
            targetRotation = Quaternion.AngleAxis(180, Vector3.forward);
        }

        void Update()
        {
            transform.rotation= Quaternion.Slerp(startRotation, targetRotation, timeCount / halfRotationTime);
            timeCount = timeCount + Time.deltaTime;
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                timeCount -= halfRotationTime;
            }
        }
    }
}
