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

     private int _quizIndex;
     private List<QuizResultData> _quizResults;
     private QuizData[] _quizzes;
//     private AudioClip _resultSound;

     private async void Start()
     {
         Debug.Log("GameScene Start");
         _quizzes = new QuizSelectManager().PrepareQuizzesByChapter(
             Const.GameSceneParam.Subject,
             Const.GameSceneParam.DifficultyLevel,
             Const.GameSceneParam.ChapterNumber
         );
         _quizResults = new List<QuizResultData>();
         
         // _correctSound = Resources.Load<AudioClip>("SE/correct");
         // _incorrectSound = Resources.Load<AudioClip>("SE/incorrect");
         // _addMedalSound = Resources.Load<AudioClip>("SE/addMedal");
         // _nextQuizSound = Resources.Load<AudioClip>("SE/nextQuiz");
         // _resultSound = Resources.Load<AudioClip>("SE/result");
         //
         // await Resources.LoadAsync("Prefabs/Game/JudgeScreen");
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

         
         quizIndexSlider.value = (_quizIndex + 1f) / _quizzes.Length;
         quizIndexText.text = $"{_quizIndex + 1} / {_quizzes.Length}";
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

                 // クイズ結果を記録（← ここ追加）
                 _quizResults.Add(new QuizResultData
                 {
                     Quiz = _quizzes[_quizIndex],
                     IsCorrect = isCorrect,
                     AnswerWord = answerWord
                 });

                 _quizIndex++;

                 if (_quizIndex < _quizzes.Length)
                     await StartNextQuiz();
                 else
                     EndGame(); // ← 最後のクイズならResultDialogへ
             });
     }

     
     private async void EndGame()
     {
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

         isSaved = true;
         Debug.Log("AnswerData saved");
     }

     private async UniTask SaveChapterProgress()
     {
         // 正解数を計算
         int correctCount = 0;
         foreach (var result in _quizResults)
         {
             if (result.IsCorrect)
                 correctCount++;
         }

         // ChapterProgressDataを作成
         var progressData = new UserDataManager.ChapterProgressData
         {
             chapterId = Const.GameSceneParam.ChapterNumber.ToString(),
             dateTime = Utils.DateTimeToUnixTime(Clock.GetInstance().Now()),
             correctCount = correctCount,
             totalCount = _quizResults.Count
         };

         // 教科名とともに保存
         await UserDataManager.GetInstance().SaveChapterProgress(Const.GameSceneParam.Subject, progressData);
         Debug.Log($"Chapter progress saved - Subject: {Const.GameSceneParam.Subject}, Chapter: {Const.GameSceneParam.ChapterNumber}, Correct: {correctCount}/{_quizResults.Count}");
     }

     private void EndScene()
     {
         var scene = SceneManager.GetSceneByName("SelectScene");
         foreach (var go in scene.GetRootGameObjects())
             if (go.name == "Canvas")
                 go.SetActive(true);

         SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
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