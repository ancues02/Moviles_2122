using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class FitTextColorToTheme : MonoBehaviour
    {
        public Text Text;
        void Start()
        {
            Color[] themeColors = GameManager.getInstance().theme.colors;        
            string aux = "";

            for (int i = 0; i < Text.text.Length; i++)
            {
                aux += $"<color=#{ColorUtility.ToHtmlStringRGB(themeColors[i % themeColors.Length])}>{Text.text[i]}</color>";
            }
            Text.text = aux;
        }
    }
}
