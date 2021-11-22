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

        public int numSides;
        private bool[] connections;

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
            
            if(numSides > 0)
                connections = new bool[numSides];
            renderSprite.color = Color.red;
            renderSprite.sprite = sprites[numSides];
            

        }

    }
}