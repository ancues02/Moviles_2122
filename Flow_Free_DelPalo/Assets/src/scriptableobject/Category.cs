using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FlowFree
{
    /**
     * Scriptable Object para gestionar las categorias como datos.
     * Contiene el nombre y los lotes de la categor�a.
     */
    [CreateAssetMenu(fileName = "Category",
        menuName = "Flow/Category", order = 1)]
    public class Category : ScriptableObject
    {
        [Tooltip("Nombre de la categoria")]
        public string categoryName;

        [Tooltip("Color de la categoria")]
        public Color categoryColor;

        [Tooltip("Lotes de la categoria")]
        public LevelPack[] packs;

        CategoryData_ data_;
        public Category()
        {
            data_ = new CategoryData_();
            data_.packs = new PackData_[packs.Length];
            for(int i = 0; i < packs.Length; i++)
            {
                data_.packs[i] = packs[i].Data;
            }
        }
    }

    [System.Serializable]
    public class CategoryData_
    {
        public PackData_[] packs;
    }

    [System.Serializable]
    public class GameData_
    {
        public string hash;
        public int hints;
        public CategoryData_[] categoriesData;
    }
}
