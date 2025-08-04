using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class NumpadUI : MonoBehaviour
{
    [SerializeField] private Transform numpadContainer;
    [SerializeField] private TextMeshProUGUI answerDisplay;
    [SerializeField] private GridLayoutGroup gridLayout;
    
    private QuizData _quizData;
    private Action<bool, string> _answeredByUser;
    private string _currentInput = "";

    public async void Setup(QuizData quizData, Action<bool, string> answeredByUser)
    {
        _quizData = quizData;
        _answeredByUser = answeredByUser;
        _currentInput = "";
        UpdateDisplay();

        // グリッドレイアウトの設定
        if (gridLayout == null)
        {
            gridLayout = numpadContainer.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                gridLayout = numpadContainer.gameObject.AddComponent<GridLayoutGroup>();
            }
        }

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.cellSize = new Vector2(150, 150);
        gridLayout.spacing = new Vector2(10, 10);

        // 既存のボタンをクリア
        foreach (Transform child in numpadContainer)
        {
            Destroy(child.gameObject);
        }

        // ボタンプレハブをロード
        var buttonPrefab = await Resources.LoadAsync<GameObject>("Prefabs/Game/NumpadButton");

        // 数字ボタン 1～9 を作成
        for (int i = 1; i <= 9; i++)
        {
            CreateNumberButton(buttonPrefab as GameObject, i.ToString());
        }

        // 4行目：左→0、中央→削除、右→空白
        CreateNumberButton(buttonPrefab as GameObject, "0");
        CreateDeleteButton(buttonPrefab as GameObject);
        CreateEmptyCell();
    }

    private void CreateNumberButton(GameObject prefab, string number)
    {
        GameObject buttonObj = Instantiate(prefab, numpadContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = number;
            buttonText.fontSize = 36;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnNumberPressed(number));
    }

    private void CreateDeleteButton(GameObject prefab)
    {
        GameObject buttonObj = Instantiate(prefab, numpadContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = "削除";
            buttonText.fontSize = 24;
        }

        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = new Color(1f, 0.7f, 0.7f); // 薄い赤
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnDelete);
    }

    private void CreateEmptyCell()
    {
        GameObject empty = new GameObject("EmptyCell");
        empty.transform.SetParent(numpadContainer);

        RectTransform rt = empty.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
    }

    private void OnNumberPressed(string number)
    {
        if (_currentInput.Length < 5)
        {
            _currentInput += number;
            UpdateDisplay();

            if (_currentInput.Length == _quizData.answer.Length)
            {
                CheckAnswer();
            }
        }
    }

    private void OnDelete()
    {
        if (_currentInput.Length > 0)
        {
            _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    private void CheckAnswer()
    {
        bool isCorrect = _currentInput == _quizData.answer;
        _answeredByUser?.Invoke(isCorrect, _currentInput);
    }

    private void UpdateDisplay()
    {
        if (answerDisplay != null)
        {
            answerDisplay.text = string.IsNullOrEmpty(_currentInput) ? "?" : _currentInput;
        }
    }

    public void DestroyGameUI()
    {
        foreach (Transform child in numpadContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
