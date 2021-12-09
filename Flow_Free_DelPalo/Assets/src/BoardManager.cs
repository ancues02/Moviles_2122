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

        // Obtenerlos del GameManager
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

            for(int i = 0; i< m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(j, -i), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i, j].name=$"Tile {i} {j}";
                    _tiles[i, j].setVisible(false);

                    if (j == 0)
                        _tiles[i, j].activeLeft();
                    
                    if (i == 0)
                        _tiles[i, j].activeTop();
                }
            }
            setMainTiles();
            transform.Translate(new Vector2(-m.Width / 2f +0.5f, (-m.Height / 2f)+m.Height - 0.5f) );
        }

        private void setMainTiles()
        {
            List<Logic.Map.Flow> flows = map.Flows;
            int i = 0;
            foreach(Logic.Map.Flow f in map.Flows){
                _tiles[f.start.x, f.start.y].setIsMain(true);
                _tiles[f.start.x, f.start.y].setVisible(true);
                _tiles[f.start.x, f.start.y].ChangeColor(colors[i]);


                _tiles[f.end.x, f.end.y].setIsMain(true);
                _tiles[f.end.x, f.end.y].setVisible(true);
                _tiles[f.end.x, f.end.y].ChangeColor(colors[i]);

                i++;
            }
        }

        


    }

}