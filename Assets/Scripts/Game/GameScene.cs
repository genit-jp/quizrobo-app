using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Linq;

public class GameScene : MonoBehaviour
{
//     [SerializeField] private AudioSource _audioSource;
     [SerializeField] private Transform contentsTransform;
     [SerializeField] private GameObject uiPanel;
     [SerializeField] private Transform answerProgressPanel;
     [SerializeField] private GameObject answerIconPrefab, robo;
     [SerializeField] private Sprite unansweredSprite, correctSprite, incorrectSprite;
     [SerializeField] private Transform enemyArea;
//     private AudioClip _addMedalSound;
//     private AudioClip _correctSound;
//     private AudioClip _incorrectSound;
//     private int _medalNum;
//     private int _medalNumWhenStart;
//     private AudioClip _nextQuizSound;

     private int _quizIndex;
     private List<QuizResultData> _quizResults;
     private QuizData[] _quizzes;
     private int _correctCount;
     private List<Image> answerIcons = new List<Image>();
     private EnemyManager _enemyManager;
     private bool _allEnemiesDefeated;
     
     private bool _gameEnded = false;
     private int _totalAttackPower = 10;
     private bool _isGotNewItem;
     private Quiz _quiz;
    
     
     private UserDataManager _userData;
     
     // デフォルト攻撃力
    
//     private AudioClip _resultSound;

     private async void Start()
     {
         _userData = UserDataManager.GetInstance();
         _userData.AddRoboCustomDataUpdateListener(OnRoboCustomDataUpdated);
         Debug.Log("GameScene Start");
         
         _quizzes = QuizGenerator.GenerateRandomQuizList(Const.GameSceneParam.ChapterNumber);
         _quizResults = new List<QuizResultData>();
         
         for (int i = 0; i < _quizzes.Length; i++)
         {
             var iconObj = Instantiate(answerIconPrefab, answerProgressPanel);
             var image = iconObj.GetComponent<Image>();
             image.sprite = unansweredSprite;
             answerIcons.Add(image);
         }
         
         // _correctSound = Resources.Load<AudioClip>("SE/correct");
         // _incorrectSound = Resources.Load<AudioClip>("SE/incorrect");
         // _addMedalSound = Resources.Load<AudioClip>("SE/addMedal");
         // _nextQuizSound = Resources.Load<AudioClip>("SE/nextQuiz");
         // _resultSound = Resources.Load<AudioClip>("SE/result");
         //
         await Resources.LoadAsync("Prefabs/Game/JudgeScreen");
         
         // EnemyManagerを使用して敵を表示
         _enemyManager = gameObject.AddComponent<EnemyManager>();
         _enemyManager.SetupEnemies(Const.GameSceneParam.ChapterNumber, enemyArea);
         
         await SetRobo();
         await StartNextQuiz();
     }

     private void OnDisable()
     {
         _userData.RemoveRoboCustomDataUpdateListener(OnRoboCustomDataUpdated);
     }
    
     private async void OnRoboCustomDataUpdated()
     {
         await SetRobo();
     }

     private async UniTask StartNextQuiz()
     {
         // if (UserDataManager.GetInstance().GetUserData().isPlayedTutorial == false && _quizIndex == 1)
         // {
         //     var tutorialPanelObj = await Utils.InstantiatePrefab("Prefabs/Common/TutorialPanel", gameObject.transform);
         //     var tutorialManager = tutorialPanelObj.GetComponent<TutorialManager>();
         //    
         // }

         foreach (Transform child in contentsTransform)
             if (child != uiPanel.transform)
                 Destroy(child.gameObject);
         
         // _audioSource.PlayOneShot(_nextQuizSound);
         
         var quizObj = await Utils.InstantiatePrefab("Prefabs/Game/Quiz", contentsTransform);
         _quiz = quizObj.GetComponent<Quiz>();
         _quiz.Setup(_quizzes[_quizIndex], _quizIndex,
             async (isCorrect, answerWord) =>
             {
                 _quiz.DestroyGameUI();
                 Debug.Log($"Answered: {answerWord}, Correct: {isCorrect}");

                 if (isCorrect)
                 {
                     //攻撃力の計算
                     _enemyManager.AttackNextEnemy(_totalAttackPower);
                     
                     if (_enemyManager.AllEnemiesDefeated && !_gameEnded)
                     {
                         _gameEnded = true;
                         EndGame();
                         return;
                     }
                 }

                 // クイズ結果を記録
                 _quizResults.Add(new QuizResultData
                 {
                     Quiz = _quizzes[_quizIndex],
                     IsCorrect = isCorrect,
                     AnswerWord = answerWord
                 });

                 answerIcons[_quizIndex].sprite = isCorrect ? correctSprite : incorrectSprite;
                 
                 // JudgeScreenを表示
                 var judgeScreenObj = await Utils.InstantiatePrefab("Prefabs/Game/JudgeScreen", transform);
                 var judgeScreen = judgeScreenObj.GetComponent<JudgeScreen>();
                 
                 // JudgeScreenをセットアップ
                 judgeScreen.Setup(isCorrect, _quizzes[_quizIndex], () =>
                 {
                     // JudgeScreenを破棄
                     Destroy(judgeScreenObj);
                     
                     // 次のクイズへ進む
                     _quizIndex++;
                     
                     if (_quizIndex < _quizzes.Length)
                         StartNextQuiz();
                     else
                         EndGame(); // 最後のクイズならResultDialogへ
                 });
             });
     }
     
     
     private async void EndGame()
     {
         _allEnemiesDefeated = _enemyManager.AllEnemiesDefeated;
         foreach (var result in _quizResults)
         {
             if (result.IsCorrect)
                 _correctCount++;
         }
         var isSaved = false;
         
         if (_allEnemiesDefeated)
         {
             // チャプター進捗を保存
             await SaveChapterProgress();
         
             // プレイヤーステータスを保存
             await SavePlayerStatus();

             isSaved = true;
             // 次のステージを解放
             await UnlockNextStage();
             
             Vector4 blockerColor = new Color(255f / 255f, 246f / 255f, 230f / 255f, 1.0f);
             var resultDialogObj = await Utils.OpenDialog("Prefabs/Game/ResultDialog", transform, blockerColor);
             var resultDialog = resultDialogObj.GetComponent<ResultDialog>();
             resultDialog.Setup(
                 _quizResults,
                 () =>
                 {
                     if (isSaved)
                     {
                         AdManager.Instance.ShowInterstitialAd(() => LoadNextChapter());
                     }
                 },
                 () =>
                 {
                     if (isSaved)
                     {
                         AdManager.Instance.ShowInterstitialAd(() => EndScene());
                     }
                 },
                 _isGotNewItem);
         }
         else
         {
             // ❗敵が逃げたダイアログ
             var dialogObj = await Utils.OpenDialog("Prefabs/Common/CommonDialog", transform);
             var commonDialog = dialogObj.GetComponent<CommonDialog>();
             commonDialog.Setup("敵が逃げてしまった", "EXPをためると新しいパーツをGET!\nロボをカスタマイズして再挑戦しよう！", (result) =>
             {
                 EndScene();
             }, CommonDialog.Mode.OK);
         }

         // _audioSource.PlayOneShot(_resultSound);
         //
         // SaveAnswerData();
         // SaveUserData();
         
         Debug.Log("AnswerData saved");
     }

     private async UniTask SaveChapterProgress()
     {
         // ChapterProgressDataを作成
         var progressData = new UserDataManager.ChapterProgressData
         {
             chapterId = Const.GameSceneParam.ChapterNumber.ToString(),
             dateTime = Utils.DateTimeToUnixTime(Clock.GetInstance().Now()),
             correctCount = _correctCount,
             totalCount = _quizResults.Count
         };

         //　進捗データを保存
         await UserDataManager.GetInstance().SaveChapterProgress(progressData);
     }

     private async UniTask SavePlayerStatus()
     {
         // 現在のPlayerStatusを取得
         var playerStatus = _userData.GetPlayerStatus();
         
         // ゲームクリア時のみEXPを加算
         if (_allEnemiesDefeated)
         {
             // 敵のHP合計値をEXPとして加算
             int totalEnemyHp = _enemyManager.GetTotalEnemyHp();
             playerStatus.exp += totalEnemyHp;
             
             await _userData.UpdatePlayerStatus(playerStatus);
             
             //EXPが次の獲得EXPまでに達したときアイテムを獲得ただし受取はfalse
             var ownedRoboId = _userData.OwnedRoboPartsIds();
             var roboData = MasterData.GetInstance().GetNextUnownedRoboByExp(ownedRoboId);
             if (roboData != null && playerStatus.exp >= roboData.exp_required)
             {
                 // ロボットを獲得
                 await _userData.AddOwnedRoboPart(roboData.id, false);
                    _isGotNewItem = true;
                 Debug.Log($"New Robo Part Unlocked: {roboData.id}");
             }
             else
             {
                 Debug.Log("No new Robo Part unlocked");
             }
         }
         else
         {
             Debug.Log("Game Over - No EXP gained");
         }
         
         
     }
     
     private async void LoadNextChapter()
     {
         var loadingPanelObj = await Utils.InstantiatePrefab("Prefabs/Common/LoadingPanel", transform);
         await UniTask.Delay(200);
         Const.GameSceneParam.ChapterNumber += 1; // チャプター番号更新
         var scene = SceneManager.GetSceneByName("GameScene");
         if (scene.isLoaded)
             await SceneManager.UnloadSceneAsync(scene);

         await SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
         
         Destroy(loadingPanelObj);
     }

     private void EndScene()
     {
         var scene = SceneManager.GetSceneByName("SelectScene");
         foreach (var go in scene.GetRootGameObjects())
             if (go.name == "Canvas")
                 go.SetActive(true);

         SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
     }
     
     
     private async UniTask SetRobo()
     {
         // ユーザーの選択したロボットデータを取得
         var userDataManager = UserDataManager.GetInstance();
         var userData = userDataManager.GetUserData();
         var selectedRoboId = userData.selectedRoboId ?? "default";
         
         var roboCustomDataDict = userDataManager.GetRoboCustomData(selectedRoboId);
         if (roboCustomDataDict != null && roboCustomDataDict.ContainsKey(selectedRoboId))
         {
             var roboCustomData = roboCustomDataDict[selectedRoboId];
             
             // RoboSettingManagerを使用してロボットを表示
             await RoboSettingManager.DisplayRobo(robo, roboCustomData);
             
             // 攻撃力を計算
             _totalAttackPower = CalculateTotalAttackPower(roboCustomData);
             Debug.Log($"Total Attack Power: {_totalAttackPower}");
         }
         else
         {
             Debug.LogWarning($"RoboCustomData not found for selectedRoboId: {selectedRoboId}");
         }
     }
     
     private int CalculateTotalAttackPower(UserDataManager.RoboCustomData roboCustomData)
     {
         var masterData = MasterData.GetInstance();
         int totalAtk = 0;
         
         // 各パーツIDからRoboDataを取得してatkを合計
         var headData = masterData.robos.FirstOrDefault(r => r.id == roboCustomData.headId);
         if (headData != null) totalAtk += headData.atk;
         
         var bodyData = masterData.robos.FirstOrDefault(r => r.id == roboCustomData.bodyId);
         if (bodyData != null) totalAtk += bodyData.atk;
         
         var armsData = masterData.robos.FirstOrDefault(r => r.id == roboCustomData.armsId);
         if (armsData != null) totalAtk += armsData.atk;
         
         var legsData = masterData.robos.FirstOrDefault(r => r.id == roboCustomData.legsId);
         if (legsData != null) totalAtk += legsData.atk;
         
         var tailData = masterData.robos.FirstOrDefault(r => r.id == roboCustomData.tailId);
         if (tailData != null) totalAtk += tailData.atk;
         
         Debug.Log(totalAtk);
         // 最小攻撃力を1に保証
         return Mathf.Max(totalAtk, 1);
     }
     
     private async UniTask UnlockNextStage()
     {
         int currentChallengeLevel = _userData.GetChallengeLevel();
         int nextStageNumber = Const.GameSceneParam.ChapterNumber + 1;
         
         // 現在の保存値より大きい場合のみ更新
         if (nextStageNumber > currentChallengeLevel)
         {
             await _userData.SetChallengeLevel(nextStageNumber);
             Debug.Log($"Next stage unlocked: {nextStageNumber}");
         }
         else
         {
             Debug.Log($"Stage {nextStageNumber} is already unlocked. Current max: {currentChallengeLevel}");
         }
     }
     
     public async void OnClickPauseButton()
     {
         _quiz.StopTimer();
         var pauseDialogObject = await Utils.OpenDialog("Prefabs/Game/PauseDialog", transform);
         var pauseDialog = pauseDialogObject.GetComponent<PauseDialog>();
         pauseDialog.Setup(
             () =>
             {
                 _quiz.ResumeTimer();
             });
     }
}