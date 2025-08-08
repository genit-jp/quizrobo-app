using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class QuizGenerator
{
    private enum QuizType
    {
        Addition,
        Subtraction,
        Multiplication
    }

    private class ChallengeLevelRule
    {
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
    
        public int DigitLevel { get; set; }
        public List<QuizType> AllowedTypes { get; set; }
        public int MaxProduct { get; set; } = 100;
        public int ChoiceRange { get; set; } = 5;
        
        public int MinNumber { get; set; } = 1;
        public int MaxNumber { get; set; } = 9;

        public bool Matches(int level)
        {
            return level >= MinLevel && level <= MaxLevel;
        }
    }


    private static readonly List<ChallengeLevelRule> _challengeRules = new()
    {
        new ChallengeLevelRule
        {
            MinLevel = 1,
            MaxLevel = 20,
            DigitLevel = 1,
            AllowedTypes = new() { QuizType.Addition, QuizType.Subtraction }
        },
        new ChallengeLevelRule
        {
            MinLevel = 21,
            MaxLevel = 40,
            DigitLevel = 1,
            AllowedTypes = new() { QuizType.Addition, QuizType.Subtraction, QuizType.Multiplication },
            MaxProduct = 100,
            MinNumber = 1, 
            MaxNumber = 5 
        },
        new ChallengeLevelRule
        {
            MinLevel = 41,
            MaxLevel = int.MaxValue,
            DigitLevel = 2,
            AllowedTypes = new() { QuizType.Addition, QuizType.Subtraction, QuizType.Multiplication },
            MaxProduct = 200
        }
    };


    public static QuizData[] GenerateRandomQuizList(int challengeLevel)
    {
        return GenerateQuizDataList(challengeLevel);
    }
    
    private static QuizData[] GenerateQuizDataList(int challengeLevel)
{
    var rule = _challengeRules.FirstOrDefault(r => r.Matches(challengeLevel));
    if (rule == null)
    {
        Debug.LogError($"No matching rule for challengeLevel: {challengeLevel}");
        return new QuizData[0];
    }

    var quizDataArray = new QuizData[10];

    for (int i = 0; i < 10; i++)
    {
        // 許可されたタイプからランダム選択
        var quizType = rule.AllowedTypes[Random.Range(0, rule.AllowedTypes.Count)];

        int a, b, correctAnswer;
        string question;

        switch (quizType)
        {
            case QuizType.Addition:
                (a, b, correctAnswer, question) = GenerateAdditionProblem(rule);
                break;
            case QuizType.Subtraction:
                (a, b, correctAnswer, question) = GenerateSubtractionProblem(rule);
                break;
            case QuizType.Multiplication:
                (a, b, correctAnswer, question) = GenerateMultiplicationProblem(rule);
                break;
            default:
                (a, b, correctAnswer, question) = GenerateAdditionProblem(rule);
                break;
        }

        // 選択肢の生成（ChoiceRange を使用）
        var choicesList = new List<int> { correctAnswer };
        while (choicesList.Count < 4)
        {
            int distractor = correctAnswer + Random.Range(-rule.ChoiceRange, rule.ChoiceRange + 1);
            if (distractor >= 0 && !choicesList.Contains(distractor))
            {
                choicesList.Add(distractor);
            }
        }

        // シャッフル
        choicesList = choicesList.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();

        quizDataArray[i] = new QuizData
        {
            Id = $"auto_{i + 1}",
            available = true,
            question = question,
            choices = choicesList.Select(x => x.ToString()).ToArray(),
            answer = correctAnswer.ToString(),
        };
    }

    return quizDataArray;
}

    
    private static (int a, int b, int answer, string question) GenerateAdditionProblem(ChallengeLevelRule rule)
    {
        if (rule.DigitLevel == 1)
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            int answer = a + b;
            string question = $"{a} + {b} = ?";
            return (a, b, answer, question);
        }
        else
        {
            // 2桁の場合は従来通り
            int tenA = Random.Range(1, 10);
            int unitA = Random.Range(1, 6);
            int maxUnitB = 9 - unitA;
            int unitB = Random.Range(1, Mathf.Min(6, maxUnitB + 1));
            int tenB = Random.Range(0, 10);

            int a = tenA * 10 + unitA;
            int b = tenB * 10 + unitB;
            int answer = a + b;
            string question = $"{a} + {b} = ?";
            return (a, b, answer, question);
        }
    }



    private static (int a, int b, int answer, string question) GenerateSubtractionProblem(ChallengeLevelRule rule)
    {
        if (rule.DigitLevel == 1)
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            if (a < b) (a, b) = (b, a); // a ≥ b にする
            int answer = a - b;
            string question = $"{a} - {b} = ?";
            return (a, b, answer, question);
        }
        else
        {
            // 2桁の減算: 繰り下がりなし
            int tenA = Random.Range(1, 10);
            int unitA = Random.Range(0, 10);
            int a = tenA * 10 + unitA;

            int tenB = Random.Range(0, tenA + 1);
            int unitB = Random.Range(0, unitA + 1);
            int b = tenB * 10 + unitB;

            if (a < b) (a, b) = (b, a);
            int answer = a - b;
            string question = $"{a} - {b} = ?";
            return (a, b, answer, question);
        }
    }



    
    private static (int a, int b, int answer, string question) GenerateMultiplicationProblem(ChallengeLevelRule rule)
    {
        int a = 0, b = 0, answer = 0;
        int attempt = 0;

        while (true)
        {
            attempt++;
            if (attempt > 100)
            {
                a = Random.Range(rule.MinNumber, rule.MaxNumber + 1);
                b = Random.Range(rule.MinNumber, rule.MaxNumber + 1);
                break;
            }

            if (rule.DigitLevel == 1)
            {
                a = Random.Range(rule.MinNumber, rule.MaxNumber + 1);
                b = Random.Range(rule.MinNumber, rule.MaxNumber + 1);
            }
            else if (rule.DigitLevel == 2)
            {
                a = Random.Range(10, 50);
                b = Random.Range(1, 10);
            }

            if (a * b <= rule.MaxProduct)
            {
                answer = a * b;
                break;
            }
        }

        string question = $"{a} × {b} = ?";
        return (a, b, answer, question);
    }



    
}
