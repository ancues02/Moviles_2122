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

        // Referencia al levelManager para 
        public LevelManager lvlManger;
        public MenuManager menuManager;

        private static GameManager _instance;


        Category selectedCategory;
        LevelPack selectedLevelPack;



        private void Awake()
        {
            
        }

        private GameManager()
        {
            // initialize your game manager here. Do not reference to GameObjects here (i.e. GameObject.Find etc.)
            // because the game manager will be created before the objects
        }


        public static GameManager getInstance()
        {
            if (!_instance)
            {
                _instance = new GameManager();
            }

            return _instance;
            
        }

        private void Start()
        {
            if(_instance)
            {
                _instance.lvlManger = lvlManger;
                _instance.menuManager = menuManager;
                Destroy(gameObject);
            }
           
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