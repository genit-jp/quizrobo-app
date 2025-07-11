using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterStar : MonoBehaviour
{
    [SerializeField] private Text chapterNumberText;
    [SerializeField] private GameObject rewardBalloon;
    [SerializeField] private Button button;
    public void Setup(ChapterData data, System.Action<ChapterData> onClick)
    {
        chapterNumberText.text = data.chapterNumber.ToString();

        // 吹き出し表示制御
        if (data.rewardRobotId != null)
        {
            rewardBalloon.SetActive(true);

            float offsetX = data.chapterNumber % 2 == 0 ? -150f : 150f;
            rewardBalloon.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetX, 100f);
        }
        else
        {
            rewardBalloon.SetActive(false);
        }

        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
