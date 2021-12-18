using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    /**
     * Se encarga de la creacion de los elementos de la
     * escena de Menu. 
     */
    public class MenuManager : MonoBehaviour
    {
        ///<summary>
        /// Prefab del display de categorias
        ///</summary>        
        public GameObject categoryDisplayPref;

        ///<summary>
        /// Objeto del canvas que contendra los displays de las categorias
        ///</summary>
        public RectTransform display;

        /// <summary>
        /// Crea los displays de categorias segun las categorias logicas
        /// </summary>
        /// <param name="categories"> Las categorias logicas</param>
        public void setCategories(List<Logic.GameCategory> categories)
        {
            CategoryDisplay categoryDplay;
            for (int i = 0; i < categories.Count; i++)
            {
                categoryDplay = Instantiate(categoryDisplayPref, display).GetComponent<CategoryDisplay>();
                categoryDplay.setAttributes(categories[i]);
            }
        }
    }
}