using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Genit;
using UnityEngine;

public class SubjectSelectPanel : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;

    public async void Setup()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); 
        }

        var subjects = MasterData.GetInstance().AllSubjects();
        int buttonCount = 5;

        var containerRect = buttonContainer.GetComponent<RectTransform>().rect;
        float containerHeight = containerRect.height;
        float containerWidth = containerRect.width;

        float amplitude = (containerWidth - 100) / 4f;
        float wavelength = (containerHeight * 2f - 400) / 5f ; 
        
        float headOffset = 100f;

        for (int i = 0; i < buttonCount; i++)
        {
            var subject = subjects[i];
            var subjectButton = await Utils.InstantiatePrefab("Prefabs/Select/SubjectButton", buttonContainer);
            var btn = subjectButton.GetComponent<SubjectButton>();
            btn.Setup(subject, () => this.OnClickSubjectButton(subject));

            var rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            float angle = (2 * i + 1) * Mathf.PI / 2f;
            float y = angle * (wavelength / (2f * Mathf.PI))　+ headOffset;
            float x = Mathf.Sin(angle) * amplitude;

            rect.anchoredPosition = new Vector2(x, -y);
        }
    }


    private void OnClickSubjectButton(string subject)
    {
        Debug.Log($"Selected Subject: {subject}");
        // クイズ開始処理など
    }

    public void OnClickGoBackButton()
    {
        Destroy(this.gameObject);
    }
}