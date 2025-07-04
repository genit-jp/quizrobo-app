using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _howToSelectGradePanel;
    [SerializeField] private GameObject[] _howToPlayPanels;
    [SerializeField] private GameObject _nextText;

    public async void SetHowToSelectGrade(Transform targetTransform)
    {
        var parentTransform = targetTransform.parent;
        targetTransform.SetParent(gameObject.transform, true);
        _howToSelectGradePanel.SetActive(true);

        await UniTask.WaitUntil(() => UserDataManager.GetInstance().GetUserData().grade != 0);

        targetTransform.SetParent(parentTransform, true);
        Destroy(gameObject);
    }

    public async void SetHowToBet(Transform targetTransform, Action disableBetButton, Action enableBetButton)
    {
        disableBetButton();
        var parentTransform = targetTransform.parent;
        targetTransform.SetParent(gameObject.transform, true);

        _howToPlayPanels[0].SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _nextText.SetActive(true);
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        _nextText.SetActive(false);
        _howToPlayPanels[0].SetActive(false);

        _howToPlayPanels[1].SetActive(true);
        enableBetButton();
        await UniTask.WaitUntil(() => GetComponentInParent<GameScene>().GetBetNum() == 3);

        targetTransform.SetParent(parentTransform, true);
        Destroy(gameObject);
    }

    public async void SetDescriptionAfterAnswer(Transform targetTransform, Action disableBetButton,
        Action enableBetButton)
    {
        disableBetButton();
        var parentTransform = targetTransform.parent;
        targetTransform.SetParent(gameObject.transform, true);

        _howToPlayPanels[2].SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _nextText.SetActive(true);
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        _nextText.SetActive(false);
        _howToPlayPanels[2].SetActive(false);

        _howToPlayPanels[3].SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _nextText.SetActive(true);
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        _nextText.SetActive(false);

        enableBetButton();
        targetTransform.SetParent(parentTransform, true);
        Destroy(gameObject);
    }
}