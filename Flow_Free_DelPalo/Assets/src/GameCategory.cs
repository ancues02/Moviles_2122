using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowFree.Logic
{
    public class GameCategory
    {
        public string Name { get; set; }
        public Color Color{ get; set; }
        public Dictionary<string, GamePack> PacksDict { get; }
        public List<GamePack> PacksArray { get; private set; }

        public void Init(Category category)
        {
            // Despues de haber inicializado el diccionario con las categorias
            PacksArray = new List<GamePack>(PacksDict.Values);
        }

        public void InitSaved(CategoryData category)
        {

        }
    }

    public class GamePack
    {
        public string Name { get; set; }
        public bool Blocked { get; set; }
        public int BlockedLevelIndex { get; set; }
        public Page[] Pages { get; set; }
        public bool Valid { get; private set; }
        public int TotalLevels { get; private set; }
        public int CompletedLevels { get; set; }
        public List<int> BestMoves { get; set; }

        public Map[] Maps { get; private set; }

        // Parsea todos los niveles de un lote
        // a partir del archivo en .txt crudo
        public void Parse(TextAsset mapRaw)
        {
            Valid = true;
            string[] mapsText = mapRaw.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            Maps = new Logic.Map[mapsText.Length];

            int i = 0;
            while (i < mapsText.Length && Valid)
            {
                Maps[i] = new Logic.Map();
                Valid = Maps[i].Parse(mapsText[i]);
                i++;
            }
            TotalLevels = Pages.Length * Page.LEVELS_PER_PAGE;
        }
    }
}
