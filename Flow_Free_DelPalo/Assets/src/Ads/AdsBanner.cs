using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace FlowFree
{
    public class AdsBanner : MonoBehaviour
    {
#if UNITY_ANDROID
        [SerializeField] string AdUnityId = "Banner_Android";
        [SerializeField] BannerPosition bannerPosition = BannerPosition.CENTER;


        private void Start()
        {
            Advertisement.Banner.SetPosition(bannerPosition);
            LoadBanner();
        }

        public void LoadBanner()
        {
            Debug.Log("Loading banner " + AdUnityId);
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };  
            Advertisement.Banner.Load(AdUnityId, options);
        }

        void OnBannerLoaded()
        {
            Debug.Log("Banner loaded");
            ShowBannerAd();            
        }

        void OnBannerError(string message)
        {
            Debug.Log($"Banner error {AdUnityId} : {message}");
        }

        void ShowBannerAd()
        {
            Advertisement.Banner.Show(AdUnityId);
        }
#endif
    }

}