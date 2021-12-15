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
namespace FlowFree
{


    // Esto es para probar nada mas
    public class GameDataManager : MonoBehaviour
    {
        public string jsonFilePath;
        Dictionary<string, CategoryData> _data;
        GameData _gameData;
        /*
         * Creamos un diccionario de guardado por defecto,
         * con las categorias que tuviera el GM, luego leemos el
         * archivo y cambiamos en este diccionario
         */
        private void Load(List<Category> categories)
        {
            // inicializar el _data antes de esto
            // Si hay datos guardados, actualizamos las categorias con ellos
            if (File.Exists(jsonFilePath))
            {
                JSONNode json = JSON.Parse(jsonFilePath);
                getDataFromJson(json);
                
            }
            Debug.Log(Application.persistentDataPath);
            writeFile(3); readFile();
        }

        void Save(List<Category> categories)
        {
            
        }

        void getDataFromJson(JSONNode jnode)
        {
            // TODO: Check hash
            //hints = jnode["hints"].AsInt;
            //List<Category> c = new List<Category>();
            JSONArray cats = (JSONArray)jnode["categories"];
            for(int i = 0; i < cats.Count; i++)
            {
                // Si existe la categoria que teniamos guardada
                if (_data.ContainsKey(cats[i]["name"]))
                {
                    JSONArray jpacks = (JSONArray)cats[i]["packs"];
                    for (int j = 0; j < jpacks.Count; j++)
                    {
                        // Si la categoria tiene el pack teniamos guardado
                        if (_data[cats[i]["name"]].packs.ContainsKey(jpacks[j]["name"]))
                        {
                            //Configuramos el pack segun lo guardado
                            _data[cats[i]["name"]].packs[jpacks[j]["name"]].blocked = jpacks[j]["blocked"].AsBool;
                            LevelData[] currentLevels = _data[cats[i]["name"]].packs[jpacks[j]["name"]].levels;
                            JSONArray jlevels = (JSONArray)jpacks[j]["levels"];
                            for (int m = 0; m < jlevels.Count; m++)
                            {
                                currentLevels[jlevels[m]["level"].AsInt] = JsonUtility.FromJson<LevelData>(jlevels[m].ToString());
                                /*currentLevels[jlevels[m]["level"].AsInt].levelBlocked = jlevels[m]["blocked"].AsBool;
                                currentLevels[jlevels[m]["level"].AsInt].bestMoves = jlevels[m]["best"].AsInt;*/
                            }
                        }
                    }
                }
            }

        }
        private void Start()
        {
            //writeFile(2); readFile();
        }
        void writeFile(int num)
        {
            using (StreamWriter wstream = new StreamWriter(jsonFilePath))
            {

                /*LevelData[] levelData = new LevelData[num];
                for(int i = 0; i < num; i++)
                {
                    levelData[i] = new LevelData();
                    levelData[i].levelBlocked = i % 2 == 0;
                    levelData[i].bestMoves = i;
                }*/
               /* string json = JsonUtility.ToJson(, true);
                wstream.Write(json);*/
            }
        }
        void readFile()
        {
            using (StreamReader rstream = new StreamReader(jsonFilePath))
            {
                string aux = rstream.ReadToEnd();
                //Category levelData = JsonUtility.FromJson<Category>(aux);
                /*for (int i = 0; i < levelData.Length; i++)
                {
                    Debug.Log(levelData[i].bestMoves + " " + levelData[i].levelBlocked);
                }*/
            }
        }
    }




    public class CategoryData
    {
        public Dictionary<string, PackData> packs;
    }
    public class PackData
    {
        public bool blocked;
        public LevelData[] levels;
    }
    [System.Serializable]
    public class LevelData
    {
        public bool levelBlocked;
        public int bestMoves;
    }

    public class GameData
    {
        public string hash;
        public int hintNum;
        CategoryData[] categories;
    }


}