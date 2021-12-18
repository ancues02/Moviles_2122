using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Display de un lote logico.
     * Se usa en la escena de seleccion de nivel
     */
    public class PackDisplay : MonoBehaviour
    {
        /// <summary>
        /// Prefab del boton de seleccion de nivel
        /// </summary>
        public GameObject buttonPref;

        /// <summary>
        /// Objeto padre de todos los botones
        /// </summary>
        public Transform buttonGroup;

        /// <summary>
        /// Texto del nombre de la pagina
        /// </summary>
        public Text pageName;

        /// <summary>
        /// Configura el display segun las caracteristicas del lote logico
        /// </summary>
        /// <param name="pack"> Lote logico</param>
        /// <param name="pageIndex"> Indice de la pagina de niveles</param>
        public void setAttributes(Logic.GamePack pack, int pageIndex)
        {
            pageName.text = pack.Pages[pageIndex].name;
            LevelSelectionButton lvlButton;
            for (int j = 0; j < Page.LEVELS_PER_PAGE; j++)
            {
                int levelIndex = j + pageIndex * Page.LEVELS_PER_PAGE;
                lvlButton = Instantiate(buttonPref, buttonGroup).GetComponent<LevelSelectionButton>();
                lvlButton.setLevelNumber(pack.Maps[levelIndex].LevelNumber);
                lvlButton.setColor(pack.Pages[pageIndex].color);
                lvlButton.setBlocked(pack.LastUnlockedLevel < levelIndex);
                lvlButton.setOnClick(() => {
                    GameManager.getInstance().SetSelectedLevel(levelIndex);
                    GameManager.getInstance().ChangeScene("Game Board");
                });

                if (pack.BestMoves[levelIndex] > -1)
                {   
                    if(pack.BestMoves[levelIndex] == pack.Maps[levelIndex].FlowNumber)
                        lvlButton.setPerfect();
                    else
                        lvlButton.setComplete();
                }       
            }                  
        }
    }
}
