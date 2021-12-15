using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// boceto de lo que guardariamos
struct CategoryData
{
    bool blocked;
    PackData[] packs;
}
struct PackData
{
    LevelData[] levels;
}
[System.Serializable]
struct LevelData
{
    public bool levelBlocked;
    public int bestMoves;
}

public class GameData_
{
    public int hintNum;
    CategoryData[] categories;
    public string hash;
}

// Esto es para probar nada mas
public class GameData : MonoBehaviour
{
    public string jsonFilePath;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        writeFile(); readFile();
    }

    void writeFile()
    {
        using (StreamWriter wstream = new StreamWriter(jsonFilePath))
        {
            LevelData levelData = new LevelData();
            levelData.levelBlocked = true;
            levelData.bestMoves = 5;
            string json = JsonUtility.ToJson(levelData, true);
            wstream.Write(json);
        }
    }
    void readFile()
    {
        using (StreamReader rstream = new StreamReader(jsonFilePath))
        {
            string aux = rstream.ReadToEnd();
            LevelData levelData = JsonUtility.FromJson<LevelData>(aux);
            Debug.Log(levelData.bestMoves + " " + levelData.levelBlocked);
        }
    }
}