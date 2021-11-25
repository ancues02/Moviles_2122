using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class BoardManager : MonoBehaviour
    {
        public GameObject TilePrefab;

        private Tile[,] _tiles;
        private int _width, _height;
        private Logic.Map map;

        //el mapa tiene que llegar ya parseado
        public void setMap(Logic.Map m)
        {
            map = m;
            m.Parse("");
            _tiles = new Tile[m.Width, m.Height];
            for(int i = 0; i< m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(i, j), Quaternion.identity, transform).GetComponent<Tile>();
                    //if(_tiles.)//para mostrar los inicios/fin de las tuberias
                }
            }
            transform.Translate(new Vector2(-m.Width / 2f, -m.Height / 2f));
        }

        void Start()
        {
            //esto no deberia llamarse aqui
            setMap(new Logic.Map());
        }

    }
}