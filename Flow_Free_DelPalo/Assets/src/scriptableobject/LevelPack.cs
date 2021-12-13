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

        [Tooltip("Paginas de niveles")]
        public Page[] pages;

        public Logic.Map[] Maps { get; private set; }

        public bool Valid { get; private set; }

        // Parsea todos los niveles de un lote
        public void Parse()
        {
            Valid = true;
            string [] mapsText = maps.text.Split(new char[]{'\r','\n'}, System.StringSplitOptions.RemoveEmptyEntries);
            Maps = new Logic.Map[mapsText.Length];

            int i = 0;
            while(i < mapsText.Length && Valid) {
                Maps[i] = new Logic.Map();
                Valid = Maps[i].Parse(mapsText[i]);
                i++;
            }
        }

    }
}