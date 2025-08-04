using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class JudgeScreen : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _explanationPanel;
    [SerializeField] private RubyTextMeshProUGUI _ansText;

    private Action _onClickNextButton;

    public async void Setup(bool isCorrect, QuizData quizData, Action onClickNextButton)
    {
        _onClickNextButton = onClickNextButton;

        _ansText.color = isCorrect
            ? new Color(94f / 255f, 148f / 255f, 232f / 255f)
            : new Color(227f / 255f, 101f / 255f, 85f / 255f);

        _ansText.uneditedText = quizData.answer;

        var judgeTexture = Resources.Load<Texture2D>(isCorrect ? "Images/maru" : "Images/batsu");
        _image.sprite = Sprite.Create(judgeTexture, new Rect(0, 0, judgeTexture.width, judgeTexture.height), new Vector2(0.5f, 0.5f));

        // ⏱ 0.5秒後に自動で閉じる
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _onClickNextButton?.Invoke();
    }
}