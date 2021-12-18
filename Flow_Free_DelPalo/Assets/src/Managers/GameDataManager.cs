using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace FlowFree
{
    /// <summary>
    /// Clase para gestionar el guardado y la recuperacion
    /// de los datos del juego.
    /// </summary>
    public class GameDataManager
    {
        /// <summary>
        ///  Estructura auxiliar para identificar a las categorias
        ///  y sus posicion en el array en las estructuras de serializacion
        /// </summary>
        struct CategorySave
        {
            public int catIndex;
            public Dictionary<string, int> packSaves;
        }
        
        /// <summary>
        /// Ruta relativa del archivo de guardado
        /// </summary>
        private const string jsonFilePath = "/saveFile.json";

        /// <summary>
        /// String arbitrario para modificar aniadir "pimienta"
        /// en las comprobaciones de integridad con el hash
        /// </summary>
        private const string pepper = "pimienta";

        /// <summary>
        /// Datos del juego guardados
        /// </summary>
        Serializable.GameData _gameData;

        /// <summary>
        /// Variable para debuggear el guardado en json
        /// </summary>
        bool _prettyJson = false;

        ///<summary>
        /// Diccionario con para identificar las categorias
        /// y lotes en las estructuras serializables
        ///</summary>
        Dictionary<string, CategorySave> _savedCategories;

        /// <summary>
        /// Deserializa la estructura del estado 
        /// del juego del archivo del guardado
        /// </summary>
        void Deserialize(ref Serializable.GameData gameData)
        {
            // si no existe, nos quedamos con los datos iniciales
            using (StreamReader rstream = new StreamReader(Application.persistentDataPath + jsonFilePath))
            {
                JsonUtility.FromJsonOverwrite(rstream.ReadToEnd(), gameData);
            }
        }

        /// <summary>
        /// Serializa la estructura del estado 
        /// del juego y la guarda en el archivo
        /// </summary>
        void Serialize()
        {
            using (StreamWriter wstream = new StreamWriter(Application.persistentDataPath + jsonFilePath))
            {
                _gameData.hash = "";
                _gameData.hash = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(_gameData, _prettyJson) + pepper.Substring(2, 6));
                string json = JsonUtility.ToJson(_gameData, _prettyJson);
                wstream.Write(json);
            }
        }

         /// <summary>
         /// Cargamos el estado del juego si estaba guardado.
         /// Si el archivo de guardado ha sido modificado
         /// por algun medio externo, se borra todo el archivo
         /// de guardado y se empieza el juego de cero
         /// </summary>
         /// <param name="categories">Categorias a guardar</param>
         /// <param name="hints">Pistas a guardar</param>
        public void Load(ref Dictionary<string, Logic.GameCategory> categories, ref int hints)
        {
            Serializable.GameData gameData = new Serializable.GameData();
            _savedCategories = new Dictionary<string, CategorySave>();

            if (File.Exists(Application.persistentDataPath + jsonFilePath))
            {
                Deserialize(ref gameData);
                if (!CheckHash(gameData))
                {
                    Debug.LogError("Save file has been modified");
                    File.Delete(Application.persistentDataPath + jsonFilePath);
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

         /// <summary>
         /// Guarda el estado del juego.
         /// Serializa las pistas y los diccionarios que se utilizan
         /// en la logica del juego en las estructuras de serializacion
         /// y se guardan en un archivo .json
         /// </summary>
         /// <param name="hints">Pistas a guardar</param>
         /// <param name="categories">Categorias a guardar</param>
        public void Save(int hints, Dictionary<string, Logic.GameCategory> categories)
        {
            List<string> catKeys = new List<string>(categories.Keys);
            foreach(string cName in catKeys)
            {
                // Sobreescribimos las categorias de las que ya teniamos datos guardados
                if (_savedCategories.ContainsKey(cName))
                {
                    _gameData.categories[_savedCategories[cName].catIndex].name = cName;
                    List<string> packKeys = new List<string>(categories[cName].PacksDict.Keys);
                    foreach(string pName in packKeys)
                    {
                        // Sobreescribimos los lotes de los que ya teniamos datos guardados
                        if (_savedCategories[cName].packSaves.ContainsKey(pName))
                        {
                            Logic.GamePack gp = categories[cName].PacksDict[pName];
                            Serializable.PackData pd = _gameData.categories[_savedCategories[cName].catIndex].packs[_savedCategories[cName].packSaves[pName]];
                            pd.name = pName;
                            pd.blocked = gp.Blocked;
                            pd.completedLevels = gp.CompletedLevels;
                            pd.lastUnlockedLevel = gp.LastUnlockedLevel;
                            pd.bestMoves = gp.BestMoves;
                        }
                        // Si tenemos un lote que no habiamos guardado, lo guardamos
                        else
                        {
                            Logic.GamePack gp = categories[cName].PacksDict[pName];
                            Serializable.PackData pd = new Serializable.PackData();
                            pd.name = pName;
                            pd.blocked = gp.Blocked;
                            pd.completedLevels = gp.CompletedLevels;
                            pd.lastUnlockedLevel = gp.LastUnlockedLevel;
                            pd.bestMoves = gp.BestMoves;

                            _savedCategories[cName].packSaves.Add(pd.name, _gameData.categories[_savedCategories[cName].catIndex].packs.Count);
                            _gameData.categories[_savedCategories[cName].catIndex].packs.Add(pd);
                        }
                    }
                }
                // Si tenemos una categoria que no habiamos guardado, la guardamos
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

        /// <summary>
        /// Compara las categorias del juego con los datos guardados.
        /// Si habia datos guardados relativos al contenido actual del juego, se sobreescriben.
        /// </summary>
        /// <param name="categories">Diccionario de categorias que se quiere cargar</param>
        private void LoadFile(Dictionary<string, Logic.GameCategory> categories)
        {
            // Inicializa el diccionario de las categorias con los datos guardados
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

            // Sobreescribe los datos del juego con lo que estuviera guardado
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
                            gp.CompletedLevels = pd.completedLevels;
                            gp.LastUnlockedLevel = pd.lastUnlockedLevel;
                            gp.BestMoves = pd.bestMoves;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Usa de las categorias del juego para inicializar las 
        /// estructuras que nos permiten guardar el juego posteriormente.
        /// Este metodo se llama cuando no se ha iniciado el juego previamente
        /// o si alguien ha modificado el archivo de guardado.
        /// </summary>
        /// <param name="categories"> Diccionario de categorias que se quiere cargar</param>
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

        /// <summary>
        /// Comprueba la integridad de los datos
        /// usando el hash y la "pimienta"
        /// </summary>
        /// <param name="gameData"> Los datos que se comprueban </param>
        /// <returns></returns>
        private bool CheckHash(Serializable.GameData gameData)
        {
            string ogHash = (string)gameData.hash.Clone();
            gameData.hash = "";
            string aux = ComputeHash(pepper.Substring(0, 2) + JsonUtility.ToJson(gameData, _prettyJson) + pepper.Substring(2, 6));
            Debug.Log(ogHash);
            Debug.Log(aux);
            return ogHash == aux;
        }

        /// <summary>
        /// Calcula el hash de un string (el archivo de texto)
        /// </summary>
        /// <param name="raw"> El texto con el que calcular le hash</param>
        /// <returns> El hash generado a partir del texto </returns>
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