using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{

    [CreateAssetMenu(fileName = "levelpack",
        menuName = "Flow/Level pack", order = 1)]
    public class LevelPack : ScriptableObject
    {
        [Tooltip("Nombre del lote")]
        public string packName;

        [Tooltip("Fichero con los niveles")]
        public TextAsset maps;
    }


    [CreateAssetMenu(fileName = "packColor",
        menuName = "Flow/Color pack", order = 1)]
    public class ColorPack : ScriptableObject
    {
        [Tooltip("Color del pack")]
        public Color[] colors;
    }


    [CreateAssetMenu(fileName = "new Color Theme",
        menuName = "Flow/ColorTheme", order = 1)]
    public class ColorTheme : ScriptableObject
    {
        [Tooltip("Colores del tema")]
        public Color[] colors;
    }
}