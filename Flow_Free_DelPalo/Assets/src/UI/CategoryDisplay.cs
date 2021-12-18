using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Display de una categoria logica.
     * Se usa en la escena de Menu
     */
    public class CategoryDisplay : MonoBehaviour
    {
        /// <summary>
        /// Prefab del boton de seleccion de lote
        /// </summary>
        public GameObject buttonPref;

        /// <summary>
        /// Texto con el nombre de la categoria
        /// </summary>
        public Text categoryName;

        /// <summary>
        /// Fondo del texto del nombre de la categoria
        /// </summary>
        public Image categoryBackground;

        /// <summary>
        /// Barra inferior del nombre de la categoria
        /// </summary>
        public Image categoryBar;

        /// <summary>
        /// Configura el display segun las caracteristicas de la categoria logica.
        /// </summary>
        /// <param name="cat"> Categoria logica</param>
        public void setAttributes(Logic.GameCategory cat)
        {
            categoryBackground.color = new Color(cat.Color.r, cat.Color.g, cat.Color.b, 0.6f);
            categoryBar.color = cat.Color;
            categoryName.text = cat.Name;

            MenuButton packButton;
            for (int i = 0; i < cat.PacksArray.Count; i++)
            {
                packButton = Instantiate(buttonPref, transform).GetComponent<MenuButton>();
                packButton.setPackName(cat.PacksArray[i].Name);
                packButton.setPackTextColor(cat.Color);
                packButton.setPackLevels(cat.PacksArray[i].CompletedLevels, cat.PacksArray[i].TotalLevels);
                packButton.setBlocked(cat.PacksArray[i].Blocked);
                
                int packIndex = i;
                packButton.setOnClick(() => {
                     GameManager.getInstance().setLevelPack(cat.Name, packIndex);
                    GameManager.getInstance().ChangeScene("Level Select");             
                });

                /*if (cData.packs[i].completedLevels == cData.packs[i].totalLevels)
                {
                    packButton.set(cData.packs[i].blocked);
                }*/
            }
        }
    }
}
