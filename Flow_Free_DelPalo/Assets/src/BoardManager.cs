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

        Vector2 vectorOffset;
        Color pressedColor = Color.black;
        Tile lastKnownTile;
        Tile currentTile;

        // Obtenerlos del GameManager
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
            _width = m.Width;
            _height = m.Height;

            for(int i = 0; i< m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(j, -i), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i, j].name=$"Tile {i} {j}";
                    _tiles[i, j].setVisible(false);
                    _tiles[i, j].setBoardPos(new Vector2Int(i, j));
                    _tiles[i, j].ChangeColor(Color.black);
                    if (j == 0)
                        _tiles[i, j].activeLeftLimit();
                    
                    if (i == 0)
                        _tiles[i, j].activeTop();
                }
            }
            setMainTiles();
            vectorOffset = new Vector2(-m.Width / 2f, (-m.Height / 2f) + m.Height);
            transform.Translate(new Vector2(vectorOffset.x + 0.5f, vectorOffset.y - 0.5f));
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

        void PressInput(Vector2 pos)
        {
            Vector2Int boardPos = new Vector2Int(Mathf.FloorToInt((pos - vectorOffset).x), 
                Mathf.FloorToInt(-(pos - vectorOffset).y));
            if (boardPos.x >= 0 && boardPos.x < _height &&
                boardPos.y >= 0 && boardPos.y < _width)
            {
                Tile activatedTile = _tiles[boardPos.y, boardPos.x];
                if (activatedTile.getColor() != Color.black)
                {
                    pressedColor = activatedTile.getColor();
                    currentTile = activatedTile;
                }
            }
        }

        void ReleaseInput(Vector2 pos)
        {
            lastKnownTile = null;
            pressedColor = Color.black;
        }

        void DragInput(Vector2 pos)
        {
            Vector2Int boardPos = new Vector2Int(Mathf.FloorToInt((pos - vectorOffset).x),
                Mathf.FloorToInt(-(pos - vectorOffset).y));
            if (boardPos.x >= 0 && boardPos.x < _height &&
                boardPos.y >= 0 && boardPos.y < _width)
            {
                Debug.Log(boardPos);
                currentTile =  _tiles[boardPos.y, boardPos.x];
                
            }
        }

        private bool canActivate()
        {
            return lastKnownTile != null && currentTile != lastKnownTile &&
                (!currentTile.getIsMain() ||
                currentTile.getIsMain() && currentTile.getColor() == pressedColor) 
                && lastKnownTile.getColor() != Color.black;
        }

        private void deactivate()
        {

        }

        void ProcessTileChange()
        {
            if(canActivate())
            {
                bool deActivate = false;
                // Compare positions
                Vector2Int lastPos = lastKnownTile.getBoardPos(),
                    newPos = currentTile.getBoardPos();
                if (lastPos.y == newPos.y - 1 && lastPos.x == newPos.x)//has ido a la derecha
                {
                    lastKnownTile.active(Logic.Directions.Right);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.active(Logic.Directions.Left);

                }
                else if (lastPos.y == newPos.y + 1 && lastPos.x == newPos.x)//has ido a la izquierda
                {
                    lastKnownTile.active(Logic.Directions.Left);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.active(Logic.Directions.Right);

                }
                else if(lastPos.x == newPos.x + 1 && lastPos.y == newPos.y)//has ido arriba
                {
                    lastKnownTile.active(Logic.Directions.Up);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.active(Logic.Directions.Down);
                }
                else if (lastPos.x == newPos.x - 1 && lastPos.y == newPos.y)//has ido abajo
                {
                    lastKnownTile.active(Logic.Directions.Down);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.active(Logic.Directions.Up);

                }

                // Si entra aqui es que la tile tenia un color diferente al nuevo
                // y hay que "borrar" el camino que tenia esa tuberia, se ha cortado el flujo
                if (deActivate)
                {
                    deactivate();
                }
            }
            lastKnownTile = currentTile;
        }

        private void OnMouseDrag()
        {
            DragInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ProcessTileChange();
            if (currentTile.getIsMain())
                pressedColor = currentTile.getColor();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                PressInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButtonUp(0))
                ReleaseInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButton(0))
                OnMouseDrag();
#else
            foreach(Input inp in Input.touches)
            {
                
            }
#endif
        }
    }

}