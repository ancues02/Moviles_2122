using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public LevelManager lvlManger;
        public MenuManager menuManager;

        /*[System.Serializable]
        public struct Level
        {
            public string name;
            public TextAsset map;
        }*/
        public LevelPack[] levels;


        private void Awake()
        {
            Debug.Log(getInstance());
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
            }

            //levels.ToString();
            //levels[0].packName...
        }

    }
}