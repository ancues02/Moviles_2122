using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    public string jsonFilePath;
    GameDataSimple gameData;

    [System.Serializable]
    public class GameDataSimple
    {
        public string hash;
        public int hintNum;
        public LevelDataArray levelsArray;
    }

    [System.Serializable]
    public class LevelDataSimple
    {
        public string category;
        public string pack;
        public int level;
        public bool blocked;
        public int best;
    }
    [System.Serializable]
    public class LevelDataArray // pack
    {
        public LevelDataSimple[] levels;
    }

    private void Start()
    {
        /*gameData = new GameDataSimple();
        /*gameData.hash = "eadkfhheuh4435643";
        gameData.hintNum = 3;
        gameData.levelsArray = new LevelDataArray();
        gameData.levelsArray.levels = new LevelDataSimple[2];
        gameData.levelsArray.levels[0] = new LevelDataSimple();
        gameData.levelsArray.levels[0].category = "categoria0";
        gameData.levelsArray.levels[0].pack = "pack0";
        gameData.levelsArray.levels[0].level = 0;
        gameData.levelsArray.levels[0].blocked = true;
        gameData.levelsArray.levels[0].best = 6;

        gameData.levelsArray.levels[1] = new LevelDataSimple();
        gameData.levelsArray.levels[1].category = "categoria1";
        gameData.levelsArray.levels[1].pack = "pack5";
        gameData.levelsArray.levels[1].level = 1;
        gameData.levelsArray.levels[1].blocked = false;
        gameData.levelsArray.levels[1].best = -1;

        writeFile();*/        
        readFile();
    }

    void writeFile()
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
             string json = JsonUtility.ToJson(gameData, true);
             wstream.Write(json);
        }
    }
    void readFile()
    {
        using (StreamReader rstream = new StreamReader(jsonFilePath))
        {
            string aux = rstream.ReadToEnd();
            gameData = null;
            gameData = JsonUtility.FromJson<GameDataSimple>(aux);
            /*for (int i = 0; i < levelData.Length; i++)
            {
                Debug.Log(levelData[i].bestMoves + " " + levelData[i].levelBlocked);
            }*/
        }
    }
}
