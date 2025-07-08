// using System;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.SceneManagement;
// using TMPro;
// using UnityEngine.UI;
//
// public class JudgeScreen : MonoBehaviour, IPointerClickHandler
// {
//     [SerializeField]private Image _image;
//     [SerializeField]private GameObject _explanationPanel;
//     [SerializeField]private RubyTextMeshProUGUI _ansText;
//     [SerializeField]private RubyTextMeshProUGUI _explanationText;
//     private Action _onClickNextButton;
//     
//     public async void Setup(bool isCorrect, int betNum, QuizData quizData, Action onClickNextButton)
//     {
//         if (isCorrect)
//         {
//             _ansText.color = new Color(94f/255f, 148f/255f, 232f/255f);
//         }
//         else
//         {
//             _ansText.color = new Color(227f/255f, 101f/255f, 85f/255f);
//         }
//         _ansText.uneditedText = quizData.answer;
//
//         if (quizData.explanation != "")
//         {
//             _explanationText.uneditedText = quizData.explanation;
//             _explanationPanel.SetActive(true);
//         }
//
//         _onClickNextButton = onClickNextButton;
//         
//         Texture2D judgeTexture;
//         if (isCorrect)
//         {
//             judgeTexture =  Resources.Load<Texture2D>("Images/maru");
//         }
//         else
//         {
//             judgeTexture =  Resources.Load<Texture2D>("Images/batsu");
//         }
//         _image.sprite = Sprite.Create(judgeTexture, new Rect(0, 0, judgeTexture.width, judgeTexture.height), new Vector2(0.5f, 0.5f));
//     }
//     
//     public async void OnPointerClick(PointerEventData eventData)
//     {
//         _onClickNextButton?.Invoke();
//     }
// }