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
        public LevelManager lvlManger;
        

        static GameManager _instance;
        Category selectedCategory;
        LevelPack selectedLevelPack;

        private void Awake()
        {
            if (_instance)
            {
                _instance.lvlManger = lvlManger;
                _instance.lvlSelectorManager = lvlSelectorManager;
                _instance.menuManager = menuManager;
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject); 
            }
        }

        public static GameManager getInstance()
        {
            return _instance;   
        }

        public void ChangeScene(string sceneName)
        {
            Scene sceneToChange = SceneManager.GetSceneByName(sceneName);
            if (sceneToChange.IsValid())
                SceneManager.SetActiveScene(sceneToChange);
        }

        public LevelPack getSelectedPack()
        {
            return selectedLevelPack;
        }

        public Logic.Map getLevel(int levelIndex)
        {
            return selectedLevelPack.Maps[levelIndex];
        }

    }
}