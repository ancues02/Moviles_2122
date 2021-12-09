using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pasar al gamermanagar el nivel que se va a jugar
//y el gamanager se lo pasa a boardmanager
//
//lo mismo con la categoria para llegar hasta aqui.
namespace FlowFree
{
    // El nº de niveles en el lote es siempre multiplo de 30,
    // cada pagina tiene 30 tiles.
    struct LevelPage
    {
        string name;
        LevelTile[] tiles;
    }

    public class PackManager : MonoBehaviour
    {
        LevelPack pack;
        LevelPage[] pages;  // el nº de paginas depende del nº de niveles del lote

        void Start()
        {
            GameManager gameManager = GameManager.getInstance();
            pack = gameManager.getSelectedPack();
        }

        private void Update()
        {
            //gestionar la seleccion del nivel y cambiar entre paginas
        }

    }
}