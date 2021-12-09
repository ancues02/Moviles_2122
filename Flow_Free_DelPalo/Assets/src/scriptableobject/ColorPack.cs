using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esto son las skins que no creo que usemos
namespace FlowFree
{
    [CreateAssetMenu(fileName = "PackColor",
        menuName = "Flow/Color pack", order = 1)]
    public class ColorPack : ScriptableObject
    {
        [Tooltip("Color del pack")]
        public Color[] colors;
    }
}