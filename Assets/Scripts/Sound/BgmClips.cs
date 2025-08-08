using UnityEngine;

public static class BgmClips
{
    public enum BgmType
    {
        None,
        GameScene,
        Select,
        Result
    }

    private static AudioSource _bgmSource;
    private static AudioClip _gameSceneClip;
    private static AudioClip _selectClip;
    private static AudioClip _resultClip; 
    private static BgmType _currentBgm = BgmType.None;

    private const float DefaultVolume = 0.5f;

    // 初期化
    private static void Init()
    {
        if (_bgmSource != null) return;

        // 常駐用GameObject & AudioSource
        var go = new GameObject("BgmClips_AudioSource");
        Object.DontDestroyOnLoad(go);
        _bgmSource = go.AddComponent<AudioSource>();
        _bgmSource.playOnAwake = false;
        _bgmSource.loop = true;
        _bgmSource.volume = DefaultVolume;

        // Resources からBGMを読み込み
        _gameSceneClip   = Resources.Load<AudioClip>("BGM/question_scene");   // Assets/Resources/BGM/room_bgm.ogg
        _selectClip = Resources.Load<AudioClip>("BGM/stageselect_customroom");
        _resultClip = Resources.Load<AudioClip>("BGM/result");

        if (_gameSceneClip == null) Debug.LogWarning("Room BGM not found in Resources/BGM/room_bgm");
        if (_selectClip == null) Debug.LogWarning("Select BGM not found in Resources/BGM/select_bgm");
    }

    public static void Play(BgmType type)
    {
        Init();
        if (_currentBgm == type) return; // 同じ曲なら何もしない

        _currentBgm = type;
        _bgmSource.Stop();

        switch (type)
        {
            case BgmType.GameScene:
                if (_gameSceneClip != null) _bgmSource.clip = _gameSceneClip;
                break;
            case BgmType.Select:
                if (_selectClip != null) _bgmSource.clip = _selectClip;
                break;
            case BgmType.Result:
                if (_resultClip != null) _bgmSource.clip = _resultClip;
                break;
            default:
                _bgmSource.clip = null;
                return;
        }

        if (_bgmSource.clip != null)
            _bgmSource.Play();
    }

    public static void Stop()
    {
        if (_bgmSource != null)
        {
            _bgmSource.Stop();
            _currentBgm = BgmType.None;
        }
    }

    public static void SetVolume(float volume)
    {
        Init();
        _bgmSource.volume = Mathf.Clamp01(volume);
    }

    public static BgmType GetCurrentBgm() => _currentBgm;
}
