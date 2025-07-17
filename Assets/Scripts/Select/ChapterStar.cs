using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterStar : MonoBehaviour
{
    [SerializeField] private Text chapterNumberText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject mask;
    public void Setup(ChapterData data, System.Action<ChapterData> onClick, bool isLocked)
    {
        chapterNumberText.text = data.chapterNumber.ToString();
        mask.SetActive(isLocked);
        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
