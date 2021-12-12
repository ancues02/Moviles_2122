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
        /// Objeto del canvas que contendra los displays de las paginas
        ///</summary>
        public Transform display;

        public Text packText;

        ///<summary>
        /// Establece las paginas segun el lote correspondiente
        ///</summary>
        public void setPack(LevelPack pack)
        {
            packText.text = pack.packName;
            PackDisplay packDplay;
            for (int i = 0; i < pack.pages.Length; i++)
            {
                packDplay = Instantiate(levelPagePref, display).GetComponent<PackDisplay>();
                packDplay.setAttributes(pack, i);
            }
        }

    }
}