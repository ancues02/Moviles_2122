﻿using System;
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
        public Dictionary<string, GamePack> PacksDict { get; private set; }
        public List<GamePack> PacksArray { get; private set; }

        public void Init(Category category)
        {
            PacksDict = new Dictionary<string, GamePack>();
            Name = category.categoryName;
            Color = category.categoryColor;
            GamePack pack;
            foreach(LevelPack levelPack in category.packs)
            {
                pack = new GamePack();
                pack.Init(levelPack);
                PacksDict.Add(levelPack.packName, pack);
            }
            PacksArray = new List<GamePack>(PacksDict.Values);
        }
    }

    public class GamePack
    {
        public string Name { get; set; }
        public bool Blocked { get; set; }
        public int LastUnlockedLevel { get; set; }
        public Page[] Pages { get; set; }
        public bool Valid { get; private set; }
        public int TotalLevels { get; set; }
        public int CompletedLevels { get; set; }
        public List<int> BestMoves { get; set; }

        public Map[] Maps { get; private set; }


        public void Init(LevelPack levelPack)
        {
            Name = levelPack.packName;
            Blocked = levelPack.blocked;
            LastUnlockedLevel = levelPack.blockedLevelIndex;
            Pages = levelPack.pages;
            TotalLevels = levelPack.pages.Length * Page.LEVELS_PER_PAGE;
            CompletedLevels = 0;
            BestMoves = new List<int>();
            for (int i = 0; i < TotalLevels; i++)
                BestMoves.Add(-1);
            Parse(levelPack.maps);
        }

        // Parsea todos los niveles de un lote
        // a partir del archivo en .txt crudo
        void Parse(TextAsset mapRaw)
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
        }
    }
}
