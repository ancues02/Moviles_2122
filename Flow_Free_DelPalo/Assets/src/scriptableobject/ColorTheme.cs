using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    [CreateAssetMenu(fileName = "new Color Theme",
        menuName = "Flow/ColorTheme", order = 1)]
    public class ColorTheme : ScriptableObject
    {
        [Tooltip("Colores del tema")]
        public Color[] colors;
    }
}