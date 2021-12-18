using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /// <summary>
    /// Se encarga de avisar al adsManager de que muestre un anuncio de cierto tipo
    /// cuando un boton con este componente ha sido pulsado
    /// </summary>
    public class ShowAdOnButtonClick : MonoBehaviour
    {
        /// <summary>
        /// Tipos de anuncio, no hay banner porque no lo necesitamos en los botones
        /// </summary>
        public enum ADType { Rewarded, Instertitial};

        /// <summary>
        /// El tipo de anuncio que vamos a querer mostrar al pulsar el boton
        /// </summary>
        /// 
        public ADType adType;
        /// <summary>
        /// El boton en cuestion para aniaderle el callback
        /// </summary>
        public Button button;

        /// <summary>
        /// Referencia al AdsManager
        /// </summary>
        [SerializeField]
        AdsManager adManager;

        /// <summary>
        /// Poner el callback que queremos al boton
        /// </summary>
        void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (adType == ADType.Rewarded)
                {
                    adManager.ShowRewardedAd();
                }
                else
                    adManager.ShowInterstitialAd();
            });

        }
    }
}
