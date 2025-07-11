using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoadingPanel : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _NormalModeDescription;
    [SerializeField] private GameObject _CalculationModeDescription;

    public void LoadNextScene(string sceneName, LoadSceneMode loadSceneMode)
    {
        gameObject.SetActive(true); // LoadPanelをアクティブにする
        // if(QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Normal)
        // {
        //     _NormalModeDescription.SetActive(true);
        //     _CalculationModeDescription.SetActive(false);
        // }
        // else if(QuizSelectManager.GetInstance().GetPlayMode() == Const.PlayMode.Calculation)
        // {
        //     _NormalModeDescription.SetActive(false);
        //     _CalculationModeDescription.SetActive(true);
        // }
        LoadSceneAsync(sceneName, loadSceneMode).Forget();
    }

    private async UniTaskVoid LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
    {
        int totalItems = 1; 
        int loadedItems = 0;

        // 画像のロード
        // foreach (var quizData in quizList)
        // {
        //     if (!string.IsNullOrEmpty(quizData.imgPath))
        //     {
        //         totalItems++;
        //         await SetImgAsync(quizData);
        //         loadedItems++;
        //         _slider.value = (float)loadedItems / totalItems;
        //     }
        // }
        // シーンの非同期ローディングを開始
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        async.allowSceneActivation = false; // シーンの自動切り替えを防ぐ
        while (!async.isDone)
        {
            // シーンのロード進捗と画像のロード進捗を合わせてスライダーの値を更新
            float progress = Mathf.Clamp01((loadedItems + async.progress) / totalItems);
            _slider.value = progress;

            // ロードがほぼ完了したら
            if (async.progress >= 0.9f && loadedItems == totalItems - 1)
            {
                loadedItems++; // シーンロードを完了したアイテムとしてカウント
                _slider.value = 1f; // スライダーを最大に
                async.allowSceneActivation = true; // シーンの切り替えを許可
            }

            await UniTask.Yield();
        }
        gameObject.SetActive(false); // シーンロード完了後、LoadPanelを非アクティブにする
    }



    private async UniTask SetImgAsync(QuizData quizData)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(quizData.imgPath);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Image download error: {request.error}");
        }
        else
        {
            quizData.imgTexture = DownloadHandlerTexture.GetContent(request);
        }
    }
}