using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace FlowFree
{
    public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
#if UNITY_IOS
    [SerializeField] string AdUnityId = "Rewarded_IOS";
#elif UNITY_ANDROID
        [SerializeField] string AdUnityId = "Rewarded_Android";
#endif
        [SerializeField] Button showButton;

        private void Awake()
        {
            showButton.interactable = false;
        }

        private void Start()
        {
            LoadAd();
        }

        public void LoadAd()
        {
            Debug.Log("Loading ad " + AdUnityId);
            Advertisement.Load(AdUnityId, this);
            showButton.interactable = true;
        }

        public void ShowAd()
        {
            Debug.Log("Showing ad " + AdUnityId);
            Advertisement.Show(AdUnityId, this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {AdUnityId} : {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {AdUnityId} : {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (GameManager.getInstance())
                GameManager.getInstance().IncreaseHints(1);
        }
    }
}