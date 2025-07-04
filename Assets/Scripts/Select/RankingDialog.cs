using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RankingDialog : DialogBaseListener
{
    [SerializeField] private Transform _scrollViewContentTransform;
    [SerializeField] RankingContent _playerRankingContent;
    
    private int _playerRank = -1;
    public async void Setup()
    {
        var playerUserData = UserDataManager.GetInstance().GetUserData();
        
        var rankingUserDataList = await UserDataManager.GetInstance().GetRankingUserData();
        int previousTotalMedal = 0;
        int rank = 0;
        var prefabPath = "Prefabs/Select/RankingContent";
        var resource = (GameObject)await Resources.LoadAsync(prefabPath);
        foreach (var rankingUserData in rankingUserDataList)
        {
            if(previousTotalMedal != rankingUserData.totalMedal)
            {
                rank++;
            }
            if (rank > 100)
            {
                break;
            }
            
            var gameObj = Instantiate(resource, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            gameObj.transform.SetParent(_scrollViewContentTransform, false);
            var rankingContent = gameObj.GetComponent<RankingContent>();
            rankingContent.Setup(rankingUserData, rank);
            
            previousTotalMedal = rankingUserData.totalMedal;
            
            if (playerUserData.totalMedal == rankingUserData.totalMedal)
            {
                _playerRank = rank;
            }
        }
        _playerRankingContent.Setup(playerUserData, _playerRank);
    }

    public void OnClickOk()
    {
        Close();
    }

    public async void OnClickChangedName()
    {
        var changeNameDialogObj = await Genit.Utils.OpenDialog("Prefabs/Select/ChangeNameDialog", transform);
        var changeNameDialog = changeNameDialogObj.GetComponent<ChangeNameDialog>();
        changeNameDialog.Setup(() =>
            {
                _playerRankingContent.Setup(UserDataManager.GetInstance().GetUserData(), _playerRank);
            }
            , UserDataManager.GetInstance().GetUserData());
    }

    public override bool OnClickBlocker()
    {
        return false;
    }
}