using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/*
 * Se encarga de cargar y guardar datos
 */
namespace FlowFree
{
    public class GameDataManager
    {
        struct CategorySave
        {
            public int catIndex;
            public Dictionary<string, int> packSaves;
        }

        public const int MAX_HINTS = 99;
        private const string jsonFilePath = "saveFile.json";
        private const string pepper = "pimienta";
        Serializable.GameData _gameData;     

        // Diccionario con para identificar las categorias
        // y lotes en las estructuras serializables
        Dictionary<string, CategorySave> _savedCategories;

        /*
         * Modificamos los datos del juego segun si
         * habia datos ya guardados
         */
        void Deserialize()
        {
            if (!File.Exists(jsonFilePath)) return; // si no existe, nos quedamos con los datos iniciales
            using (StreamReader rstream = new StreamReader(jsonFilePath))
            {
                JsonUtility.FromJsonOverwrite(rstream.ReadToEnd(), _gameData);
            }
        }
        void Serialize()
        {
            using (StreamWriter wstream = new StreamWriter(jsonFilePath))
            {
                _gameData.hash = "";
                _gameData.hash = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, true) + pepper.Substring(2, 6));
                string json = JsonUtility.ToJson(_gameData, true);
                wstream.Write(json);
            }
        }
        public void Load(ref Dictionary<string, Logic.GameCategory> categories)
        {
            Deserialize();
            if (!CheckHash())
            {
                Debug.LogError("Save file has been modified");
                return;
            }
            // Check hash and do things
            List<string> catKeys = new List<string>(categories.Keys);
            foreach (string cName in catKeys)
            {
                if (_savedCategories.ContainsKey(cName))
                {
                    categories[cName].Name = _gameData.categories[_savedCategories[cName].catIndex].name;
                    List<string> packKeys = new List<string>(categories[cName].PacksDict.Keys);
                    foreach (string pName in packKeys)
                    {
                        if (_savedCategories[cName].packSaves.ContainsKey(pName))
                        {
                            Serializable.PackData pd = _gameData.categories[_savedCategories[cName].catIndex].packs[_savedCategories[cName].packSaves[pName]];
                            Logic.GamePack gp = categories[cName].PacksDict[pName];

                            gp.Name = pd.name;
                            gp.Blocked = pd.blocked;
                            gp.TotalLevels = pd.totalLevels; // este no hace falta
                            gp.CompletedLevels = pd.completedLevels;
                            gp.LastUnlockedLevel = pd.lastUnlockedLevel;
                            gp.BestMoves = pd.bestMoves;
                        }
                    }
                }
            }
        }

        /*
         * Guardamos los datos del juego
         */
        public void Save(Dictionary<string, Logic.GameCategory> categories)
        {
            List<string> catKeys = new List<string>(categories.Keys);
            foreach(string cName in catKeys)
            {
                if (_savedCategories.ContainsKey(cName))
                {
                    _gameData.categories[_savedCategories[cName].catIndex].name = cName;
                    List<string> packKeys = new List<string>(categories[cName].PacksDict.Keys);
                    foreach(string pName in packKeys)
                    {
                        if (_savedCategories[cName].packSaves.ContainsKey(pName))
                        {
                            Logic.GamePack gp = categories[cName].PacksDict[pName];
                            Serializable.PackData pd = _gameData.categories[_savedCategories[cName].catIndex].packs[_savedCategories[cName].packSaves[pName]];
                            pd.name = pName;
                            pd.blocked = gp.Blocked;
                            pd.totalLevels = gp.TotalLevels;    // este no hace falta
                            pd.completedLevels = gp.CompletedLevels;
                            pd.lastUnlockedLevel = gp.LastUnlockedLevel;
                            pd.bestMoves = gp.BestMoves;
                        }
                    }
                }
            }

            Serialize();
        }

        private bool CheckHash()
        {
            string ogHash = (string)_gameData.hash.Clone();

            _gameData.hash = "";
            string aux = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, true) + pepper.Substring(2, 6));
            Debug.Log(ogHash);
            Debug.Log(aux);
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

        // Estos metodos no harian falta
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
        }

    }
}