using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
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
        public void setCategories(List<Logic.GameCategory> categories)
        {
            // Creamos los displays de las categorias
            CategoryDisplay categoryDplay;
            for (int i = 0; i < categories.Count; i++)
            {
                categoryDplay = Instantiate(categoryDisplayPref, display).GetComponent<CategoryDisplay>();
                categoryDplay.setAttributes(categories[i], i);
            }
        }
    }
}