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
        

        private List<Color> colors;

        /**
         * #FF0000, #008D00, #0C29FE, #EAE000,
            #FB8900, #00FFFF, #FF0AC9, #A52A2A, #800080, #FFFFFF, #9F9FBD, #00FF00, #A18A51, #09199F,
            #008080 y #FE7CEC
         */

        void Start()
        {

            colors = new List<Color>();
            colors.Add(Color.red);
            colors.Add(Color.blue);
            colors.Add(Color.white);
            colors.Add(Color.green);
            colors.Add(Color.yellow);
            colors.Add(Color.cyan);

            //esto no deberia llamarse aqui
            setMap(new Logic.Map());

            
        }


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
                    if (map.IsFlow(i, j))
                    {
                        _tiles[i, j].setIsMain(true);
                        _tiles[i, j].ChangeColor(colors[i]);
                    }
                    else
                        _tiles[i, j].setVisible(false);
                        //para mostrar los inicios/fin de las tuberias
                }
            }
            transform.Translate(new Vector2(-m.Width / 2f, -m.Height / 2f));
        }

       

        

    }
}