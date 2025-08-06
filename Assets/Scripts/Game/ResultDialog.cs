using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class ResultDialog: DialogBaseListener
{
    [SerializeField] private GameObject _scrollViewContent, _roboContent, rewardGetImage;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Image rewardItemImage;
    
    private Action _onGoToNextButtonClicked;
    private Action _onGoToSelectButtonClicked;
    private bool _userInteracted = false;

    private MasterData _masterData;
    private Dictionary<string, UserDataManager.RoboCustomData> _customRoboData;

    public async void Setup(List<QuizResultData> quizResults, Action onGoToNextButtonClicked, Action onGoToSelectButtonClicked)
    {
        _onGoToNextButtonClicked = onGoToNextButtonClicked;
        _onGoToSelectButtonClicked = onGoToSelectButtonClicked;

        var prefabPath = "Prefabs/Game/ResultContent";
        var resource = (GameObject)await Resources.LoadAsync(prefabPath);
        foreach (var quizResult in quizResults)
        {
            var gameObj = Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

            gameObj.transform.SetParent(_scrollViewContent.transform, false);
            var resultContent = gameObj.GetComponent<ResultContent>();
            resultContent.Setup(quizResult);
        }

        var userDataManager = UserDataManager.GetInstance();
        _customRoboData = userDataManager.GetRoboCustomData(userDataManager.GetUserData().selectedRoboId);
        if (_customRoboData != null && _customRoboData.ContainsKey(userDataManager.GetUserData().selectedRoboId))
        {
            var roboCustomData = _customRoboData[userDataManager.GetUserData().selectedRoboId];
            await RoboSettingManager.DisplayRobo(_roboContent, roboCustomData);
        }

        // プレイヤーステータスを表示
        var playerStatus = userDataManager.GetPlayerStatus();
        int currentExp = playerStatus.exp;

        // 次のレベルまでに必要なEXP
        var ownedRoboId = UserDataManager.GetInstance().OwnedRoboPartsIds();
        _masterData = MasterData.GetInstance();
        var roboData = _masterData.GetNextUnownedRoboByExp(ownedRoboId);
        int expForNextLevel = roboData.exp_required;

        expSlider.value = (float)currentExp / expForNextLevel;

        // 報酬ロボパーツの画像を設定
        if (!string.IsNullOrEmpty(roboData.id))
        {
            var rewardRoboId = roboData.id;
            string spritePath = $"Images/Robo/{rewardRoboId}";
            Sprite roboSprite = Resources.Load<Sprite>(spritePath);

            if (roboSprite != null && rewardItemImage != null)
            {
                rewardItemImage.sprite = roboSprite;
            }
            else
            {
                Debug.LogWarning($"Reward sprite not found at path: {spritePath}");
            }
        }

        //新規取得ロボパーツの表示
        var unclaimedIds = userDataManager.GetUnclaimedRewardRoboPartIds(currentExp);
        if (unclaimedIds == null) return;
        {
            rewardGetImage.SetActive(true);
            foreach (var partId in unclaimedIds)
            {
                // 🎨 画像表示やアニメ演出
                string spritePath = $"Images/Robo/{partId}";
                var sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null)
                {
                    rewardItemImage.sprite = sprite;
                    await UniTask.Delay(1500); // 演出時間
                    
                    var userData = userDataManager.GetUserData();
                    var roboId = userData.selectedRoboId ?? "default";
                    
                    _customRoboData = userDataManager.GetRoboCustomData(roboId);
                    if (_customRoboData.TryGetValue(roboId, out var roboCustomData))
                    {
                        
                        var newAvailableRoboData = _masterData.GetRoboDataById(partId);

                        if (newAvailableRoboData != null)
                        {
                            var type = newAvailableRoboData.type.ToLower(); // "arms" など

                            switch (type)
                            {
                                case "head": roboCustomData.headId = partId; break;
                                case "body": roboCustomData.bodyId = partId; break;
                                case "arms": roboCustomData.armsId = partId; break;
                                case "legs": roboCustomData.legsId = partId; break;
                                case "tail": roboCustomData.tailId = partId; break;
                                default:
                                    Debug.LogWarning($"Unknown part type '{type}' for partId: {partId}");
                                    break;
                            }

                            await userDataManager.SaveRoboCustomData(roboId, roboCustomData);
                        }
                        else
                        {
                            Debug.LogWarning($"No RoboData found for partId: {partId}");
                        }
                    }

                }

                // ✅ purchased = true に更新
                await userDataManager.AddOwnedRoboPart(partId, true);
            }
        }
        
        await UniTask.Delay(3000);

        if (!_userInteracted)
        {
            Debug.Log("3秒経過。自動で次のチャプターに進みます");
            Destroy(gameObject);
            _onGoToNextButtonClicked?.Invoke();
        }
    }

    public void OnClickGoToSelect()
    {
        _userInteracted = true;
        _onGoToSelectButtonClicked?.Invoke();
    }

    public void OnClickGoToNextButton()
    {
        Destroy(gameObject);
        _userInteracted = true;
        _onGoToNextButtonClicked?.Invoke();
    }
    
    public override bool OnClickBlocker()
    {
        _userInteracted = true;
        return false;
    }
}