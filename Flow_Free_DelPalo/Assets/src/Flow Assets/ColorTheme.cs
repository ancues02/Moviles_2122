using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    [CreateAssetMenu(fileName = "new Color Theme",
        menuName = "Flow/ColorTheme", order = 1)]

    /**
     * Scriptable Object para gestionar los temas como datos.
     * Contiene los coores que tendrán los 16 flujos
     */
    public class ColorTheme : ScriptableObject
    {
        [Tooltip("Colores del tema")]
        public Color[] colors;
    }
}