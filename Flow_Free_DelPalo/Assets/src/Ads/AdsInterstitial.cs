using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
namespace FlowFree
{
    public class AdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
#if UNITY_IOS
        [SerializeField] string AdUnityId = "Interstitial_IOS";
#elif UNITY_ANDROID
        [SerializeField] string AdUnityId = "Interstitial_Android";
#endif
        public LevelManager lvlManager;
        private void Start()
        {
            if (!lvlManager)
                Debug.LogError("Falta una referencia en AdsInterstitial");
            LoadAd();
        }

        public void LoadAd()
        {
            Debug.Log("Loading ad " + AdUnityId);
            Advertisement.Load(AdUnityId, this);
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
            Debug.Log("CERRAR ANUNCIO");
            lvlManager.NextLevel();
        }
    }
}