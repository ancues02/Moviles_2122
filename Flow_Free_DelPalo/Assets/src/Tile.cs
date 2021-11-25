using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Tile : MonoBehaviour
    {
        //public enum ConnectionType
        //{
        //    Normal,
        //    Wall,
        //    Portal,
        //    Void
        //}

        //private ConnectionType[] sidesType;


        [Tooltip("Si es inicio/fin de tuberia")]
        public bool isMain;

        public List<Sprite> sprites;

        public Sprite hintedSprite;

        [Tooltip("Sprite del circulo")]
        public SpriteRenderer renderSprite;

       

        void Start()
        {
            if (renderSprite == null)
                Debug.LogError("Espabila, que te falta una referencia");
            

            renderSprite.color = Color.red;
            renderSprite.sprite = sprites[0];
           

        }

        public void ChangeColor(Color color)
        {
            renderSprite.color = color;
        }

    }
}