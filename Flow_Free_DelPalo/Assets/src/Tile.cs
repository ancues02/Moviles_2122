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

        Vector2Int boardPos;

        Color tileColor;

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
            tileColor = color;
            renderSprite.color = color;
        }

        public Color getColor()
        {
            return tileColor;
        }

        public void setVisible(bool visible)
        {
            renderSprite.enabled = visible;
        }

        public void setBoardPos(Vector2Int pos)
        {
            boardPos = pos;
        }

        public Vector2Int getBoardPos()
        {
            return boardPos;
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