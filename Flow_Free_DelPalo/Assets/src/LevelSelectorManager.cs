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
        public int testLevel;

        const int LEVELS_PER_PAGE = 30;
        LevelPage[] pages;  // el nº de paginas depende del nº de niveles del lote
        
        LevelPack pack;

        private void Update()
        {
            //gestionar la seleccion del nivel y cambiar entre paginas

            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager gm = GameManager.getInstance();
                gm.setSelectedLevel(testLevel);
                gm.ChangeScene("GameBoardTest");
            }
        }

        public void setPack(LevelPack p)
        {
            pack = p;
        }

    }
}