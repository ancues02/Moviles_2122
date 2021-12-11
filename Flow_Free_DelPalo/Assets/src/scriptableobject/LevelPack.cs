using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowFree
{


    /**
     * Scriptable Object para gestionar los lotes como datos.
     * Contiene el nombre del lote y el fichero de texto
     * que conforma el lote.
     */
    [CreateAssetMenu(fileName = "Levelpack",
        menuName = "Flow/Level pack", order = 1)]
    public class LevelPack : ScriptableObject
    {
        [Tooltip("Nombre del lote")]
        public string packName;

        [Tooltip("Fichero con los niveles")]
        public TextAsset maps;

        // TODO: Juntar estos dos en una estructura
        [Tooltip("Paginas de niveles")]
        public Page[] pages;

        public Logic.Map[] Maps { get => levels; }

        // Niveles de un lote
        private Logic.Map[] levels;

        public bool Parse()
        {
            bool done = false;


            return done;
        }
    }
}