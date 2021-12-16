using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace FlowFree
{
    public class LevelSelectorManager : MonoBehaviour
    {
        ///<summary>
        /// Prefab del display de la pagina de niveles
        ///</summary>
        public GameObject levelPagePref;

        ///<summary>
        /// Prefab del maracador de la pagina de niveles
        ///</summary>
        public GameObject pageMarkerPref;

        ///<summary>
        /// Objeto del canvas que contendra los displays de las paginas
        ///</summary>
        public RectTransform display;

        ///<summary>
        /// Objeto del canvas que contendra los marcadores de las paginas
        ///</summary>
        public Transform markerDisplay;

        ///<summary>
        /// Texto con el nombre del lote
        ///</summary>
        public Text packText;

        Image[] markerImages;

        ///<summary>
        /// Establece las paginas segun el lote correspondiente
        ///</summary>
        public void setPack(LevelPack pack)
        {
            packText.text = pack.packName;
            if (pack.pages.Length <= 0) return; // si no hay ninguna pagina no hacemos nada
           
            markerImages = new Image[pack.pages.Length];
            // Ajustamos el display segun el nº de paginas
            /*float newX = display.sizeDelta.x + pack.pages.Length * ((RectTransform)levelPagePref.transform).sizeDelta.x;
            display.sizeDelta = new Vector2(newX, display.sizeDelta.y);*/

            // Creamos los displays de las paginas y los marcadores
            PackDisplay packDplay;
            for (int i = 0; i < pack.pages.Length; i++)
            {
                packDplay = Instantiate(levelPagePref, display).GetComponent<PackDisplay>();
                packDplay.setAttributes(pack, i);
                markerImages[i] = Instantiate(pageMarkerPref, markerDisplay).GetComponent<Image>();
            }
            markerImages[0].color = Color.white;

        }

        ///<summary>
        /// Cambia el color del marcador de la pagina,
        /// se llama cuando se esta haciendo scroll
        ///</summary>
        public void changePageMarker(Vector2 v)
        {
            foreach(Image i in markerImages)
            {
                i.color = Color.gray;
            }

            float x = Mathf.Clamp(v.x, 0f, 0.8f);
            markerImages[(int)(x * markerImages.Length)].color = Color.white;
        }
    }
}