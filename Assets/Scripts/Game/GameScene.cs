using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Genit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    
//     private AudioClip _resultSound;

     private async void Start()
     {
         Debug.Log("GameScene Start");
         
         _quizzes = QuizGenerator.GenerateRandomQuizList();
         
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
         SetEnemies();
         await SetRobo();
         await StartNextQuiz();
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
         var quiz = quizObj.GetComponent<Quiz>();
         // quiz.Setup(_quizzes[_quizIndex], _quizIndex,
         //     async (isCorrect, answerWord) => HandleQuizAnswer(isCorrect, answerWord, quiz));
         quiz.Setup(_quizzes[_quizIndex], _quizIndex,
             async (isCorrect, answerWord) =>
             {
                 quiz.DestroyGameUI();
                 Debug.Log($"Answered: {answerWord}, Correct: {isCorrect}");

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

     private void SetEnemies()
     {
         // 敵データファイルを読み込む
         var enemyDataText = Resources.Load<TextAsset>("Data/enemy_data");
         if (enemyDataText == null)
         {
             Debug.LogError("enemy_data.txt not found");
             return;
         }
         
         // 改行で分割して敵IDのリストを取得
         string[] enemyPatterns = enemyDataText.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
         
         // チャプター番号から対応する敵パターンを取得（1始まりなので-1）
         int chapterIndex = Const.GameSceneParam.ChapterNumber - 1;
         if (chapterIndex < 0 || chapterIndex >= enemyPatterns.Length)
         {
             Debug.LogError($"Chapter {Const.GameSceneParam.ChapterNumber} not found in enemy_data");
             return;
         }
         
         string enemyPattern = enemyPatterns[chapterIndex].Trim();
         
         // 既存の敵画像をクリア
         foreach (Transform child in enemyArea)
         {
             Destroy(child.gameObject);
         }
         
         // パターンの各文字が敵を表す（最大4体）
         int enemyCount = Mathf.Min(enemyPattern.Length, 4);
         
         for (int i = 0; i < enemyCount; i++)
         {
             char enemyChar = enemyPattern[i];
             string enemyId = enemyChar.ToString();
             
             // 敵画像を作成
             GameObject enemyImageObj = new GameObject($"Enemy_{enemyId}_{i}");
             enemyImageObj.transform.SetParent(enemyArea, false);
             
             // Imageコンポーネントを追加
             Image enemyImage = enemyImageObj.AddComponent<Image>();
             
             // スプライトを読み込む
             Sprite enemySprite = Resources.Load<Sprite>($"Images/Enemy/{enemyId}");
             if (enemySprite != null)
             {
                 enemyImage.sprite = enemySprite;
                 enemyImage.preserveAspect = true;
             }
             else
             {
                 Debug.LogWarning($"Enemy sprite not found: Images/Enemy/{enemyId}");
             }
             
             // RectTransformの設定
             RectTransform rectTransform = enemyImageObj.GetComponent<RectTransform>();
             rectTransform.sizeDelta = new Vector2(100, 100); // サイズは適宜調整
         }
         
         Debug.Log($"Chapter {Const.GameSceneParam.ChapterNumber}: Spawned {enemyCount} enemies from pattern '{enemyPattern}'");
     }
     
     private async void EndGame()
     {
         foreach (var result in _quizResults)
         {
             if (result.IsCorrect)
                 _correctCount++;
         }
         var isSaved = false;
         Vector4 blockerColor = new Color(255f / 255f, 246f / 255f, 230f / 255f, 1.0f);
         var resultDialogObj = await Utils.OpenDialog("Prefabs/Game/ResultDialog", transform, blockerColor);
         var resultDialog = resultDialogObj.GetComponent<ResultDialog>();
         resultDialog.Setup(_quizResults , () =>
             {
                 if (isSaved) AdManager.Instance.ShowInterstitialAd(() => EndScene());
             });
         // _audioSource.PlayOneShot(_resultSound);
         //
         // SaveAnswerData();
         // SaveUserData();

         // チャプター進捗を保存
         await SaveChapterProgress();
         
         // プレイヤーステータスを保存
         await SavePlayerStatus();

         isSaved = true;
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

         // 教科名とともに保存
         await UserDataManager.GetInstance().SaveChapterProgress(Const.GameSceneParam.Subject, progressData);
     }

     private async UniTask SavePlayerStatus()
     {
         // 現在のPlayerStatusを取得
         var userDataManager = UserDataManager.GetInstance();
         var playerStatus = userDataManager.GetPlayerStatus();
         
         // LevelingSystemを使用してEXPを計算
         int expFromCorrectAnswers = LevelingSystem.CalculateExpFromCorrectAnswers(_correctCount);
         
         // 現在のEXPに加算
         playerStatus.exp += expFromCorrectAnswers;
         
         playerStatus.level = LevelingSystem.CalculateLevelFromExp(playerStatus.exp);
         
         // HPを10加算（仮の値）
         playerStatus.hp = Mathf.Min(playerStatus.hp + LevelingSystem.RecoveryHpPerQuest, 100);
         
         // PlayerStatusを保存
         await userDataManager.UpdatePlayerStatus(playerStatus);
         
         Debug.Log($"Player Status Updated - EXP: {playerStatus.exp} (+{expFromCorrectAnswers}), HP: {playerStatus.hp} (+10)");
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
         }
         else
         {
             Debug.LogWarning($"RoboCustomData not found for selectedRoboId: {selectedRoboId}");
         }
     }
     
     public async void OnClickPauseButton()
     {
         var pauseDialogObject = await Utils.OpenDialog("Prefabs/Game/PauseDialog", transform);
         var pauseDialog = pauseDialogObject.GetComponent<PauseDialog>();
         pauseDialog.Setup(
             () =>
             {
                 //クローズ後の処理が必要になったらここに書く
             });
     }
}