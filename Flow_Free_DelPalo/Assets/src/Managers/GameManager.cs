using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlowFree
{
    public class GameManager : MonoBehaviour
    {
        // Categorias que contienen los lotes que estan en el juego
        public Category[] categories;

        // Tema de colores que se usara en el juego
        public ColorTheme theme;

        public MenuManager menuManager;
        public LevelSelectorManager lvlSelectorManager;
        public LevelManager lvlManager;
        

        static GameManager _instance;

        //int selectedCategory;
        LevelPack selectedLevelPack;
        int selectedLevel;

        // Para testear
        public bool Testing;
        public int CategoryIndex;
        public int PackIndex;
        public int Level;

        private void Awake()
        {        
            if (Testing)
                Test(CategoryIndex, PackIndex, Level);
            if(!_instance)
            {
                //selectedLevelPack = new LevelPack();
                _instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                // Actualizamos las instancias de los managers
                _instance.menuManager = menuManager;
                _instance.lvlSelectorManager = lvlSelectorManager;
                _instance.lvlManager = lvlManager;

                // Se destruye al final del frame asi que puede ir aqui
                Destroy(gameObject);  
            }

            // Iniciamos los managers si toca
            if (_instance.menuManager)
                _instance.menuManager.setCategories(_instance.categories);

            if (_instance.lvlSelectorManager)
                _instance.lvlSelectorManager.setPack(_instance.selectedLevelPack);

            if (_instance.lvlManager)
            {
                if (_instance.selectedLevelPack.Valid)
                {
                    _instance.lvlManager.board.setFlowColors(_instance.theme.colors);
                    _instance.lvlManager.board.setMap(_instance.selectedLevelPack.Maps[_instance.selectedLevel]);
                }
            }
        }

        public static GameManager getInstance()
        {
            return _instance;   
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /**
         * Establece el lote que hemos seleccionado,
         * necesita saber desde que categoria se selecciona
         * el lote y que lote es.
         * Se llama desde el menu     
         */
        public void setLevelPack(int categoryIndex, int packIndex)
        {
            selectedLevelPack = categories[categoryIndex].packs[packIndex];
            selectedLevelPack.Parse();
            if (!selectedLevelPack.Valid)
                Debug.LogError("El lote " + selectedLevelPack.packName + " de la categoria " + categories[categoryIndex] + " no tiene el formato correcto");
            else
                Debug.Log("Lote cargado correctamente");
            
        }


        public void setSelectedLevel(int levelIndex)
        {
            selectedLevel = levelIndex;
        }

        public bool DoesNextLevelExist()
        {
            return selectedLevel + 1 < selectedLevelPack.Maps.Length;
        }
        public void nextLevel()
        {
            selectedLevel = Mathf.Clamp(selectedLevel + 1, 0, selectedLevelPack.Maps.Length -1);
        }

        public bool DoesPrevLevelExist()
        {
            return selectedLevel - 1 >= 0;
        }
        public void prevLevel()
        {
           
            selectedLevel = Mathf.Clamp(selectedLevel-1,0, selectedLevelPack.Maps.Length-1);
        }

        /**
         * Metodo para probar cualquier nivel desde la escena del board
         * configurando directamente la categoria el lote y el nivel
         */
        private void Test(int categoryIndex, int packIndex, int level)
        {
            setLevelPack(categoryIndex, packIndex);
            setSelectedLevel(level);
        }
    }
}