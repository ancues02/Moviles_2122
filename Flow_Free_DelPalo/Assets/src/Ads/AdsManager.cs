using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
namespace FlowFree
{
    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField] string gameId = "4506529";

        [SerializeField] bool testMode = false;

        [SerializeField] AdsInterstitial adsInterstitial;
        [SerializeField] AdsRewarded adsRewarded;

        void Awake()
        {
            // Initialize Ads service
            Advertisement.Initialize(gameId, testMode, this);
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads initialization failed: {error.ToString()} - {message}");
        }

        public void ShowRewardedAd()
        {
            adsRewarded.ShowAd();
        }

        public void ShowInterstitialAd()
        {
            adsInterstitial.ShowAd();
        }
    }
}