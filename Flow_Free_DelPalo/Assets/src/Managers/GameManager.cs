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
                _dataManager.Load(ref catDict);
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
                _instance.lvlSelectorManager.setPack(_instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex]);

            if (_instance.lvlManager)
            {
                if (_instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].Valid)
                {
                    _instance.lvlManager.board.SetFlowColors(_instance.theme.colors);
                    _instance.lvlManager.board.GetCameraSize();
                    Logic.Map map = _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].Maps[_instance.selectedLevel];
                    _instance.lvlManager.board.SetMap(map); 
                    //iniciar los parametros de lvlManager, basicamente poner los textos en funcion al nivel a jugar y lo que se este guardado
                    _instance.lvlManager.InitialParams(map.LevelNumber, map.Width,
                        map.Height, map.FlowNumber,!_instance.DoesPrevLevelExist(), !_instance.DoesNextLevelExist() || 
                        _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].LastUnlockedLevel == _instance.selectedLevel,
                        _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].BestMoves[_instance.selectedLevel]);
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

        public void SetSelectedLevel(int levelIndex)
        {
            selectedLevel = levelIndex;
        }

        public bool DoesNextLevelExist()
        {
            return selectedLevel + 1 < _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].Maps.Length;
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
                selectedLevel++;
            return true;
        }

        public bool DoesPrevLevelExist()
        {
            return selectedLevel - 1 >= 0;
        }

        public void prevLevel()
        {
           
            selectedLevel = Mathf.Clamp(selectedLevel-1,0, _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex].Maps.Length-1);
        }

        /**
         * Metodo para probar cualquier nivel desde la escena del board
         * configurando directamente la categoria el lote y el nivel
         */
        private void Test(int categoryIndex, int packIndex, int level)
        {
            setLevelPack(categoryIndex, packIndex);
            SetSelectedLevel(level);
        }

        public bool useHint()
        {
            modifyHint(-1);
            return hints <= 0;
        }

        public void IncreaseHints(int numHints_)
        {
            modifyHint(numHints_);
            lvlManager.board.CheckHints();
        }

        public int getHints()
        {
            return hints;
        }

        public void LevelComplete(int moves)
        {
            Logic.GamePack currPack = _instance.catArray[_instance._categoryIndex].PacksArray[_instance._packIndex];
            if (currPack.BestMoves[_instance.selectedLevel] == -1)
            {
                currPack.BestMoves[_instance._packIndex] = moves;
                currPack.CompletedLevels++;
            }
            else
            {
                currPack.BestMoves[_instance._packIndex] = Mathf.Min(currPack.BestMoves[_instance._packIndex], moves);
            }
            // Si hemos completado el nivel, desbloqueamos el siguiente
            if (_instance.selectedLevel == currPack.LastUnlockedLevel)
               currPack.LastUnlockedLevel++;

            // Guardamos cada vez que se pase el nivel
            _instance._dataManager.Save(catDict);
        }

        public void unlockPack(int catInd, int pInd)
        {
            _instance.catArray[catInd].PacksArray[pInd].Blocked = false;
        }

        // usar este para todo o cambiar los otros
        private void modifyHint(int value)
        {
            hints = Mathf.Clamp(hints + value, 0, MAX_HINTS);
        }
    }
}