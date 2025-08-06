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
            MaxProduct = 100
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
                (a, b, correctAnswer, question) = GenerateAdditionProblem(rule.DigitLevel);
                break;
            case QuizType.Subtraction:
                (a, b, correctAnswer, question) = GenerateSubtractionProblem(rule.DigitLevel);
                break;
            case QuizType.Multiplication:
                (a, b, correctAnswer, question) = GenerateMultiplicationProblem(rule.DigitLevel, rule.MaxProduct);
                break;
            default:
                (a, b, correctAnswer, question) = GenerateAdditionProblem(rule.DigitLevel);
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

    
    private static (int a, int b, int answer, string question) GenerateAdditionProblem(int digitLevel)
    {
        if (digitLevel == 1)
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            int answer = a + b;
            string question = $"{a} + {b} = ?";
            return (a, b, answer, question);
        }
        else
        {
            // 2桁の加算: 繰り上がりなし、一の位は 1〜5
            int tenA = Random.Range(1, 10);
            int unitA = Random.Range(1, 6); // 1〜5

            int maxUnitB = 9 - unitA; // 繰り上がらない範囲で制限
            int unitB = Random.Range(1, Mathf.Min(6, maxUnitB + 1));
            int tenB = Random.Range(0, 10);

            int a = tenA * 10 + unitA;
            int b = tenB * 10 + unitB;
            int answer = a + b;
            string question = $"{a} + {b} = ?";
            return (a, b, answer, question);
        }
    }


    private static (int a, int b, int answer, string question) GenerateSubtractionProblem(int digitLevel)
    {
        if (digitLevel == 1)
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            if (a < b) (a, b) = (b, a);
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

            if (a < b) (a, b) = (b, a); // 念のため入れ替え
            int answer = a - b;
            string question = $"{a} - {b} = ?";
            return (a, b, answer, question);
        }
    }


    
    private static (int a, int b, int answer, string question) GenerateMultiplicationProblem(int digitLevel, int maxProduct)
    {
        int a = 0, b = 0, answer = 0;
        int attempt = 0;

        while (true)
        {
            attempt++;
            if (attempt > 100)
            {
                a = Random.Range(1, 10);
                b = Random.Range(1, 10);
                break;
            }

            if (digitLevel == 1)
            {
                // ✅ 1桁 × 1桁
                a = Random.Range(1, 10);
                b = Random.Range(1, 10);
            }
            else if (digitLevel == 2)
            {
                // ✅ 2桁 × 1桁
                a = Random.Range(10, 50);  // 2桁
                b = Random.Range(1, 10);    // 1桁
            }

            if (a * b <= maxProduct)
            {
                answer = a * b;
                break;
            }
        }

        string question = $"{a} × {b} = ?";
        return (a, b, answer, question);
    }


    
}
