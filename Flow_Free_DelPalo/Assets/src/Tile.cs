using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Tile : MonoBehaviour
    {
        [Tooltip("Sprite del circulo")]
        public SpriteRenderer circleSprite;

        [Tooltip("Sprite de la estrella")]
        public SpriteRenderer starSprite;

        [Tooltip("Sprite del flow")]
        public SpriteRenderer flowSprite;

        void Start()
        {
            if (circleSprite == null || starSprite == null || flowSprite == null)
                Debug.LogError("Espabila, que te falta una referencia");
        }

    }
}