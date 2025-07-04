using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PictureBookPanel : MonoBehaviour
{
    [SerializeField]private RubyTextMeshProUGUI _page;
    [SerializeField]private RubyTextMeshProUGUI _description;
    [SerializeField]private GameObject _descriptionPanel;
    [SerializeField]private GameObject _endPanel;
    [SerializeField]private Image _image;
    [SerializeField]private GestureController _gestureController;
    [SerializeField]private GameObject _leftArrow;
    [SerializeField]private GameObject _rightArrow;
    [SerializeField]private Animator _leftArrowAnimator;
    [SerializeField]private Animator _rightArrowAnimator;
    [SerializeField]private GameObject _lockPanel;
    [SerializeField]private AudioSource _audioSource;
    
    List<PictureBookData> _pictureBookDataList = new List<PictureBookData>();
    private int _currentDataIndex;
    private int _flickDataIndex;
    private Animator _primaryAnimator;
    private Animator _secondaryAnimator;

    private class PictureBookData
    {
        public int Page;
        public string Description;
    }
    
    private void Start()
    {
        _gestureController.rightFlick += RightFlick;
        _gestureController.leftFlick += LeftFlick;
        _primaryAnimator = _leftArrowAnimator;
        _secondaryAnimator = _rightArrowAnimator;
    }

    private void Update()
    {
        // アニメーションの同期
        AnimatorStateInfo stateInfo = _primaryAnimator.GetCurrentAnimatorStateInfo(0);
        _secondaryAnimator.Play(stateInfo.fullPathHash, -1, stateInfo.normalizedTime);
    }

    public void Setup()
    {
        _pictureBookDataList = LoadPictureBookData();
        
        _currentDataIndex = FindCurrentDataIndex();
        _flickDataIndex = _currentDataIndex;
        SetData(_currentDataIndex);
        // SetSlider(_currentDataIndex);
    }

    private void SetData(int index)
    {
        if (index < 12)
        {
            PlayBGM("bgm_start");
        }
        else if (index < 13)
        {
            _audioSource.Stop();
        }
        else if (index < 17)
        {
            PlayBGM("bgm_sad");
        }
        else if (index < 21)
        {
            PlayBGM("bgm_semiend");
        }
        else
        {
            PlayBGM("bgm_end");
        }

        
        if(_flickDataIndex == 0)
        {
            _primaryAnimator = _rightArrowAnimator;
            _secondaryAnimator = _leftArrowAnimator;
            _leftArrow.SetActive(false);
        }
        else
        {
            _leftArrow.SetActive(true);
        }
        
        if(_flickDataIndex == _currentDataIndex　+ 1)
        {
            _primaryAnimator = _leftArrowAnimator;
            _secondaryAnimator = _rightArrowAnimator;
            _rightArrow.SetActive(false);
        }
        else
        {
            _rightArrow.SetActive(true);
        }
        
        if (index > _pictureBookDataList.Count - 1)
        {
            _endPanel.SetActive(true);
            _lockPanel.SetActive(false);
            _description.uneditedText = "あそんでくれてありがとう！\nレビューしてあたらしいストーリーをまっててね！";
            _page.uneditedText = "";
        }
        else if (index > _currentDataIndex)
        {
            PictureBookData data = _pictureBookDataList[index];
            _lockPanel.SetActive(true);
            _endPanel.SetActive(false);
            _description.uneditedText = "クイズにちょうせんして\nストーリーを<r=かいほう>解放</r>しよう！";
            _page.uneditedText = data.Page.ToString();
            SetImage("p"+(data.Page).ToString());
        }
        else
        {
            PictureBookData data = _pictureBookDataList[index];
            _lockPanel.SetActive(false);
            _endPanel.SetActive(false);
            _descriptionPanel.SetActive(true);
            _page.uneditedText = data.Page.ToString();
            _description.uneditedText = data.Description;
            SetImage("p"+(data.Page).ToString());
        }
    }

    private List<PictureBookData> LoadPictureBookData()
    {
        if(_pictureBookDataList.Count > 0)
        {
            return _pictureBookDataList;
        }
        
        List<PictureBookData> dataList = new List<PictureBookData>();
    
        TextAsset pictureBookData = Resources.Load<TextAsset>("PictureBook/story_data");
        string[] dataLines = pictureBookData.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < dataLines.Length; i++)
        {
            string[] fields = dataLines[i].Split(',');
            if (fields.Length ==　2) 
            {
                PictureBookData data = new PictureBookData()
                {
                    Page = int.TryParse(fields[0], out int page) ? page : 0,
                    Description = fields[1].Replace("\\n", "\n")
                };
                dataList.Add(data);
            }
        }
    
        Debug.Log($"Loaded {dataList.Count} picture book data.");
        return dataList;
    }
    
    private int FindCurrentDataIndex()
    {
        if (_pictureBookDataList.Count == 0)
        {
            return 0;
        }

        // for (int i = 1; i < _pictureBookDataList.Count; i++)
        // {
        //     if (_pictureBookDataList[i].Medal > _totalMedal)
        //     {
        //         return i - 1;
        //     }
        // }
        // return _pictureBookDataList.Count - 1;
        
        var answerDataList = UserDataManager.GetInstance().GetAnswerDataList();
        var distinctOnePlayIds = answerDataList.Select(answer => answer.onePlayId).Distinct().ToList();
        
        if(distinctOnePlayIds.Count >= _pictureBookDataList.Count)
        {
            return _pictureBookDataList.Count - 1;
        }
        
        return distinctOnePlayIds.Count;
    }
    
    private void PlayBGM(string bgmName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"BGM/{bgmName}");
        if (_audioSource.clip == clip && _audioSource.isPlaying)
        {
            return; // 同じクリップが再生されている場合は何もしない
        }
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void SetImage(string imageName)
    {
        Sprite imageSprite = Resources.Load<Sprite>($"PictureBook/Images/{imageName}");
        if (imageSprite != null)
        {
            _image.sprite = imageSprite;
        }
        else
        {
            Debug.LogWarning($"Image not found: {imageName}");
        }
    }
    
    public void RightFlick()
    {
        if (_flickDataIndex > 0)
        {
            _flickDataIndex--;
            SetData(_flickDataIndex);
        }
    }

    public void LeftFlick()
    {
        if(_flickDataIndex < _currentDataIndex + 1){
            _flickDataIndex++;
            SetData(_flickDataIndex);
        }
    }
    
    public void OnClickReview()
    {
#if UNITY_IOS
        Application.OpenURL("https://apps.apple.com/jp/app/%E3%82%AF%E3%82%A4%E3%82%BA%E5%8D%9A%E5%A3%AB/id6476756838?l=en-US");
#endif
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=jp.genit.kidsquiz");
#endif
    }
}