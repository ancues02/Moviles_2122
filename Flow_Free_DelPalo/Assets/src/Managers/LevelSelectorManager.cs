using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pasar al gamermanagar el nivel que se va a jugar
//y el gamanager se lo pasa a boardmanager
//
//lo mismo con la categoria para llegar hasta aqui.
namespace FlowFree
{
    public class LevelSelectorManager : MonoBehaviour
    {
        public GameObject levelPagePref;

        // Objeto del canvas que contendra los displays de las categorias
        public Transform display;

        LevelPage[] pages;  // el nº de paginas depende del nº de niveles del lote
        public void setPack(LevelPack pack)
        {
            GameObject categoryObj;
            PackDisplay categoryDplay;
            for (int i = 0; i < pack.pages.Length; i++)
            {
                categoryObj = Instantiate(levelPagePref, display);
                categoryDplay = categoryObj.GetComponent<PackDisplay>();
                categoryDplay.setAttributes(pack, i);
            }
        }

    }
}