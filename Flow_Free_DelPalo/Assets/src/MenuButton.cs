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

        // TODO: Mirar si se ha pasado niveles para esto
        public Text packLevelsText;
        public void setPackName(string packName)
        {
            packNameText.text = packName;
        }
        public void setPackTextColor(Color c)
        {
            packNameText.color = c;
        }
        public void setPackLevels(int numLevels)
        {
            packLevelsText.text = "0 / " + numLevels;
        }

        public void setOnClick(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}
