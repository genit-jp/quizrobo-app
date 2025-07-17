using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterStar : MonoBehaviour
{
    [SerializeField] private Text chapterNumberText;
    [SerializeField] private Button button;
    public void Setup(ChapterData data, System.Action<ChapterData> onClick)
    {
        chapterNumberText.text = data.chapterNumber.ToString();

        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
