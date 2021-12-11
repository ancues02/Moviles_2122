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

        //los caminos que hay en el tablero durante el juego
        //cada fila guarda un flow diferente. Siempre tienen como minimo el incio y fin de un flow
        private List<Tile>[] _flows;
        private int _flowsIndex;//el indice de la tuberia que se esta modificando

        private List<Tile>[] _tmpFlows;//flows que se han cortado mientras estas pulsando

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
            _flows = new List<Tile>[m.FlowNumber];
            _tmpFlows = new List<Tile>[m.FlowNumber];

            for (int i = 0; i< m.Width; ++i)
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

        // Dice que Tiles son main, es decir, los circulos grandes,
        // les pone su color y tambien en los añadimos a los flows del boardManager
        private void setMainTiles()
        {
            List<Logic.Map.Flow> flows = map.Flows;
            int i = 0;
            foreach(Logic.Map.Flow f in map.Flows){
                _tiles[f.start.x, f.start.y].ChangeColor(colors[i]);
                _tiles[f.start.x, f.start.y].setIsMain(true);
                _tiles[f.start.x, f.start.y].setVisible(true);
                _flows[i] = new List<Tile>();
                _tmpFlows[i] = new List<Tile>();
                _flows[i].Add(_tiles[f.start.x, f.start.y]);

                _tiles[f.end.x, f.end.y].ChangeColor(colors[i]);
                _tiles[f.end.x, f.end.y].setIsMain(true);
                _tiles[f.end.x, f.end.y].setVisible(true);
                _flows[i].Add(_tiles[f.end.x, f.end.y]);

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
                    if (currentTile.getIsMain())
                    {
                        int i = 0;
                        while (i < _flows.Length)
                        {
                            if (_flows[i].Contains(currentTile))
                                break;
                            i++;
                        }
                        _flowsIndex = i;
                        deactivateMain();
                    }
                }
            }
        }

        void ReleaseInput(Vector2 pos)
        {
            lastKnownTile = null;
            pressedColor = Color.black;
            // quitar informacion de tuberias cortadas
            for(int i = 0; i < _tmpFlows.Length; ++i)
            {
                _tmpFlows[i].Clear();
            }
        }

        void DragInput(Vector2 pos)
        {
            Vector2Int boardPos = new Vector2Int(Mathf.FloorToInt((pos - vectorOffset).x),
                Mathf.FloorToInt(-(pos - vectorOffset).y));
            if (boardPos.x >= 0 && boardPos.x < _height &&
                boardPos.y >= 0 && boardPos.y < _width)
            {
                /*if(!_tiles[boardPos.y, boardPos.x].getIsMain() ||
                    (_tiles[boardPos.y, boardPos.x].getIsMain() &&
                    _tiles[boardPos.y, boardPos.x].getColor() == pressedColor))*/
                    currentTile =  _tiles[boardPos.y, boardPos.x];
                
            }
        }

        // Si se pulsa en un Tile main, se desactiva todo el flow que tenia
        private void deactivateMain()
        {
            //desactivamos todas las tiles de la tuberia           
            _flows[_flowsIndex][0].deactiveAll();
            while (_flows[_flowsIndex].Count > 2)
            {
                _flows[_flowsIndex][1].deactiveAll();
                _flows[_flowsIndex].RemoveAt(1);
            }
            _flows[_flowsIndex][1].deactiveAll();

        }

        // Desactiva el camino de una tuberia
        private void deactivate(Logic.Directions dir)
        {
            /*
                
                // buscamos donde guardar la nueva tuberia cortada
                int j = 0;
                while (_tmpFlows[j].Count != 0)
                {
                    j++;
                }
                //desactivamos todas las tiles de la tuberia y las aniadimos a la lista de tuberias cortadas
                while (_flows[_flowsIndex].Count > 2)
                {
                    _tmpFlows[j].Add(_flows[i][1]);
                    _flows[_flowsIndex][1].deactive();
                    _flows[_flowsIndex].RemoveAt(1);
                }*/

            
        }


        // Comprueba si hay que cambiar el color de una tuberia
        private bool canActivate()
        {
            return lastKnownTile != null && currentTile != lastKnownTile &&
                (!currentTile.getIsMain() ||
                currentTile.getIsMain() && currentTile.getColor() == pressedColor)
                && lastKnownTile.getColor() != Color.black;
        }

        void ProcessTileChange()
        {
            if(canActivate())
            {
                int i = 0;
                while( i < _flows.Length)
                {
                    if (_flows[i].Contains(lastKnownTile))
                        break;
                    i++;
                }
                _flowsIndex = i;
                bool deActivate = false;
                // Compare positions
                Vector2Int lastPos = lastKnownTile.getBoardPos(),
                    newPos = currentTile.getBoardPos();

                Logic.Directions fromDir = Logic.Directions.Right;
                Logic.Directions toDir = Logic.Directions.Right;
                if (lastPos.y == newPos.y - 1 && lastPos.x == newPos.x)//has ido a la derecha
                {
                    lastKnownTile.modify(toDir = Logic.Directions.Right, true);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Left, true);

                }
                else if (lastPos.y == newPos.y + 1 && lastPos.x == newPos.x)//has ido a la izquierda
                {
                    lastKnownTile.modify(toDir = Logic.Directions.Left, true);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Right, true);

                }
                else if(lastPos.x == newPos.x + 1 && lastPos.y == newPos.y)//has ido arriba
                {
                    lastKnownTile.modify(toDir = Logic.Directions.Up, true);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Down, true);
                }
                else if (lastPos.x == newPos.x - 1 && lastPos.y == newPos.y)//has ido abajo
                {
                    lastKnownTile.modify(toDir = Logic.Directions.Down, true);
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Up, true);

                }


                // Si entra aqui es que la tile tenia un color diferente al nuevo
                // y hay que "borrar" el camino que tenia esa tuberia, se ha cortado el flujo
                if (deActivate)
                {
                    deactivate(fromDir);
                }
                if(!currentTile.getIsMain())
                _flows[_flowsIndex].Insert(_flows[_flowsIndex].Count-1, currentTile);

            }
            lastKnownTile = currentTile;
        }

        private void DragInput()
        {
            DragInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ProcessTileChange();
            //if (currentTile.getIsMain())
            //    pressedColor = currentTile.getColor();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                PressInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButtonUp(0))
                ReleaseInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (Input.GetMouseButton(0))
                DragInput();
#else
            foreach(Input inp in Input.touches)
            {
                
            }
#endif
        }
    }

}