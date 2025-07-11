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
     [SerializeField] private Slider quizIndexSlider;
     [SerializeField] private Text quizIndexText;
//     private AudioClip _addMedalSound;

//     private AudioClip _correctSound;
//     private AudioClip _incorrectSound;
//     private int _medalNum;
//     private int _medalNumWhenStart;
//     private AudioClip _nextQuizSound;
//
     private Const.PlayMode _playMode;
     private int _quizIndex;
//     private List<QuizResultData> _quizResults;
     private QuizData[] _quizzes;
//     private AudioClip _resultSound;
//
     private async void Start()
     {
         Debug.Log("GameScene Start");
         _quizzes = new QuizSelectManager().PrepareQuizzesByChapter(
             Const.GameSceneParam.Subject,
             Const.GameSceneParam.DifficultyLevel,
             Const.GameSceneParam.ChapterNumber
         );
         // _quizResults = new List<QuizResultData>();
         // _medalNum = UserDataManager.GetInstance().GetUserData().totalMedal;
         // _medalNumWhenStart = _medalNum;
         // _playMode = QuizSelectManager.GetInstance().GetPlayMode();
         //
         // _correctSound = Resources.Load<AudioClip>("SE/correct");
         // _incorrectSound = Resources.Load<AudioClip>("SE/incorrect");
         // _addMedalSound = Resources.Load<AudioClip>("SE/addMedal");
         // _nextQuizSound = Resources.Load<AudioClip>("SE/nextQuiz");
         // _resultSound = Resources.Load<AudioClip>("SE/result");
         //
         // await Resources.LoadAsync("Prefabs/Game/JudgeScreen");
         await StartNextQuiz();
     }
//
     public async UniTask StartNextQuiz()
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

         if (_playMode != Const.PlayMode.Calculation)
         {
             quizIndexSlider.value = (_quizIndex + 1f) / _quizzes.Length;
             quizIndexText.text = $"{_quizIndex + 1} / {_quizzes.Length}";
         }
         else if (_playMode == Const.PlayMode.Calculation)
         {
             quizIndexText.text = $"だい {_quizIndex + 1} もんめ";

             if (_quizIndex + 1 >= 100)
                 quizIndexSlider.value = 1f;
             else if (_quizIndex + 1 >= 50)
                 quizIndexSlider.value = (_quizIndex + 1f - 50f) / 50f;
             else if (_quizIndex + 1 >= 30)
                 quizIndexSlider.value = (_quizIndex + 1f - 30f) / 20f;
             else if (_quizIndex + 1 >= 10)
                 quizIndexSlider.value = (_quizIndex + 1f - 10f) / 20f;
             else
                 quizIndexSlider.value = (_quizIndex + 1f) / 10f;
         }

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
                 _quizIndex++;
                 if (_quizIndex < _quizzes.Length)
                     await StartNextQuiz();
                 else
                     Debug.Log("All quizzes done!");
             });
         
     }
//
//     private async void HandleQuizAnswer(bool isCorrect, string answerWord, Quiz quiz)
//     {
//         quiz.DestroyGameUI();
//         StopCoroutine(_countdownCoroutine);
//
//         // if (UserDataManager.GetInstance().GetUserData().isPlayedTutorial == false && _quizIndex == 1)
//         // {
//         //     var tutorialPanelObj =
//         //         await Utils.InstantiatePrefab("Prefabs/Common/TutorialPanel", gameObject.transform);
//         //     var tutorialManager = tutorialPanelObj.GetComponent<TutorialManager>();
//         //     tutorialManager.SetDescriptionAfterAnswer(_betAndMedalPanel.transform,
//         //         _betAndMedalPanel.DisableBetButton,
//         //         _betAndMedalPanel.EnableBetButton);
//         //     await UserDataManager.GetInstance().SetUserData(new Dictionary<string, object>
//         //     {
//         //         { UserDataManager.USER_DATA_KEY_IS_PLAYED_TUTORIAL, true }
//         //     });
//         // }
//
//         if (_playMode == Const.PlayMode.Normal)
//         {
//             if (isCorrect)
//             {
//                 // _audioSource.PlayOneShot(_correctSound);
//                 // StartCoroutine(AnimateNumber(_medalNum - _betNum, _medalNum + _betNum));
//                 StartCoroutine(AnimateNumber(_medalNum, _medalNum + _betNum));
//                 _medalNum += _betNum;
//             }
//             else
//             {
//                 _audioSource.PlayOneShot(_incorrectSound);
//                 // _medalNum -= _betNum;
//             }
//         }
//
//         if (_playMode == Const.PlayMode.Calculation)
//             if (isCorrect)
//             {
//                 // _audioSource.PlayOneShot(_correctSound);
//                 StartCoroutine(AnimateNumber(_medalNum, _medalNum + _betNum));
//                 _medalNum += _betNum;
//             }
//
//         _betNum = 0;
//         _betAndMedalPanel.SetBetNum(_betNum);
//
//         _quizResults.Add(new QuizResultData
//         {
//             Quiz = _quizzes[_quizIndex],
//             IsCorrect = isCorrect,
//             AnswerWord = answerWord
//         });
//
//         if (_playMode == Const.PlayMode.Normal)
//         {
//             var judgeScreenObj = await Utils.InstantiatePrefab("Prefabs/Game/JudgeScreen", _contentsTransform);
//             var judgeScreen = judgeScreenObj.GetComponent<JudgeScreen>();
//             judgeScreen.Setup(isCorrect, _betNum, _quizzes[_quizIndex], async () =>
//             {
//                 _quizIndex++;
//                 if (_quizIndex < _quizzes.Length && _medalNum > 0)
//                     await StartNextQuiz();
//                 else
//                     EndGame();
//             });
//         }
//         else if (_playMode == Const.PlayMode.Calculation)
//         {
//             _quizIndex++;
//             if (isCorrect && _quizIndex < _quizzes.Length && _medalNum > 0)
//                 await StartNextQuiz();
//             else
//                 EndGame();
//         }
//     }
//
//     private async void SaveAnswerData()
//     {
//         var onePlayId = Utils.GenerateRandomString(32);
//         var dateTimeNow = Utils.DateTimeToUnixTime(Clock.GetInstance().Now());
//         foreach (var quizResult in _quizResults)
//         {
//             var data = new UserDataManager.AnswerData();
//             data.onePlayId = onePlayId;
//             data.playMode = _playMode;
//             data.quizId = quizResult.Quiz.id;
//             data.dateTime = dateTimeNow;
//             data.isCorrect = quizResult.IsCorrect;
//             data.medalNum = _medalNum;
//             data.answerWord = quizResult.AnswerWord;
//             await UserDataManager.GetInstance().AddUserAnswerData(data);
//         }
//     }
//
//     private async void SaveUserData()
//     {
//         //データを取得
//         var userData = UserDataManager.GetInstance().GetUserData();
//         var rating = userData.rating;
//         var stage = userData.stage;
//
//         var correctCount = 0;
//
//         foreach (var quizResult in _quizResults)
//             if (quizResult.IsCorrect)
//                 correctCount++;
//
//         var correctRate = (float)correctCount / _quizResults.Count;
//         if (correctRate >= 0.9)
//             rating += 10;
//         else if (correctRate >= 0.7)
//             rating += 3;
//         else if (correctRate >= 0.5)
//             rating += 0;
//         else if (correctRate >= 0.3)
//             rating -= 5;
//         else
//             rating -= 10;
//
//         if (rating >= 30 && stage < 7)
//         {
//             stage++;
//             rating = 0;
//         }
//         else if (rating <= -15 && stage > 1)
//         {
//             stage--;
//             rating = 0;
//         }
//
//         if (rating > 30)
//             rating = 30;
//         else if (rating < -15) rating = -15;
//
//         var data = new Dictionary<string, object>
//         {
//             { UserDataManager.USER_DATA_KEY_TOTAL_MEDAL, _medalNum },
//             { UserDataManager.USER_DATA_KEY_RATING, rating },
//             { UserDataManager.USER_DATA_KEY_STAGE, stage }
//         };
//
//         await UserDataManager.GetInstance().SetUserData(data);
//     }
//
//     private async void EndGame()
//     {
//         var isSaved = false;
//         Vector4 blockerColor = new Color(255f / 255f, 246f / 255f, 230f / 255f, 1.0f);
//         var resultDialogObj = await Utils.OpenDialog("Prefabs/Game/ResultDialog", transform, blockerColor);
//         var resultDialog = resultDialogObj.GetComponent<ResultDialog>();
//         resultDialog.Setup(_quizResults, _medalNum, _medalNum - _medalNumWhenStart
//             , () =>
//             {
//                 if (isSaved) AdManager.Instance.ShowInterstitialAd(() => EndScene());
//             });
//         _audioSource.PlayOneShot(_resultSound);
//
//         SaveAnswerData();
//         SaveUserData();
//
//         isSaved = true;
//         Debug.Log("AnswerData saved");
//     }
//
//     private void EndScene()
//     {
//         var scene = SceneManager.GetSceneByName("SelectScene");
//         foreach (var go in scene.GetRootGameObjects())
//             if (go.name == "Canvas")
//                 go.SetActive(true);
//
//         SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
//     }
//
//     private IEnumerator AnimateNumber(int startValue, int targetValue)
//     {
//         float timer = 0;
//         var animationDuration = 0.5f;
//         var previousMedalValue = startValue; // 前のフレームのメダルの値を追跡する
//
//         while (timer < animationDuration)
//         {
//             // 現在の経過時間に基づいて、表示する数値を計算
//             var progress = timer / animationDuration;
//             var currentMedalValue = (int)Mathf.Lerp(startValue, targetValue, progress);
//
//             // 数値が前のフレームから変更された場合、音を再生
//             if (currentMedalValue != previousMedalValue) _audioSource.PlayOneShot(_addMedalSound);
//
//             // 数値をテキストとして設定
//             _betAndMedalPanel.SetMedalNum(currentMedalValue);
//
//             // 次のフレームまで待機
//             yield return null;
//             timer += Time.deltaTime;
//
//             // 前のフレームの値を更新
//             previousMedalValue = currentMedalValue;
//         }
//
//         // 最終的な目標値を設定し、最後にもう一度音を再生
//         _betAndMedalPanel.SetMedalNum(targetValue);
//         _audioSource.PlayOneShot(_addMedalSound);
//     }
//
//     private IEnumerator CountdownCoroutine(Quiz quiz)
//     {
//         var count = 20;
//
//         if (QuizSelectManager.GetInstance().GetPlayMode() != Const.PlayMode.Calculation)
//         {
//             count = 15;
//         }
//         else if (QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Calculation)
//         {
//             if (_quizIndex + 1 >= 100)
//             {
//                 count = 1;
//                 _betNumInCalculationMode = 5;
//             }
//             else if (_quizIndex + 1 >= 50)
//             {
//                 count = 3;
//                 _betNumInCalculationMode = 4;
//             }
//             else if (_quizIndex + 1 >= 30)
//             {
//                 count = 5;
//                 _betNumInCalculationMode = 3;
//             }
//             else if (_quizIndex + 1 >= 10)
//             {
//                 count = 7;
//                 _betNumInCalculationMode = 2;
//             }
//             else
//             {
//                 count = 10;
//                 _betNumInCalculationMode = 1;
//             }
//
//             _betAndMedalPanel.SetBetNum(_betNumInCalculationMode);
//         }
//
//         while (count > 0)
//         {
//             _betAndMedalPanel.SetCount(count); // UIにカウントダウンを表示
//             yield return new WaitForSeconds(1); // 1秒待機
//             count--; // カウントダウン
//         }
//
//         _betAndMedalPanel.SetCount(0); // カウントダウン終了
//
//         if (_playMode != Const.PlayMode.Calculation)
//             HandleQuizAnswer(false, "", quiz);
//         else if (_playMode == Const.PlayMode.Calculation) EndGame();
//     }
//
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