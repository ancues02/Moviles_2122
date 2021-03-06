using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
namespace FlowFree
{
    /// <summary>
    /// El manager de los anuncios. Es el encargado de inicializar los anuncios y 
    /// cuando hace falta mostrar un anuncio, se hace atraves de el
    /// </summary>
    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
    {
        /// <summary>
        /// el id del juego de Android
        /// </summary>
        [SerializeField] string gameId = "4506529";

        /// <summary>
        /// Mostrar anuncio de pruebas
        /// </summary>
        [SerializeField] bool testMode = false;

        /// <summary>
        /// Enlace  al encargado de anuncios interstital
        /// </summary>
        [SerializeField] AdsInterstitial adsInterstitial;

        /// <summary>
        /// Enlace al encargado de anuncios rewarded
        /// </summary>
        [SerializeField] AdsRewarded adsRewarded;

        /// <summary>
        /// Enlace al encargado de banners
        /// </summary>
        [SerializeField] AdsBanner adsBanner;

        /// <summary>
        /// Enlace al lvlManager, necesita poder comunicarle que ha acabado el anuncio
        /// </summary>
        [SerializeField] LevelManager lvlManager;


        void Awake()
        {
            // Initialize Ads service
            Advertisement.Initialize(gameId, testMode, this);
            
        }

        /// <summary>
        /// Cuando se inicializa el anuncio
        /// </summary>
        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete");
        }

        /// <summary>
        /// Si da algun fallo al intentar inicializar el anuncio
        /// </summary>
        /// <param name="error"> el tipo de error </param>
        /// <param name="message"> el mensaje del error</param>
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads initialization failed: {error.ToString()} - {message}");
        }

        /// <summary>
        /// Mostrar un anuncio de tipo Rewarded
        /// </summary>
        public void ShowRewardedAd()
        {
            if(Advertisement.isInitialized)
                adsRewarded.ShowAd();
        }

        public void OnRewardedAdEnded()
        {
            if(GameManager.getInstance())
                GameManager.getInstance().IncreaseHints(1);
        }

        /// <summary>
        /// Mostrar un anuncio de tipo Interstitial. Solo se llama al pasar de nivel
        /// asi que si no se ha iniciado, cambiar de nivel directamente para que se pueda jugar
        /// sin conexion a internet
        /// </summary>
        public void ShowInterstitialAd()
        {
            if(Advertisement.isInitialized)
                adsInterstitial.ShowAd();
            else if (lvlManager)
                lvlManager.ChangeLevel(true);
        }


        public void OnInterstitialAdEnded()
        {
            if (lvlManager)
                lvlManager.ChangeLevel(true);
        }

        /// <summary>
        /// Mostrar un anuncio tipo Banner
        /// </summary>
        public void ShowBannerAd()
        {
            if(Advertisement.isInitialized)
                adsBanner.ShowBannerAd();
        }
    }
}