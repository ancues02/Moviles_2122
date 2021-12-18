using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Cambia el sprite segun si el nivel/pack
 * esta bloqueado, completado o perfecto.
 */
namespace FlowFree
{
    public class ChangeStateSprite : MonoBehaviour
    {
        public Image stateImage;

        public Sprite BlockedSprite;
        public Sprite CompleteSprite;
        public Sprite PerfectSprite;

        public void setNone()
        {
            stateImage.enabled = false;
        }
        public void setBlocked()
        {
            stateImage.sprite = BlockedSprite; 
            stateImage.enabled = true;
        }
        public void setComplete()
        {
            stateImage.sprite = CompleteSprite;
            stateImage.enabled = true;
        }
        public void setPerfect()
        {
            stateImage.sprite = PerfectSprite;
            stateImage.enabled = true;
        }

        public void setColor(Color c)
        {
            stateImage.color = c;
        }
    }
}
