
using UnityEngine;

public abstract class QuizUI: MonoBehaviour
{
    public abstract void Setup(QuizData quizData, System.Action<bool, string> answeredByUser);
    public abstract void DestroyGameUI();
}
