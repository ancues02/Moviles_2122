using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowFree
{
    [CreateAssetMenu(fileName = "Category",
        menuName = "Flow/Category", order = 1)]

    /**
     * Scriptable Object para gestionar las categorias como datos.
     * Contiene el nombre y los lotes de la categor�a.
     */
    public class Category : ScriptableObject
    {
        [Tooltip("Nombre de la categoria")]
        public string categoryName;

        [Tooltip("Color de la categoria")]
        public Color categoryColor;

        [Tooltip("Lotes de la categoria")]
        public LevelPack[] packs;
    }
}
