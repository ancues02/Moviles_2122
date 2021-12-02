using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Tile : MonoBehaviour
    {
        public enum ConnectionType
        {
            Normal,
            Wall,
            Void
        }
        struct Connection
        {
            ConnectionType[] sidesType;
        }



        bool isMain = false;

        public bool getIsMain() { return isMain; }
        public void setIsMain(bool isFlow) { isMain = isFlow; }
        public List<Sprite> sprites;

        public Sprite hintedSprite;

        [Tooltip("Sprite del circulo")]
        public SpriteRenderer renderSprite;

        [Tooltip("Hijos del tile")]
        public GameObject[] childrens;


        void Start()
        {
            if (renderSprite == null) 
                Debug.LogError("Espabila, que te falta una referencia");
            

            //renderSprite.color = Color.red;
            renderSprite.sprite = sprites[0];
            //renderSprite.color = Color.black;
           

        }

        public void ChangeColor(Color color)
        {
            renderSprite.color = color;
        }



        public void setVisible(bool visible)
        {
            renderSprite.enabled = visible;
        }

        public void activeTop()
        {
            childrens[3].SetActive(true);
        }

        public void activeLeft()
        {
            childrens[2].SetActive(true);
        }

    }
}