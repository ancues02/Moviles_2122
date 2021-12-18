using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowFree.Logic
{
    /**
     * Paquete de niveles del juego
     */
    public class GamePack
    {
        public string Name { get; set; }    // Nombre del paquete
        public bool Blocked { get; set; }   // Si el paquete esta bloqueado o no
        public int LastUnlockedLevel { get; set; }  // Indice del ultimo nivel desbloqueado en el paquete
        public Page[] Pages { get; set; }   // Paginas en las que se dividen los paquetes
        public bool Valid { get; private set; } // Si se ha creado ocrrectamente
        public int TotalLevels { get; set; }    // Numero total de niveles
        public int CompletedLevels { get; set; }    // Numero total de niveles completdos
        public List<int> BestMoves { get; set; }    // Lista con los mejores movimientos de cada nivel

        public Map[] Maps { get; private set; } // Lista de los mapas correspondientes a los niveles

        /// <summary>
        /// Inicializa el paquete logico a partir del paquete como asset
        /// </summary>
        /// <param name="levelPack">Paquete usasdo para rellenar los datos</param>
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

        /// <summary>
        /// Parsea todos los niveles del lote y los asigna a la lista de mapas
        /// </summary>
        /// <param name="mapRaw">Archivo de texto con los datos de los niveles</param>
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
