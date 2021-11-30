using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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