using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterStar : MonoBehaviour
{
    [SerializeField] private Text chapterNumberText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject mask, kirakiraTrail;
    
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
        _maxChapterNumber = UserDataManager.GetInstance().GetMaxChapterNumber(_subject);
    }
    
    public void Setup(ChapterData data, System.Action<ChapterData> onClick, string subject)
    {
        _subject = subject;
        OnChapterProgressDataUpdated();
        chapterNumberText.text = data.chapterNumber.ToString();
        bool isLocked = data.chapterNumber > _maxChapterNumber + 1;
        mask.SetActive(isLocked);
        button.onClick.AddListener(() => onClick?.Invoke(data));
        
        // kirakiraTrailの回転設定
        if (kirakiraTrail != null)
        {
            var rectTransform = kirakiraTrail.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (data.chapterNumber % 2 == 0) // 偶数の場合
                {
                    // Y軸で180度回転させて鏡のように反転
                    rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                // 奇数の場合はそのまま（デフォルト値を維持）
            }
        }
    }
}
