using UnityEngine;

public static class SeClips
{
    private static AudioClip _correctClip;
    private static AudioClip _incorrectClip;
    private static AudioSource _audioSource;

    // 初期化（最初の呼び出し時に自動実行）
    private static void Init()
    {
        if (_audioSource != null) return;

        // 再生用AudioSourceを作る
        var go = new GameObject("SeClips_AudioSource");
        Object.DontDestroyOnLoad(go);
        _audioSource = go.AddComponent<AudioSource>();

        // Resourcesからロード
        _correctClip   = Resources.Load<AudioClip>("SE/answer_correct");  
        _incorrectClip = Resources.Load<AudioClip>("SE/answer_wrong"); 

        if (_correctClip == null) Debug.LogWarning("Correct SE not found in Resources/SE/correct");
        if (_incorrectClip == null) Debug.LogWarning("Incorrect SE not found in Resources/SE/incorrect");
    }

    public static void PlayCorrect()
    {
        Init();
        if (_correctClip != null)
            _audioSource.PlayOneShot(_correctClip);
    }

    public static void PlayIncorrect()
    {
        Init();
        if (_incorrectClip != null)
            _audioSource.PlayOneShot(_incorrectClip);
    }
}