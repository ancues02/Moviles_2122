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
        public ChangeStateSprite stateSprite;

        public void setColor(Color color)
        {
            button.image.color = color;
            //stateSprite.setColor(new Color(color.r, color.g, color.b, 0.5f));
        }
        
        public void setLevelNumber(int numLevel)
        {
            levelNumberText.text = numLevel.ToString();
        }

        public void setOnClick(UnityEngine.Events.UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        // TODO: heredar de algo que tenga estos metodos para no repetirlos
        public void setBlocked(bool blocked)
        {
            button.interactable = !blocked;
            if (blocked) stateSprite.setBlocked();
            else         stateSprite.setNone();
        }

        public void setComplete()
        {
            button.interactable = true;
            stateSprite.setComplete();
        }
        public void setPerfect()
        {
            button.interactable = true;
            stateSprite.setPerfect();
        }
    }
}
