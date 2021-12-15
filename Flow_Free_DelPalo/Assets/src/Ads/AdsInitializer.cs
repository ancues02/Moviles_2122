using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
namespace FlowFree
{
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
#if UNITY_IOS
    [SerializeField] string gameId = "4506528";
#elif UNITY_ANDROID
        [SerializeField] string gameId = "4506529";
#endif

        //[SerializeField] public string miVideoPlacement = "rewardedVideo";
        // [SerializeField] public string myAdStatus = "";
        [SerializeField] public bool enablePerPlacementMode = true;
        [SerializeField] bool testMode = true;

        // ShowOptions options = new ShowOptions();

        void Start()
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

        public void ShowRewardedVideo()
        {

        }
        public void ShowInterstitialAd()
        {
            //Advertisement.a
            //if (Advertisement.isInitialized)
            //    Advertisement.Show(options);
        }

    }
}