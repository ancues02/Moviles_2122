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
        bool _prettyJson = true;

        // Diccionario con para identificar las categorias
        // y lotes en las estructuras serializables
        Dictionary<string, CategorySave> _savedCategories;

        /*
         * Modificamos los datos del juego segun si
         * habia datos ya guardados
         */
        void Deserialize(ref Serializable.GameData gameData)
        {
            // si no existe, nos quedamos con los datos iniciales
            using (StreamReader rstream = new StreamReader(jsonFilePath))
            {
                JsonUtility.FromJsonOverwrite(rstream.ReadToEnd(), gameData);
            }
        }
        void Serialize()
        {
            using (StreamWriter wstream = new StreamWriter(jsonFilePath))
            {
                _gameData.hash = "";
                _gameData.hash = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, _prettyJson) + pepper.Substring(2, 6));
                string json = JsonUtility.ToJson(_gameData, _prettyJson);
                wstream.Write(json);
            }
        }
        public void Load(ref Dictionary<string, Logic.GameCategory> categories, ref int hints)
        {
            Serializable.GameData gameData = new Serializable.GameData();
            _savedCategories = new Dictionary<string, CategorySave>();

            if (File.Exists(jsonFilePath))
            {
                Deserialize(ref gameData);
                if (!CheckHash(gameData))
                {
                    Debug.LogError("Save file has been modified");
                    LoadDefault(categories);
                }
                else
                {
                    _gameData = gameData;
                    hints = _gameData.hints;
                    LoadFile(categories);
                }
            }
            else
                LoadDefault(categories);
        }

        /*
         * Guardamos los datos del juego
         */
        public void Save(int hints, Dictionary<string, Logic.GameCategory> categories)
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
                        else
                        {
                            Logic.GamePack gp = categories[cName].PacksDict[pName];
                            Serializable.PackData pd = new Serializable.PackData();
                            pd.name = pName;
                            pd.blocked = gp.Blocked;
                            pd.totalLevels = gp.TotalLevels;    // este no hace falta
                            pd.completedLevels = gp.CompletedLevels;
                            pd.lastUnlockedLevel = gp.LastUnlockedLevel;
                            pd.bestMoves = gp.BestMoves;

                            _savedCategories[cName].packSaves.Add(pd.name, _gameData.categories[_savedCategories[cName].catIndex].packs.Count);
                            _gameData.categories[_savedCategories[cName].catIndex].packs.Add(pd);
                        }
                    }
                }
                else
                {
                    Serializable.CategoryData cd = new Serializable.CategoryData();
                    CategorySave cs = new CategorySave();
                    cs.packSaves = new Dictionary<string, int>();
                    cd.name = cName;
                    Serializable.PackData pd;
                    Logic.GamePack gp;
                    for(int i = 0; i < categories[cName].PacksArray.Count; i++) {
                        gp = categories[cName].PacksArray[i];
                        pd = new Serializable.PackData();

                        pd.name = gp.Name;
                        pd.blocked = gp.Blocked;
                        pd.totalLevels = gp.TotalLevels;    // este no hace falta
                        pd.completedLevels = gp.CompletedLevels;
                        pd.lastUnlockedLevel = gp.LastUnlockedLevel;
                        pd.bestMoves = gp.BestMoves;
                        cd.packs.Add(pd);
                        cs.packSaves.Add(gp.Name, i);
                    }
                    cs.catIndex = _gameData.categories.Count;
                    _gameData.categories.Add(cd);
                    _savedCategories.Add(cName, cs);
                }
            }
            _gameData.hints = hints;
            Serialize();
        }


        private void LoadFile(Dictionary<string, Logic.GameCategory> categories)
        {
            // Initialize file dict with stored data
            for(int i = 0; i < _gameData.categories.Count; i++)
            {
                CategorySave cs = new CategorySave();
                cs.catIndex = i;
                cs.packSaves = new Dictionary<string, int>();
                for (int j = 0; j < _gameData.categories[i].packs.Count; j++)
                {
                    cs.packSaves.Add(_gameData.categories[i].packs[j].name, j);
                }
                _savedCategories.Add(_gameData.categories[i].name, cs);
            }

            // Compare and overwrite categories with its stored data
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

        /// <summary>
        /// Usa de las categorias del juego para crear las 
        /// estructuras que nos permiten guardar el juego posteriormente.
        /// Este metodo se llama cuando no se ha iniciado el juego previamente
        /// o si alguien ha modificado el archivo de guardado.
        /// </summary>
        /// <param name="categories"> Diccionario de categorias que se qiuere cargar</param>
        private void LoadDefault(Dictionary<string, Logic.GameCategory> categories)
        {
            _gameData = new Serializable.GameData();
            //_gameData.categories = new List<Serializable.CategoryData>();
            List<string> catKeys = new List<string>(categories.Keys);
            for (int i = 0; i < catKeys.Count; i++)
            {
                Serializable.CategoryData cd = new Serializable.CategoryData();
                CategorySave cs = new CategorySave();
                cs.packSaves = new Dictionary<string, int>();
                cs.catIndex = i;
                List<string> packKeys = new List<string>(categories[catKeys[i]].PacksDict.Keys);
                for(int j = 0; j < packKeys.Count; j++)
                {
                    //Serializable.PackData pd = ;
                    cs.packSaves.Add(packKeys[j], j);
                    cd.packs.Add(new Serializable.PackData());
                }
                _gameData.categories.Add(cd);
                _savedCategories.Add(catKeys[i], cs);
            }
        }

        private bool CheckHash(Serializable.GameData gameData)
        {
            string ogHash = (string)gameData.hash.Clone();
            gameData.hash = "";
            string aux = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(gameData, _prettyJson) + pepper.Substring(2, 6));
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
    }
}