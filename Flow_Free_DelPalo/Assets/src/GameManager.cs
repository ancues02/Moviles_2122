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
            if (_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
            
        }

        private void Start()
        {
            if(_instance != null)
            {
                _instance.lvlManger = lvlManger;
                _instance.menuManager = menuManager;
            }
        }

    }
}