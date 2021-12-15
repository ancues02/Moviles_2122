using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;
/**
 *  -------------------SAVED DATA JSON FORMAT--------------------------------
 *  {
 *      hash: "slajfljefja39u872895",
 *      hints: 3,
 *      categories: 
 *      [
 *          {
 *              name: "Intro"
 *              packs:
 *              [
 *                  {
 *                      name: "Classic"
 *                      blocked: false,
 *                      levels:
 *                      [
 *                          {
 *                              level: 0,
 *                              blocked: true,
 *                              best: 6
 *                          },
 *                          (...)
 *                      ]
 *                  },
 *                  (...)
 *              ]
 *          },
 *          (...)
 *      ]
 *  
 *  }
 *  
 *  {
 *      hash: "slajfljefja39u872895"
 *      hints: 4
 *      unlockedPacks:
 *      [
 *          {
 *              category: "Extreme"
 *              pack: 
 *          }
 *      ]
 *      levels:
 *      [
 *          {
 *              category: "Intro",
 *              pack: "Classic",
 *              level: 54
 *              blocked: false,
 *              best: 5
 *          },
 *          (...)
 *      ]
 *  }
 */

/*
    Se encarga de cargar y guardar datos.

 */
namespace FlowFree
{
    public class GameDataManager
    {
        public const int MAX_HINTS = 99;
        private string jsonFilePath = "savedData.json";
        GameData _gameData;
        /*
         * Creamos un diccionario de guardado por defecto,
         * con las categorias que tuviera el GM, luego leemos el
         * archivo y cambiamos en este diccionario
         */

        // Hace el parse de todos los niveles y configura
        // los datos iniciales del juego.
        public void ParseAll(Category[] categories)
        {
            _gameData = new GameData();
            _gameData.hash = "";
            _gameData.hints = 3;
            foreach(Category cat in categories)
            {
                CategoryData cSave = new CategoryData(cat.categoryName);
                _gameData.categories.Add(cSave);
                foreach(LevelPack pack in cat.packs)
                {
                    pack.Parse();
                    PackData pData = new PackData();
                    pData.name = pack.packName;
                    pData.blocked = pack.blocked;
                    pData.lastUnlockedLevel = pack.blockedLevelIndex;
                    pData.bestMoves = new List<int>(pack.getTotalLevels());
                    for(int i = 0; i < pack.getTotalLevels(); i++)
                        pData.bestMoves.Add(-1);

                    cSave.packs.Add(pData);
                }
            }
        }

        /*
         * Modificamos los datos del juego segun si
         * habia datos ya guardados
         */
        public void Load()
        {
            if (!File.Exists(jsonFilePath)) return; // si no existe, nos quedamos con los datos iniciales
            using (StreamReader rstream = new StreamReader(jsonFilePath))
            {
                GameData savedData = JsonUtility.FromJson<GameData>(rstream.ReadToEnd());
                if (CheckHash(savedData))
                {
                    _gameData = savedData;  // Comprobar si ha habido algun cambio extra
                }
            }
        }

        /*
         * Guardamos el juego.
         */
        public void Save()
        {
            using (StreamWriter wstream = new StreamWriter(jsonFilePath))
            {
                string json = JsonUtility.ToJson(_gameData);
                wstream.Write(json);
            }
        }

        public void completeLevel(int catInd, int pInd, int lvlInd, int moves)
        {
            _gameData.categories[catInd].packs[pInd].bestMoves[lvlInd] = Mathf.Min(_gameData.categories[catInd].packs[pInd].bestMoves[lvlInd], moves);
        }

        public void modifyHint(int value)
        {
            _gameData.hints = Mathf.Clamp(_gameData.hints, 0, MAX_HINTS); 
        }

        private bool CheckHash(GameData data)
        {
            return true;
        }

        private string ComputeHash(string file)
        {
            return "";
        }
    }


    [System.Serializable]
    public class GameData
    {
        public string hash;
        public int hints;
        public List<CategoryData> categories;

        public GameData()
        {
            categories = new List<CategoryData>();
        }
    }

    [System.Serializable]
    public class CategoryData
    {
        public string name;
        public List<PackData> packs;

        public CategoryData(string name_)
        {
            name = name_;
            packs = new List<PackData>();
        }
    }

    [System.Serializable]
    public class PackData
    {
        public string name;
        public bool blocked;
        public int lastUnlockedLevel;
        public List<int> bestMoves;
    }

}