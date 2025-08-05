// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using Genit;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class SelectChallengeStagePanel : MonoBehaviour
// {
//
//
//     private void Start()
//     {
//
//     }
//
//     private void OnDestroy()
//     {
//         
//     }
//
//     private void OnUserDataUpdated()
//     {
//         foreach (Transform child in chapterButtonParent)
//         {
//             Destroy(child.gameObject);
//         }
//
//         var challengeLevel = UserDataManager.GetInstance().GetChallengeLevel();
//         _chapters = MasterData.GetInstance().GetChaptersBySubjectAndLevel(_subject, challengeLevel.ToString());
//         SetChapterButtons();
//     }
//     
//     public void Setup(string subject, Action startGame)
//     {
//         _subject = "算数"; // ← ここで固定
//     
//         title.text = _subject;
//         _onStartGame = startGame;
//
//         // 背景画像も算数で固定
//         int spriteIndex = 1; // 算数 → index 1
//         if (backGround != null && backgroundSprites != null && spriteIndex < backgroundSprites.Length)
//         {
//             backGround.sprite = backgroundSprites[spriteIndex];
//         }
//
//         SetChapterButtons();
//     }
//
//
//     private async void SetChapterButtons()
// {
//     foreach (Transform child in chapterButtonParent)
//     {
//         Destroy(child.gameObject); // 既存のボタンを削除
//     }
//
//     // StartPoint を配置
//     var startPointObj = await Utils.InstantiatePrefab("Prefabs/Select/SubjectSelect/StartPoint", chapterButtonParent);
//     var startPointRect = startPointObj.GetComponent<RectTransform>();
//     startPointRect.anchorMin = startPointRect.anchorMax = new Vector2(0.5f, 1f);
//     startPointRect.pivot = new Vector2(0.5f, 1f);
//     startPointRect.anchoredPosition = new Vector2(0, -300f);
//
//     // enemy_data.txt を読み込み
//     var enemyDataText = Resources.Load<TextAsset>("Data/enemy_data");
//     if (enemyDataText == null)
//     {
//         Debug.LogError("enemy_data.txt not found");
//         return;
//     }
//
//     string[] lines = enemyDataText.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
//     int chapterCount = lines.Length;
//     int maxClearedChapterNumber = UserDataManager.GetInstance().GetMaxChapterNumber("算数");
//
//     float offsetX = 150f;
//     float verticalOffset = 450f;
//
//     for (int i = 0; i < chapterCount; i++)
//     {
//         int chapterNumber = i + 1;
//
//         var chapterButton = Instantiate(Resources.Load<ChapterStar>("Prefabs/Select/SubjectSelect/ChapterStar"), chapterButtonParent);
//         
//         // chapterData を自前で構築（subject は固定で "算数"）
//         ChapterData chapterData = new ChapterData
//         {
//             chapterNumber = chapterNumber,
//             subject = "算数"
//         };
//
//         chapterButton.Setup(chapterData, ShowChallengeDialog, "算数");
//
//         var rect = chapterButton.GetComponent<RectTransform>();
//         rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
//         rect.pivot = new Vector2(0.5f, 1f);
//
//         float buttonHeight = rect.sizeDelta.y * 0.7f;
//         float y = verticalOffset + i * buttonHeight;
//         float x = (i % 2 == 0) ? offsetX : -offsetX;
//
//         rect.anchoredPosition = new Vector2(x, -y);
//
//         if (chapterNumber == maxClearedChapterNumber)
//         {
//             _currentChapterStar = chapterButton;
//         }
//     }
//
//     PlaceRobotOnChapter();
// }
//
//     
//     public void OnClickLevelChangeButton()
//     {
//         dropDownMenu.gameObject.SetActive(true);
//         var options = new List<string>(Const.DIFFICULTY_NAME_MAP.Keys);
//         dropDownMenu.GetComponent<DropDownMenu>().Setup(options, OnSelectLevel);
//     }
//
//     private async void OnSelectLevel(string challengeLevel)
//     {
//         await UserDataManager.GetInstance().SetChallengeLevel(int.Parse(challengeLevel));
//     }
//     
//     private async void ShowChallengeDialog(ChapterData data)
//     {
//         
//         var go = await Genit.Utils.OpenDialog("Prefabs/Common/CommonDialog", this.gameObject.transform);
//         var cd = go.GetComponent<CommonDialog>();
//         var challengeTitle = $"{data.chapterNumber}にチャレンジする？";
//         cd.Setup(challengeTitle, challengeTitle, async result => 
//         {
//             if (result == CommonDialog.Result.OK)
//             {
//                 Const.GameSceneParam.Subject = data.subject;
//                 Const.GameSceneParam.DifficultyLevel = data.difficultyLevel;
//                 Const.GameSceneParam.ChapterNumber = data.chapterNumber;
//                 
//                 // ダイアログのアニメーションが完了するのを待つ
//                 await UniTask.Delay(150);
//                 
//                 _onStartGame();
//             }
//         }, CommonDialog.Mode.OK_CANCEL);
//     }
//
//     private async void PlaceRobotOnChapter()
//     {
//         RectTransform targetRect = null;
//         float adjustedY = 0f;
//
//         if (_currentChapterStar != null)
//         {
//             targetRect = _currentChapterStar.GetComponent<RectTransform>();
//             adjustedY = 100f;
//         }
//         else
//         {
//             // StartPoint にロボを配置（最初に生成されていると仮定）
//             foreach (Transform child in chapterButtonParent)
//             {
//                 if (child.name.Contains("StartPoint"))
//                 {
//                     targetRect = child.GetComponent<RectTransform>();
//                     adjustedY = 0f;
//                     break;
//                 }
//             }
//
//             if (targetRect == null)
//             {
//                 Debug.LogWarning("StartPoint が見つかりませんでした");
//                 return;
//             }
//         }
//
//         // ロボットプレハブを生成
//         var roboPrefab = await Utils.InstantiatePrefab("Prefabs/Robo/RoboPrefab", chapterButtonParent);
//         var roboRect = roboPrefab.GetComponent<RectTransform>();
//
//         // アンカー・ピボット設定
//         roboRect.anchorMin = roboRect.anchorMax = new Vector2(0.5f, 1f);
//         roboRect.pivot = new Vector2(0.5f, 0.5f); // 下部中央を基準点に
//
//         // ロボットの位置を targetRect の下に配置
//         roboRect.anchoredPosition = new Vector2(
//             targetRect.anchoredPosition.x,
//             targetRect.anchoredPosition.y - adjustedY
//         );
//
//         // ロボットのサイズを調整（必要に応じて）
//         roboRect.localScale = new Vector3(0.5f, 0.5f, 1f);
//
//         // ユーザーの選択したロボットデータを設定
//         var userDataManager = UserDataManager.GetInstance();
//         var userData = userDataManager.GetUserData();
//         var selectedRoboId = userData.selectedRoboId ?? "default";
//
//         var roboCustomDataDict = userDataManager.GetRoboCustomData(selectedRoboId);
//         if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(selectedRoboId))
//         {
//             var roboPrefabComponent = roboPrefab.GetComponent<RoboPrefab>();
//             if (roboPrefabComponent != null)
//             {
//                 roboPrefabComponent.SetRobo(roboCustomDataDict[selectedRoboId]);
//             }
//         }
//     }
//
//     
//     public void OnClickCloseButton()
//     {
//         Destroy(this.gameObject);
//     }
// }
