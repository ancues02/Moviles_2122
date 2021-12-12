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

        public int categoryTest;
        public int packTest;

        CategoryDisplay[] categoryDisplays;
        public void setCategories(Category[] categories)
        {
            GameObject categoryObj;
            CategoryDisplay categoryDplay;
            foreach(Category cat in categories)
            {
                categoryObj = Instantiate(CategoryDisplayPref, Vector2.zero, Quaternion.identity, Display.transform);
                categoryDplay = categoryObj.GetComponent<CategoryDisplay>();
                categoryDplay.setAttributes(cat);
            }
        }
        private void Update()
        {
            // Gestiona la seleccion de categoria y pack dentro de la categoria
            // ademas del scroll vertical

            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager gm = GameManager.getInstance();
                gm.setLevelPack(categoryTest, packTest);
                gm.ChangeScene("LevelSelectionTest");
            }
        }
    }
}