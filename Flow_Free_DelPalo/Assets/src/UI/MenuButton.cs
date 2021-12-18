using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Boton de seleccion de paquete (en el menu)
     */
    public class MenuButton : MonoBehaviour
    {
        public Button button;
        public Text packNameText;

        public Text packLevelsText;
        public ChangeStateSprite stateSprite;
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
            if (blocked) stateSprite.setBlocked();
            else         stateSprite.setNone();
        }
    }
}
