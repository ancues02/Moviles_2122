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

        [SerializeField] bool testMode = false;


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
    }
}