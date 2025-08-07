using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KetosGames.SceneTransition
{
	public class LoadingBar : MonoBehaviour
	{
		void Update()
		{
			GetComponent<Image>().fillAmount = SceneLoader.Instance.Progress;
		}
	}
}
