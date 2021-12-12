using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class LevelSelectionButton : MonoBehaviour
    {
        public Button button;

        public Text levelNumberText;

        public void setColor(Color color)
        {
            button.image.color = color;
        }
        
        public void setLevelNumber(int numLevel)
        {
            levelNumberText.text = numLevel.ToString();
        }

        public void setOnClick(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}
