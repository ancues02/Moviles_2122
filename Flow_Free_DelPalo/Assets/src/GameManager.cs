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

        // 
        private void Awake()
        {        
            if(!_instance)
            {
                //selectedLevelPack = new LevelPack();
                _instance = this;
                DontDestroyOnLoad(gameObject); 
            }
        }

        private void Start()
        {
            if (_instance != this)
            {
                // Dejamos en la instancia el MenuManager actual
                _instance.menuManager = menuManager;

                // Dejamos en la instancia el LvlSelectorManager actual
                if (lvlSelectorManager)
                { 
                    lvlSelectorManager.setPack(selectedLevelPack);
                }
                _instance.lvlSelectorManager = lvlSelectorManager;

                // Dejamos en la instancia el LvlManager y le damos el nivel actual al tablero
                if (lvlManager)
                {
                    if (_instance.selectedLevelPack.Valid)
                    {
                        lvlManager.board.setFlowColors(theme.colors);
                        lvlManager.board.setMapTest(_instance.selectedLevelPack.Maps[_instance.selectedLevel]);
                    }
                }
                _instance.lvlManager = lvlManager;
                
                Destroy(gameObject);
            }
        }

        public static GameManager getInstance()
        {
            return _instance;   
        }

        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            /*Scene sceneToChange = SceneManager.GetSceneByName(sceneName);

            Scene scenes = SceneManager.GetSceneByName("GameBoard");
            
            if (sceneToChange.IsValid())
                SceneManager.SetActiveScene(sceneToChange);*/
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
            selectedLevel++;
        }

        public bool DoesPrevLevelExist()
        {
            return selectedLevel - 1 >= 0;
        }
        public void prevLevel()
        {
            selectedLevel--;
        }
    }
}