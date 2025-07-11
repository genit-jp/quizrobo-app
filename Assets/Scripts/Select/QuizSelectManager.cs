using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public sealed class QuizSelectManager
{
    public QuizData[] PrepareQuizzesByChapter(string subject, string difficultyLevel, int chapterNumber)
    {
        var masterData = MasterData.GetInstance();
        
        // 指定された条件に合致するチャプターを検索
        var targetChapter = masterData.chapters.FirstOrDefault(chapter => 
            chapter.subject == subject && 
            chapter.difficultyLevel == difficultyLevel && 
            chapter.chapterNumber == chapterNumber);

        if (targetChapter == null || targetChapter.quizIds == null || targetChapter.quizIds.Length == 0)
        {
            Debug.LogWarning($"該当するチャプターが見つかりません: Subject={subject}, Level={difficultyLevel}, Chapter={chapterNumber}");
            return new QuizData[0];
        }

        // チャプターに含まれるクイズIDに対応するQuizDataを抽出
        var quizList = new List<QuizData>();
        foreach (var quizId in targetChapter.quizIds)
        {
            var quiz = masterData.quizzes.FirstOrDefault(q => q.Id == quizId);
            if (quiz != null && quiz.available)
            {
                quizList.Add(quiz);
            }
        }

        // クイズをランダムに並び替え
        var random = new System.Random();
        var randomizedQuizzes = quizList.OrderBy(x => random.Next()).ToArray();

        return randomizedQuizzes;
    }

    // private static QuizSelectManager _instance;
    // public List<QuizData> selectQuizzes = new List<QuizData>();
    // private Const.PlayMode _playMode;
    //
    // public static QuizSelectManager GetInstance()
    // {
    //     if (_instance == null)
    //     {
    //         _instance = new QuizSelectManager();
    //     }
    //
    //     return _instance;
    // }
    //
    // public void SetSelectQuizzes(int grade, int quizNum, Const.PlayMode mode, string subject = "")
    // {
    //     selectQuizzes.Clear();
    //     
    //     if (mode == Const.PlayMode.Normal)
    //     {
    //         // var answerDataList = UserDataManager.GetInstance().GetAnswerDataList();
    //         // var distinctOnePlayIds = answerDataList.Select(answer => answer.onePlayId).Distinct().ToList();
    //         // if (distinctOnePlayIds.Count <= 10)
    //         // {
    //         //     SelectByGoodQuizAlgorithm(grade, quizNum);
    //         //     Debug.Log("goodQuizAlgorithm");
    //         //     if (selectQuizzes.Count < quizNum)
    //         //     {
    //         //         SelectByRandomAlgorithm(grade, quizNum - selectQuizzes.Count);
    //         //     }
    //         // }
    //         // else
    //         // {
    //         //     SelectByRandomAlgorithm(grade, quizNum);
    //         //     Debug.Log("randomAlgorithm");
    //         // }
    //         SelectByGoodQuizAlgorithm(grade, quizNum);
    //         if (selectQuizzes.Count < quizNum)
    //         {
    //             SelectByRandomAlgorithm(grade, quizNum - selectQuizzes.Count);
    //         }
    //         
    //         _playMode = mode;
    //     }
    //     else if (mode == Const.PlayMode.Calculation)
    //     {
    //         SelectByCalculationMode(grade);
    //         _playMode = mode;
    //     }
    //     
    //     //教科選択(仮)
    //         // if (subject != "")
    //         // {
    //         //     quizzes = MasterData.GetInstance().quizzes.ToList();
    //         //     quizzes = quizzes.Where(quiz => quiz.targetGrade == grade).ToList();
    //         //     quizzes = quizzes.Where(quiz => quiz.subject == subject).ToList();
    //         //     quizzes = ShuffleQuizzes(quizzes);
    //         //     selectQuizzes = quizzes.Take(quizNum).ToList();
    //         // }
    // }
    //
    // private void SelectByGoodQuizAlgorithm(int grade, int quizNum)
    // {
    //     var filteredMasterQuiz = RemovePlayedQuizzes(100);
    //     var goodQuizzes = filteredMasterQuiz
    //         .Where(quiz => quiz.isGood || quiz.quizTags.Contains("読みがな") || quiz.quizTags.Contains("計算"))
    //         .ToList();
    //     goodQuizzes = goodQuizzes.Where(quiz => quiz.targetGrade <= grade).ToList();
    //     goodQuizzes = ShuffleQuizzes(goodQuizzes);
    //
    //     string masterVersion = FirebaseManager.Instance.GetRemoteConfigValue(Const.MasterVersion);
    //     var subjectRatios = new Dictionary<string, int>();
    //     if (masterVersion == "0.6.0")
    //     {
    //         subjectRatios = Const.SubjectRatioInGoodQuizFor060[grade];
    //     }
    //     else
    //     {
    //         subjectRatios = Const.SubjectRatioInGoodQuizFor120[grade];
    //     }
    //     
    //     foreach (var subjectRatio in subjectRatios)
    //     {
    //         string subject = subjectRatio.Key;
    //         int subjectQuizNum = (int)Math.Round(quizNum * (subjectRatio.Value / 100.0), MidpointRounding.AwayFromZero);
    //         var subjectGoodQuizzes = new List<QuizData>();
    //         
    //         if (subject != "その他")
    //         {
    //             subjectGoodQuizzes = goodQuizzes.Where(quiz => quiz.subject == subject).ToList();
    //             subjectGoodQuizzes = ShuffleQuizzes(subjectGoodQuizzes);
    //         }
    //         else
    //         {
    //             var otherSubjects = Const.OthersSubjects[grade];
    //             foreach (var otherSubject in otherSubjects)
    //             {
    //                 var otherSubjectGoodQuizzes = goodQuizzes.Where(quiz => quiz.subject == otherSubject).ToList();
    //                 subjectGoodQuizzes.AddRange(otherSubjectGoodQuizzes);
    //                 subjectGoodQuizzes = ShuffleQuizzes(subjectGoodQuizzes);
    //             }
    //         }
    //         
    //         selectQuizzes.AddRange(subjectGoodQuizzes.Take(subjectQuizNum).ToList());
    //     }
    //     
    //     selectQuizzes = ShuffleQuizzes(selectQuizzes); 
    // }
    //
    // private void SelectByRandomAlgorithm(int grade, int quizNum)
    // {
    //     var filteredMasterQuizzes = RemovePlayedQuizzes(50);
    //     
    //     var quizzes = filteredMasterQuizzes.Where(quiz => quiz.targetGrade == grade).ToList();
    //     
    //     //問題数調整アルゴリズム(仮)
    //     //↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
    //     var additionalQuizzes = new List<QuizData>();
    //     if (grade == 4)
    //     {
    //         var socialAndScienceQuizzes = filteredMasterQuizzes.Where(quiz => quiz.targetGrade == 3).ToList();
    //         socialAndScienceQuizzes = socialAndScienceQuizzes.Where(quiz => quiz.subject == "理科" || quiz.subject == "社会").ToList();
    //         additionalQuizzes.AddRange(socialAndScienceQuizzes);
    //     }else if (grade == 5)
    //     {
    //         var socialAndScienceQuizzes = filteredMasterQuizzes.Where(quiz => quiz.targetGrade == 3 || quiz.targetGrade == 4).ToList();
    //         socialAndScienceQuizzes = socialAndScienceQuizzes.Where(quiz => quiz.subject == "理科" || quiz.subject == "社会").ToList();
    //         additionalQuizzes.AddRange(socialAndScienceQuizzes);
    //     }else if (grade == 6)
    //     {
    //         var bushuQuizzes = filteredMasterQuizzes.Where(quiz => quiz.quizTags.Any(tag => tag == "部首")).ToList();
    //         additionalQuizzes.AddRange(bushuQuizzes);
    //         
    //         var socialAndScienceQuizzes = filteredMasterQuizzes.Where(quiz => quiz.targetGrade == 3 || quiz.targetGrade == 4).ToList();
    //         socialAndScienceQuizzes = socialAndScienceQuizzes.Where(quiz => quiz.subject == "理科" || quiz.subject == "社会").ToList();
    //         additionalQuizzes.AddRange(socialAndScienceQuizzes);
    //     }
    //     quizzes.AddRange(additionalQuizzes);
    //     //↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
    //     
    //     UserDataManager.UserData userData = UserDataManager.GetInstance().GetUserData();
    //     int stage = userData.stage;
    //     
    //     var levelRatios = Const.StageToLevelRatio[stage];
    //     int remainingQuizzesNum = quizNum;
    //
    //     for (int level = levelRatios.Count; level >= 1 && remainingQuizzesNum > 0; level--)
    //     {
    //         int levelQuizzesCount =
    //             (int)Math.Round(quizNum * (levelRatios[level - 1] / 100.0), MidpointRounding.AwayFromZero);
    //
    //         if (level == 1)
    //         {
    //             // 最後のレベルで不足分を補う
    //             levelQuizzesCount = remainingQuizzesNum;
    //         }
    //         else
    //         {
    //             levelQuizzesCount = Math.Min(levelQuizzesCount, remainingQuizzesNum);
    //         }
    //
    //         var quizzesByLevel = quizzes.Where(quiz => quiz.quizLevel == level).ToList();
    //         quizzesByLevel = ShuffleQuizzes(quizzesByLevel); // リストをシャッフル
    //
    //         // 必要な数を取得し、選択リストに追加
    //         levelQuizzesCount = Math.Min(levelQuizzesCount, quizzesByLevel.Count);
    //         selectQuizzes.AddRange(quizzesByLevel.Take(levelQuizzesCount).ToList());
    //         remainingQuizzesNum -= levelQuizzesCount; // 残りの問題数を更新
    //     }
    //
    //     selectQuizzes = ShuffleQuizzes(selectQuizzes); 
    // }
    //
    // private void SelectByCalculationMode(int grade)
    // {
    //     var masterQuizzes = MasterData.GetInstance().quizzes.ToList();
    //     var keisanQuizzes = masterQuizzes.Where(quiz => quiz.targetGrade <= grade).ToList();
    //     keisanQuizzes = keisanQuizzes.Where(quiz => quiz.subject == "算数").ToList();
    //     keisanQuizzes = keisanQuizzes.Where(quiz => quiz.quizTags.Any(tag => tag == "計算")).ToList();
    //     
    //     var easyQuizzes = keisanQuizzes.Where(quiz => !((quiz.targetGrade == 2 && quiz.quizLevel == 3) || (quiz.targetGrade == 3 && quiz.quizLevel == 5))).ToList(); //問題数調整
    //     easyQuizzes = ShuffleQuizzes(easyQuizzes);
    //     selectQuizzes.AddRange(easyQuizzes.Take(30).ToList());
    //     
    //     var mediumQuizzes = keisanQuizzes.Where(quiz => !(quiz.targetGrade == 3 && quiz.quizLevel == 5)).ToList(); //問題数調整
    //     mediumQuizzes = ShuffleQuizzes(mediumQuizzes);
    //     selectQuizzes.AddRange(mediumQuizzes.Take(20).ToList());
    //     
    //     var hardQuizzes = ShuffleQuizzes(keisanQuizzes); 
    //     selectQuizzes.AddRange(hardQuizzes.ToList());
    // }
    //
    // private List<QuizData> RemovePlayedQuizzes(int removeNum)
    // {
    //     var answerDataList = UserDataManager.GetInstance().GetAnswerDataList();
    //     answerDataList = answerDataList.OrderByDescending(answerData => answerData.dateTime).Take(removeNum).ToList();
    //     
    //     var masterQuizzes = MasterData.GetInstance().quizzes.ToList();
    //     var filteredMasterQuizzes = masterQuizzes.Where(quiz =>
    //     {
    //         foreach (var answerData in answerDataList)
    //         {
    //             if (quiz.id == answerData.quizId)
    //             {
    //                 return false;
    //             }
    //         }
    //         return true;
    //     }).ToList();
    //     return filteredMasterQuizzes;
    // }
    //
    // private List<QuizData> ShuffleQuizzes(List<QuizData> quizzes)
    // {
    //     var random = new System.Random();
    //     var randomQuizzes = quizzes.OrderBy(x => random.Next()).ToList();
    //     return randomQuizzes;
    // }
    //
    // public Const.PlayMode GetPlayMode()
    // {
    //     return _playMode;
    // }
}
