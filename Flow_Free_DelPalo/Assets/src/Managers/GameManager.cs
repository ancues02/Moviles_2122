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
        //public  aux;
        // Tema de colores que se usara en el juego
        public ColorTheme theme;

        public MenuManager menuManager;
        public LevelSelectorManager lvlSelectorManager;
        public LevelManager lvlManager;

        static GameManager _instance;

        //int selectedCategory;
        int _categoryIndex, _packIndex;
        int selectedLevel;
        
        // Gestion del guardado 
        GameDataManager _dataManager;

        public int numHints { get; private set; } = 3;

        private void Awake()
        {
            #if UNITY_EDITOR
                numHints=1;
            #endif
            if(!_instance)
            {
                //selectedLevelPack = new LevelPack();
                _dataManager = new GameDataManager();
                _dataManager.ParseAll(categories);
                _dataManager.Load();
                /*_dataManager.initCategories();
                _dataManager.load();*/
                //_dataManager.Load();
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
                _instance.menuManager.setCategories(_instance.categories, _instance._dataManager.GetGameData().categories.ToArray());

            if (_instance.lvlSelectorManager)
                _instance.lvlSelectorManager.setPack(
                    _instance.categories[_instance._categoryIndex].packs[_instance._packIndex], 
                    _instance._dataManager.GetGameData().categories[_instance._categoryIndex].packs[_instance._packIndex]
                );

            if (_instance.lvlManager)
            {
                if (_instance.categories[_instance._categoryIndex].packs[_instance._packIndex].Valid)
                {
                    _instance.lvlManager.board.SetFlowColors(_instance.theme.colors);
                    _instance.lvlManager.board.GetCameraSize();
                    Logic.Map map = _instance.categories[_instance._categoryIndex].packs[_instance._packIndex].Maps[_instance.selectedLevel];
                    _instance.lvlManager.board.SetMap(map); 
                    _instance.lvlManager.InitialParams(map.LevelNumber, map.Width,
                        map.Height, !_instance.DoesPrevLevelExist(), !_instance.DoesNextLevelExist());
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
            _categoryIndex = categoryIndex;
            _packIndex = packIndex; 
        }

        public void setSelectedLevel(int levelIndex)
        {
            selectedLevel = levelIndex;
        }

        public bool DoesNextLevelExist()
        {
            return selectedLevel + 1 < _instance.categories[_instance._categoryIndex].packs[_instance._packIndex].Maps.Length;
        }

        public void nextLevel()
        {
            selectedLevel = Mathf.Clamp(selectedLevel + 1, 0, _instance.categories[_instance._categoryIndex].packs[_instance._packIndex].Maps.Length -1);
        }

        public bool DoesPrevLevelExist()
        {
            return selectedLevel - 1 >= 0;
        }

        public void prevLevel()
        {
           
            selectedLevel = Mathf.Clamp(selectedLevel-1,0, _instance.categories[_instance._categoryIndex].packs[_instance._packIndex].Maps.Length-1);
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

        public bool useHint()
        {
            bool ret = false;

            if(numHints > 0)
            {
                ret = true;
                numHints--;
                // TODO Reescribe n�mero de hints en archivo o algo
            }
            return ret;
        }

        public void IncreaseHints(int numHints_)
        {
            numHints += numHints_;
            lvlManager.board.CheckHints();
        }

        public void LevelComplete(int moves)
        {
            _dataManager.completeLevel(_instance._categoryIndex, _instance._packIndex, _instance.selectedLevel, moves);
        }

        private void OnApplicationQuit()
        {
            _dataManager.Save();
        }
    }
}