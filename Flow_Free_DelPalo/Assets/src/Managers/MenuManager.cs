using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class MenuManager : MonoBehaviour
    {
        // Prefab del display de categorias
        public GameObject CategoryDisplayPref;

        // Objeto del canvas que contendra los displays de las categorias
        public GameObject Display;
        public void setCategories(Category[] categories)
        {
            GameObject categoryObj;
            CategoryDisplay categoryDplay;
            for (int i = 0; i < categories.Length; i++)
            {
                categoryObj = Instantiate(CategoryDisplayPref, Display.transform);
                categoryDplay = categoryObj.GetComponent<CategoryDisplay>();
                categoryDplay.setAttributes(categories[i], i);
            }
        }
    }
}