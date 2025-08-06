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
    private Action _onOkButtonClicked;

    public async void Setup(List<QuizResultData> quizResults, Action onOkButtonClicked)
    {
        _onOkButtonClicked = onOkButtonClicked;

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
        var roboCustomDataDict = userDataManager.GetRoboCustomData(userDataManager.GetUserData().selectedRoboId);
        if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(userDataManager.GetUserData().selectedRoboId))
        {
            var roboCustomData = roboCustomDataDict[userDataManager.GetUserData().selectedRoboId];
            await RoboSettingManager.DisplayRobo(_roboContent, roboCustomData);
        }

        // プレイヤーステータスを表示
        var playerStatus = userDataManager.GetPlayerStatus();
        int currentExp = playerStatus.exp;

        // 次のレベルまでに必要なEXP
        var ownedRoboId = UserDataManager.GetInstance().OwnedRoboPartsIds();
        var roboData = MasterData.GetInstance().GetNextUnownedRoboByExp(ownedRoboId);
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
                }

                // ✅ purchased = true に更新
                await userDataManager.AddOwnedRoboPart(partId, true);
            }
        }
    }


    public void OnClickHomeButton()
    {
        _onOkButtonClicked?.Invoke();
    }
    
    public override bool OnClickBlocker()
    {
        return false;
    }
}