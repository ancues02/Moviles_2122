using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
namespace FlowFree
{
    /// <summary>
    /// El encargado de los anuncios tipo interstitial. Solo salen anuncios de este tipo al acabar un nivel
    /// </summary>
    public class AdsInterstitial : MonoBehaviour , IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] string AdUnityId = "Interstitial_Android";

        /// <summary>
        /// Enlace al lvlManager, necesita poder comunicarle que ha acabado el anuncio
        /// </summary>
        public LevelManager lvlManager;

        /// <summary>
        /// Cargamos el tipo interstitial al empezar la escena
        /// </summary>
        private void Start()
        {
            if (!lvlManager)
                Debug.LogWarning("Falta referencia de LevelManager en AdsInterstitial");
            LoadAd();
        }

        /// <summary>
        /// La carga de los anuncios
        /// </summary>
        public void LoadAd()
        {
            Debug.Log("Loading ad " + AdUnityId);
            Advertisement.Load(AdUnityId, this);
        }


        /// <summary>
        /// Mostrar un anuncio
        /// </summary>
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
            Debug.Log("Anuncio Insterstital empezado");

        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        /// <summary>
        /// Cuando acaba un anuncio, se avisa al lvlManager para que cambie de escena
        /// </summary>
        /// <param name="placementId"></param>
        /// <param name="showCompletionState"></param>
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if(lvlManager)
                lvlManager.ChangeLevel(true);
            Debug.Log("Anuncio Insterstital acabado"); 
        }



    }
}