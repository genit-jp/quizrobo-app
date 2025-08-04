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

    public static QuizData[] GenerateRandomQuizList()
    {
        return GenerateQuizDataList();
    }
    
    public static QuizData[] GenerateQuizDataList()
    {
        var quizDataArray = new QuizData[10];
        
        for (int i = 0; i < 10; i++)
        {
            // Generate random quiz type
            var quizType = (QuizType)Random.Range(0, 3);
            
            int a, b, correctAnswer;
            string question;
            
            switch (quizType)
            {
                case QuizType.Addition:
                    (a, b, correctAnswer, question) = GenerateAdditionProblem();
                    break;
                case QuizType.Subtraction:
                    (a, b, correctAnswer, question) = GenerateSubtractionProblem();
                    break;
                case QuizType.Multiplication:
                    (a, b, correctAnswer, question) = GenerateMultiplicationProblem();
                    break;
                default:
                    (a, b, correctAnswer, question) = GenerateAdditionProblem();
                    break;
            }
            
            // Generate choices with distractors
            var choicesList = new List<int> { correctAnswer };
            
            while (choicesList.Count < 4)
            {
                int distractor = correctAnswer + Random.Range(-5, 6);
                
                if (distractor >= 0 && !choicesList.Contains(distractor))
                {
                    choicesList.Add(distractor);
                }
            }
            
            // Shuffle choices
            choicesList = choicesList.OrderBy(x => Random.Range(0, int.MaxValue)).ToList();
            
            // Create QuizData
            quizDataArray[i] = new QuizData
            {
                Id = $"auto_{i + 1}",
                available = true,
                subject = "算数",
                question = question,
                choices = choicesList.Select(x => x.ToString()).ToArray(),
                answer = correctAnswer.ToString(),
                difficultyLevels = "easy"
            };
        }
        
        return quizDataArray;
    }
    
    private static (int a, int b, int answer, string question) GenerateAdditionProblem()
    {
        int a, b;
        
        if (Random.Range(0, 2) == 0)
        {
            // 1-digit + 1-digit
            a = Random.Range(1, 10);
            b = Random.Range(1, 10);
        }
        else
        {
            // 2-digit + 2-digit
            a = Random.Range(10, 100);
            b = Random.Range(10, 100);
        }
        
        int answer = a + b;
        string question = $"{a} + {b} = ?";
        
        return (a, b, answer, question);
    }
    
    private static (int a, int b, int answer, string question) GenerateSubtractionProblem()
    {
        int a, b;
        
        if (Random.Range(0, 2) == 0)
        {
            // 1-digit - 1-digit
            a = Random.Range(1, 10);
            b = Random.Range(1, 10);
        }
        else
        {
            // 2-digit - 2-digit
            a = Random.Range(10, 100);
            b = Random.Range(10, 100);
        }
        
        // Ensure non-negative result
        if (a < b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        
        int answer = a - b;
        string question = $"{a} - {b} = ?";
        
        return (a, b, answer, question);
    }
    
    private static (int a, int b, int answer, string question) GenerateMultiplicationProblem()
    {
        int a, b;
        int multiType = Random.Range(0, 3);
        
        switch (multiType)
        {
            case 0:
                // 1-digit × 1-digit
                a = Random.Range(1, 10);
                b = Random.Range(1, 10);
                break;
            case 1:
                // 1-digit × 2-digit
                a = Random.Range(1, 10);
                b = Random.Range(10, 100);
                break;
            case 2:
                // 1-digit × 3-digit
                a = Random.Range(1, 10);
                b = Random.Range(100, 1000);
                break;
            default:
                a = Random.Range(1, 10);
                b = Random.Range(1, 10);
                break;
        }
        
        int answer = a * b;
        string question = $"{a} × {b} = ?";
        
        return (a, b, answer, question);
    }
}
