using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace FlowFree
{
    /// <summary>
    /// El encargado de los anuncios tipo banner
    /// </summary>
    public class AdsBanner : MonoBehaviour
    {

        [SerializeField] string AdUnityId = "Banner_Android";
        [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

        /// <summary>
        /// Lo colocamos y lo cargamos al empezar
        /// </summary>
        private void Start()
        {
            Advertisement.Banner.SetPosition(bannerPosition);
            LoadBanner();
        }

        /// <summary>
        /// La carga del banner
        /// </summary>
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

        /// <summary>
        /// Cuando se carga, se muestra
        /// </summary>
        void OnBannerLoaded()
        {
            Debug.Log("Banner loaded");
            ShowBannerAd();            
        }

        /// <summary>
        /// Por si ha habido algun error en la carga del banner
        /// </summary>
        /// <param name="message"> el mensaje de error</param>
        void OnBannerError(string message)
        {
            Debug.Log($"Banner error {AdUnityId} : {message}");
        }

        /// <summary>
        /// Muestra el banner
        /// </summary>
        public void ShowBannerAd()
        {
            Advertisement.Banner.Show(AdUnityId);
        }
    }

}