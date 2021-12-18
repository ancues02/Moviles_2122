using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    [CreateAssetMenu(fileName = "Levelpack",
        menuName = "Flow/Level pack", order = 1)]

    /**
    * Scriptable Object para gestionar los lotes como datos.
    * Contiene el nombre del lote y el fichero de texto
    * que conforma el lote.
    */
    public class LevelPack : ScriptableObject
    {
        [Tooltip("Nombre del lote")]
        public string packName;

        [Tooltip("Fichero con los niveles")]
        public TextAsset maps;

        [Tooltip("Paginas de niveles")]
        public Page[] pages;

        [Tooltip("Bloqueo del lote")]
        public bool blocked;

        [Tooltip("Nivel a partir del cual todos estan bloqueados")]
        public int blockedLevelIndex;
    }
}