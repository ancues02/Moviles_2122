using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlowFree
{
    /// <summary>
    /// Singleton que gestiona los recursos logicos del juego
    /// y controla el paso de informacion entre escenas.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public const int MAX_HINTS = 99;

        /// <summary>
        /// Categorias que contienen los lotes que estan en el juego
        /// </summary>
        public Category[] categories;

        /// <summary>
        /// Tema de colores que se usara en el juego
        /// </summary>
        public ColorTheme theme;

        /// <summary>
        /// Manager del menu
        /// </summary>
        public MenuManager menuManager;

        /// <summary>
        /// Manager de la seleccion de niveles
        /// </summary>
        public LevelSelectorManager lvlSelectorManager;

        /// <summary>
        /// Manager del nivel
        /// </summary>
        public LevelManager lvlManager;

        /// <summary>
        /// Manager del guardado  
        /// </summary>
        GameDataManager _dataManager;

        /// <summary>
        /// Instancia estatica para el Singleton
        /// </summary>
        static GameManager _instance;

        /// <summary>
        /// Pistas disponibles
        /// </summary>
        int hints;
  
        /// <summary>
        /// Diccionario para la gestion de categorias logicas 
        /// identificadas con su nombre.
        /// </summary>
        Dictionary<string, Logic.GameCategory> catDict;

        /// <summary>
        /// Lista de las categorias logicas. Se configura
        /// segun las categorias presentes en el diccionario
        /// </summary>
        List<Logic.GameCategory> catArray;

        /// <summary>
        /// Nivel seleccionado
        /// </summary>
        int selectedLevel;
        
        /// <summary>
        /// Categoria seleccionada
        /// </summary>
        Logic.GameCategory selectedCategory;

        /// <summary>
        /// Lote seleccionado
        /// </summary>
        Logic.GamePack selectedPack;

        /// <summary>
        /// Configuramos la instancia segun si es la primera o no. Si es la primera,
        /// creamos y configuramos las categorias logicas segun los assets de las categorias
        /// y teniendo en cuenta los datos guardados. También hacemos que nos destruya en el 
        /// cambio de escenas.
        /// Configuramos los managers de cada escena segun la escena en la que estemos
        /// </summary>
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

        /// <summary>
        /// Devuelve la instancia estatica del GameManager
        /// </summary>
        /// <returns></returns>
        public static GameManager getInstance()
        {
            return _instance;   
        }

        /// <summary>
        /// Cambia de escenas
        /// </summary>
        /// <param name="sceneName"> La siguiente escena</param>
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Establece el lote que hemos seleccionado,
        /// necesita saber desde que categoria se selecciona
        /// el lote y que lote es. Se llama desde el menu.
        /// </summary>
        /// <param name="catName"> Nombre de la categoria</param>
        /// <param name="packIndex"> El indice del lote en el array de lotes</param>
        public void SetLevelPack(string catName, int packIndex)
        {
            selectedCategory = catDict[catName];
            selectedPack = catDict[catName].PacksArray[packIndex];
        }

        /// <summary>
        /// Establece el nivel seleccionado del lote actual
        /// </summary>
        /// <param name="levelIndex"> El indice del nivel</param>
        public void SetSelectedLevel(int levelIndex)
        {
            selectedLevel = levelIndex;
        }

        /// <summary>
        /// Devuelve si existe o no el nivel posterior
        /// </summary>
        bool DoesNextLevelExist()
        {
            return _instance.selectedLevel + 1 < _instance.selectedPack.TotalLevels;
        }

        /// <summary>
        /// Cambia al siguiente nivel, si no hay mas niveles, cambia al menu
        /// </summary>
        /// <returns>Devuelve si hay o no nivel siguiente</returns>
        public bool NextLevel()
        {
            bool exist = DoesNextLevelExist();
            if (exist)
                selectedLevel++;
            return exist;
        }

        /// <summary>
        /// Devuelve si existe o no el nivel anterior
        /// </summary>
        bool DoesPrevLevelExist()
        {
            return _instance.selectedLevel - 1 >= 0;
        }

        /// <summary>
        /// Cambia al siguiente nivel, si no hay mas niveles, cambia al menu
        /// </summary>
        /// <returns>Devuelve si hay o no nivel siguiente</returns>
        public bool PrevLevel()
        {          
            bool exist = DoesPrevLevelExist();
            if (exist)
                selectedLevel--;
            return exist;
        }

        /// <summary>
        /// Utiliza una pista y guarda las pistas tras utilizarla
        /// </summary>
        /// <returns> Devuelve si hay o no pistas</returns>
        public bool UseHint()
        {
            bool hintsLeft = hints > 0;
            if (hintsLeft)
            {
                modifyHint(-1);
                _dataManager.Save(hints, catDict);
            }
            return hintsLeft;
        }

        /// <summary>
        /// Aumenta el numero de pistas
        /// </summary>
        /// <param name="numHints_"> El aumento de pistas</param>
        public void IncreaseHints(int numHints_)
        {
            modifyHint(numHints_);
            lvlManager.board.CheckHints();
        }

        /// <summary>
        /// Devuelve el numero de pistas
        /// </summary>
        public int GetHints()
        {
            return hints;
        }

        /// <summary>
        /// Completa un nivel con los movimientos correspondientes
        /// y comprueba si se habia completado antes y si es o no perfecto.
        /// Guarda el estado tras pasar el nivel.
        /// </summary>
        /// <param name="moves"> El numero de movimientos</param>
        public void LevelComplete(int moves)
        {           
            if (selectedPack.BestMoves[selectedLevel] == -1)
            {
                selectedPack.BestMoves[selectedLevel] = moves;
                selectedPack.CompletedLevels++;
            }
            else
            {
                selectedPack.BestMoves[selectedLevel] = Mathf.Min(selectedPack.BestMoves[selectedLevel], moves);
            }
            // Si hemos completado el nivel, desbloqueamos el siguiente
            if (selectedLevel == selectedPack.LastUnlockedLevel)
                selectedPack.LastUnlockedLevel++;

            // Guardamos cada vez que se pase el nivel
            _dataManager.Save(hints, catDict);
        }

        /// <summary>
        /// Desbloquea un lote
        /// </summary>
        /// <param name="catName"> el nombre de la categoria</param>
        /// <param name="pInd"> el indice del lote</param>
        public void unlockPack(string catName, int pInd)
        {
            catDict[catName].PacksArray[pInd].Blocked = false;
        }

        /// <summary>
        /// Modifica el numero de pistas
        /// </summary>
        /// <param name="value"> El numero con el que se modifican las pistas</param>
        void modifyHint(int value)
        {
            hints = Mathf.Clamp(hints + value, 0, MAX_HINTS);
        }

        /// <summary>
        /// Guardamos el estado cada vez que se cierre la aplicacion por si acaso
        /// </summary>
        private void OnApplicationQuit()
        {
            _instance._dataManager.Save(_instance.hints, _instance.catDict);
        }
    }
}