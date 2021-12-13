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
        public void setCategories(Category[] categories)
        {

            // Ajustamos el display segun el nº de categorias
            float newY = display.sizeDelta.y;

            // Creamos los displays de las categorias
            CategoryDisplay categoryDplay;
            for (int i = 0; i < categories.Length; i++)
            {
                categoryDplay = Instantiate(categoryDisplayPref, display).GetComponent<CategoryDisplay>();
                categoryDplay.setAttributes(categories[i], i);
                newY += categoryDplay.getActualHeight();
            }
            display.sizeDelta = new Vector2(display.sizeDelta.x, newY);
        }
    }
}