using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace FlowFree
{
    /// <summary>
    /// El encargado de los anuncios tipo rewarded. Se usan anuncios de este tipo cuando pides una pista
    /// y no te quedan o cuando quieres conseguir otra viendo un anuncio (arriba a la derecha de la UI)
    /// Funcionaba un poco raro, cuando saltaba un anuncio de este tipo te sumaba una pista, cuando salta otro 
    /// te sumaba dos, luego 3... si habian salido 4 anuncios, detectaba al acabar el quinto que se estaban cerrando 5 anuncios
    /// asi que te sumaba 5 pistas. Lo que hemos hecho es poner un contador, para que solo te sume una pista
    /// </summary>
    public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] string AdUnityId = "Rewarded_Android";

        /// <summary>
        /// El boton que va a lanzar los anuncios al pulsarse de la parte superior de la UI
        /// </summary>
        [SerializeField] Button showButton;

        /// <summary>
        /// El contador para que solo sume una pista
        /// </summary>
        int cont = 0;

        /// <summary>
        /// Desactivamos el boton hasta que esten cargados los anuncios
        /// </summary>
        private void Awake()
        {
            showButton.interactable = false;
        }

        /// <summary>
        /// Cargamos los anuncios al empezar
        /// </summary>
        private void Start()
        {
            LoadAd();
        }

        /// <summary>
        /// La carga de anuncios
        /// </summary>
        public void LoadAd()
        {
            Debug.Log("Loading ad " + AdUnityId);
            Advertisement.Load(AdUnityId, this);
        }

        /// <summary>
        /// Mostrar un anuncio de este tipo, ponemos el contador a 1
        /// </summary>
        public void ShowAd()
        {
            Debug.Log("Showing adRewarded " );
            Advertisement.Show(AdUnityId, this);
            cont = 1;
        }

        /// <summary>
        /// Al acabar de cargarse el anuncio, activamos el boton
        /// </summary>
        /// <param name="placementId"></param>
        public void OnUnityAdsAdLoaded(string placementId)
        {
            showButton.interactable = true;
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

        /// <summary>
        /// Al acabar un anuncio, aniade una pista, solo lo hace cuando contador es igual a 1 
        /// para asegurarnos de que solo se aniade una pista y no varias
        /// </summary>
        /// <param name="placementId"></param>
        /// <param name="showCompletionState"></param>
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (GameManager.getInstance() && cont == 1)
                GameManager.getInstance().IncreaseHints(1);
            cont = 0;
        }

    }
}