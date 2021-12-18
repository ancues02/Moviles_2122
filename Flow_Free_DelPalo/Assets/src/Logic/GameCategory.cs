using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowFree.Logic
{
    /**
     * Categoria del juego
     * Contiene packs de niveles
     */
    public class GameCategory
    {
        public string Name { get; set; }    // Nombre de la categoria
        public Color Color{ get; set; }     // Color asociado
        public Dictionary<string, GamePack> PacksDict { get; private set; } // Packs de niveles de la categoria
        public List<GamePack> PacksArray { get; private set; }  // Elementos del diccionario dispuestos en forma de lista

        /// <summary>
        /// Inicializa la categoria logica a partir de la categoria como asset
        /// </summary>
        /// <param name="category">Asset usado para rellenar los datos</param>
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
}
