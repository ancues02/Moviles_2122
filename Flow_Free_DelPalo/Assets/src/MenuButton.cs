using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class MenuButton : MonoBehaviour
    {
        public Button button;
        public Text packNameText;

        public Text packLevelsText;
        public void setPackName(string packName)
        {
            packNameText.text = packName;
        }
        public void setPackTextColor(Color c)
        {
            packNameText.color = c;
        }
        public void setPackLevels(int completedLevels, int numLevels)
        {
            packLevelsText.text = completedLevels + " / " + numLevels;
        }

        public void setOnClick(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public void setBlocked(bool blocked)
        {
            button.interactable = !blocked;
        }
    }
}
