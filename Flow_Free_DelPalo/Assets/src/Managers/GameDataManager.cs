using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;
using System.Security.Cryptography;
using System.Text;

/*
 * Se encarga de cargar y guardar datos
 */
namespace FlowFree
{
    public class GameDataManager
    {
        public const int MAX_HINTS = 99;
        private string jsonFilePath = "/saveFile.json";
        private const string pepper = "pimienta";
        GameData _gameData;
        public GameData GetGameData()
        {
            return _gameData;
        }
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
                    pData.completedLevels = 0;
                    pData.totalLevels = pack.getTotalLevels();
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
            if (!File.Exists(Application.persistentDataPath + jsonFilePath)) return; // si no existe, nos quedamos con los datos iniciales
            using (StreamReader rstream = new StreamReader(Application.persistentDataPath + jsonFilePath))
            {
                JsonUtility.FromJsonOverwrite(rstream.ReadToEnd(), _gameData);
                if (!CheckHash())
                {
                    Debug.LogError("File has been modified");
                    Application.Quit();
                    // o reseteamos los valores
                }

            }
        }

        /*
         * Guardamos los datos del juego y el hash
         */
        public void Save()
        {
            using (StreamWriter wstream = new StreamWriter(Application.persistentDataPath + jsonFilePath))
            {
                _gameData.hash = "";
                _gameData.hash = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, true) + pepper.Substring(2, 6));
                string json = JsonUtility.ToJson(_gameData, true);
                wstream.Write(json);
            }
        }

        public void completeLevel(int catInd, int pInd, int lvlInd, int moves)
        {
            if (_gameData.categories[catInd].packs[pInd].bestMoves[lvlInd] == -1)
            {
                _gameData.categories[catInd].packs[pInd].bestMoves[lvlInd] = moves;
                _gameData.categories[catInd].packs[pInd].completedLevels++;
            }
            else
            {
                _gameData.categories[catInd].packs[pInd].bestMoves[lvlInd] = Mathf.Min(_gameData.categories[catInd].packs[pInd].bestMoves[lvlInd], moves);
            }
            // si hemos completado el nivel, desbloqueamos el siguiente
            if (lvlInd == _gameData.categories[catInd].packs[pInd].lastUnlockedLevel)
                _gameData.categories[catInd].packs[pInd].lastUnlockedLevel++;

        }


        public void unlockPack(int catInd, int pInd)
        {
            _gameData.categories[catInd].packs[pInd].blocked = false;
        }

        public void modifyHint(int value)
        {
            _gameData.hints = Mathf.Clamp(_gameData.hints + value, 0, MAX_HINTS);
            Save();
        }

        private bool CheckHash()
        {
            string ogHash = (string)_gameData.hash.Clone();
            _gameData.hash = "";
            string aux = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, true) + pepper.Substring(2, 6));
            return ogHash == aux;
        }

        private string ComputeHash(string raw)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(raw));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }       
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
        public int totalLevels;
        public int completedLevels;
        public int lastUnlockedLevel;
        public List<int> bestMoves;
    }

}