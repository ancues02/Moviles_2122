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
            colors.Add(Color.green);
            colors.Add(Color.yellow);
            colors.Add(Color.cyan);
            colors.Add(Color.white);

            //esto no deberia llamarse aqui
            setMap(new Logic.Map());

            
        }


        //el mapa tiene que llegar ya parseado
        public void setMap(Logic.Map m)
        {
            map = m;
            m.Parse("");
            _tiles = new Tile[m.Width, m.Height];
            int contColor = 0;
            for(int i = 0; i< m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(j, -i), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i,j].name=$"Tile {i} {j}";
                    if (map.IsFlow(i, j))//si eres inicio o fin de una tuberia
                    {
                        _tiles[i, j].setIsMain(true);
                        //devuelve el color que tiene la tuberia (negro default)
                        Color c = map.InitialColor(i, j, colors[contColor]);
                        if (c != Color.black)//es la primera vez que encuentra esta tuberia
                        {
                            _tiles[i, j].ChangeColor(colors[contColor++]);
                        }
                        else
                            _tiles[i, j].ChangeColor(c);
                    }
                    else
                        _tiles[i, j].setVisible(false);
                        //para mostrar los inicios/fin de las tuberias
                }
            }
            transform.Translate(new Vector2(-m.Width / 2f, (-m.Height / 2f)+m.Height));
        }

       

        

    }
}