using System;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;
using System.Threading;

public class AdManager
{
    private static AdManager _instance;
    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private int _retryAttempt;
    private SynchronizationContext mainThread;
    
#if UNITY_ANDROID
    private string _bannerAdUnitId = "ca-app-pub-1912855275020208/2761286243";
    private string _interstitialAdUnitId = "ca-app-pub-1912855275020208/6317387875";
#elif UNITY_IOS
    private string _bannerAdUnitId = "ca-app-pub-1912855275020208/7362876236";
    private string _interstitialAdUnitId = "ca-app-pub-1912855275020208/9647911189";
#else
    private string _bannerAdUnitId = "unused";
    private string _interstitialAdUnitId = "unused";
#endif

    public static AdManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AdManager();
                _instance.mainThread = SynchronizationContext.Current;
            }
            return _instance;
        }
    }
    
    public void Initialize()
    {
        MobileAds.Initialize(status =>
        {
            
        });
        CreateBannerView();
        LoadInterstitialAd();
    }

    public void CreateBannerView()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
        
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(_bannerAdUnitId, adSize, AdPosition.Bottom);
        _bannerView.OnAdPaid += AdjustComponent.HandlePaidEvent;
    }
    
    public void LoadBannerAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var request = new AdRequest();
        _bannerView?.LoadAd(request);
    }
    
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
        
        InterstitialAd.Load(_interstitialAdUnitId, new AdRequest(), (async (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Interstitial ad load failed");
                HandleFailedToLoad();
            }
            else
            {
                Debug.Log("Interstitial ad loaded");
                _interstitialAd = ad;
                _retryAttempt = 0;
            }
        }));
    }
    
    private async void HandleFailedToLoad()
    {
        _retryAttempt++;
        var retryDelay = Math.Pow(2, Math.Min(6, _retryAttempt));
        await UniTask.Delay((int)(retryDelay * 1000));
        LoadInterstitialAd();
    }
    
    public void ShowInterstitialAd(Action onAdClosed = null)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                mainThread.Post(_ => onAdClosed?.Invoke(), null);
            };
            _interstitialAd.OnAdFullScreenContentClosed += LoadInterstitialAd;
            _interstitialAd.OnAdFullScreenContentFailed += (sender) =>
            {
                mainThread.Post(_ => onAdClosed?.Invoke(), null); 
            };
            _interstitialAd.OnAdFullScreenContentFailed += (sender) => LoadInterstitialAd();
            _interstitialAd.OnAdPaid += AdjustComponent.HandlePaidEvent;
            _interstitialAd.Show();
        }
        else
        {
            mainThread.Post(_ => onAdClosed?.Invoke(), null);
        }
    }
}