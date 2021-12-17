using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlowFree
{
    public class GameManager : MonoBehaviour
    {
        public const int MAX_HINTS = 99;

        // Categorias que contienen los lotes que estan en el juego
        public Category[] categories;

        // Tema de colores que se usara en el juego
        public ColorTheme theme;

        public MenuManager menuManager;
        public LevelSelectorManager lvlSelectorManager;
        public LevelManager lvlManager;

        static GameManager _instance;

        int hints;

        
        Dictionary<string, Logic.GameCategory> catDict;
        List<Logic.GameCategory> catArray;
        int _categoryIndex, _packIndex, selectedLevel;
        
        Logic.GameCategory selectedCategory;
        Logic.GamePack selectedPack;
        Dictionary<string, int> catPos;


        // Gestion del guardado 
        GameDataManager _dataManager;

        private void Awake()
        {         
            if(!_instance)
            {
                hints = 3;
                _dataManager = new GameDataManager();
                catDict = new Dictionary<string, Logic.GameCategory>();
                Logic.GameCategory gameCategory;
                foreach(Category cat in categories)
                {
                    gameCategory = new Logic.GameCategory();
                    gameCategory.Init(cat);
                    catDict.Add(cat.categoryName, gameCategory);
                }     
                _dataManager.Load(ref catDict, ref hints);
                catArray = new List<Logic.GameCategory>(catDict.Values);

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
                _instance.menuManager.setCategories(_instance.catArray);

            if (_instance.lvlSelectorManager)
                _instance.lvlSelectorManager.setPack(_instance.selectedPack, _instance.selectedCategory.Color);

            if (_instance.lvlManager)
            {
                if (_instance.selectedPack.Valid)
                {
                    _instance.lvlManager.board.SetFlowColors(_instance.theme.colors);
                    _instance.lvlManager.board.GetCameraSize();
                    Logic.Map map = _instance.selectedPack.Maps[_instance.selectedLevel];
                    _instance.lvlManager.board.SetMap(map); 
                    //iniciar los parametros de lvlManager, basicamente poner los textos en funcion al nivel a jugar y lo que se este guardado
                    _instance.lvlManager.InitialParams(map.LevelNumber, map.Width,
                        map.Height, map.FlowNumber,!_instance.DoesPrevLevelExist(), !_instance.DoesNextLevelExist() || 
                        _instance.selectedPack.LastUnlockedLevel == _instance.selectedLevel,
                        _instance.selectedPack.BestMoves[_instance.selectedLevel],
                        _instance.selectedCategory.Color);
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
        public void setLevelPack(string catName, int packIndex)
        {
            _instance.selectedCategory = _instance.catDict[catName];
            _instance.selectedPack = _instance.catDict[catName].PacksArray[packIndex];
        }

        public void SetSelectedLevel(int levelIndex)
        {
            _instance.selectedLevel = levelIndex;
        }

        public bool DoesNextLevelExist()
        {
            return _instance.selectedLevel + 1 < _instance.selectedPack.TotalLevels;
        }

        /// <summary>
        /// Cambia al siguiente nivel, si no hay mas niveles, cambia al menu
        /// </summary>
        /// <returns>Devuelve si hay o no nivel siguiente</returns>
        public bool NextLevel()
        {
            if(!_instance.DoesNextLevelExist())
            {
                ChangeScene("Menu");
                return false;
            }
            else
                _instance.selectedLevel++;
            return true;
        }

        public bool DoesPrevLevelExist()
        {
            return _instance.selectedLevel - 1 >= 0;
        }

        public void PrevLevel()
        {          
            _instance.selectedLevel = Mathf.Clamp(_instance.selectedLevel-1,0, _instance.selectedPack.TotalLevels - 1);
        }

        public bool UseHint()
        {
            modifyHint(-1);
            _dataManager.Save(_instance.hints, _instance.catDict);
            return _instance.hints > 0;
        }

        public void IncreaseHints(int numHints_)
        {
            modifyHint(numHints_);
            _instance.lvlManager.board.CheckHints();
        }

        public int GetHints()
        {
            return _instance.hints;
        }

        public void LevelComplete(int moves)
        {           
            if (_instance.selectedPack.BestMoves[_instance.selectedLevel] == -1)
            {
                _instance.selectedPack.BestMoves[_instance._packIndex] = moves;
                _instance.selectedPack.CompletedLevels++;
            }
            else
            {
                _instance.selectedPack.BestMoves[_instance._packIndex] = Mathf.Min(_instance.selectedPack.BestMoves[_instance._packIndex], moves);
            }
            // Si hemos completado el nivel, desbloqueamos el siguiente
            if (_instance.selectedLevel == _instance.selectedPack.LastUnlockedLevel)
                _instance.selectedPack.LastUnlockedLevel++;

            // Guardamos cada vez que se pase el nivel
            _instance._dataManager.Save(_instance.hints, _instance.catDict);
        }

        public void unlockPack(int catInd, int pInd)
        {
            _instance.catArray[catInd].PacksArray[pInd].Blocked = false;
        }

        // usar este para todo o cambiar los otros
        private void modifyHint(int value)
        {
            _instance.hints = Mathf.Clamp(_instance.hints + value, 0, MAX_HINTS);
        }

        private void OnApplicationQuit()
        {
            _instance._dataManager.Save(_instance.hints, _instance.catDict);
        }
    }
}