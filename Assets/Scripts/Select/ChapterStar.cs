using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterStar : MonoBehaviour
{
    [SerializeField] private Text chapterNumberText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject mask, kirakiraOdd, kirakiraEven;
    
    private int _maxChapterNumber;
    private string _subject;
    
    private void Start()
    {
        UserDataManager.GetInstance().AddChapterProgressDataUpdateListener(OnChapterProgressDataUpdated);
    }
    
    private void OnDestroy()
    {
        UserDataManager.GetInstance().RemoveChapterProgressDataUpdateListener(OnChapterProgressDataUpdated);
    }
    
    private void OnChapterProgressDataUpdated()
    {
        _maxChapterNumber = UserDataManager.GetInstance().GetChallengeLevel();
    }
    
    public void Setup(ChapterData data, System.Action<ChapterData> onClick, string subject)
    {
        _subject = subject;
        OnChapterProgressDataUpdated();
        chapterNumberText.text = data.chapterNumber.ToString();
        bool isLocked = data.chapterNumber > _maxChapterNumber + 1;
        mask.SetActive(isLocked);
        button.onClick.AddListener(() => onClick?.Invoke(data));
        
        // 奇数と偶数でキラキラの表示を切り替える
        if (data.chapterNumber % 2 == 0) // 偶数の場合
        {
            kirakiraEven.SetActive(true);
            kirakiraOdd.SetActive(false);
        }
        else 
        {
            kirakiraOdd.SetActive(true);
            kirakiraEven.SetActive(false);
        }
        
    }
}
