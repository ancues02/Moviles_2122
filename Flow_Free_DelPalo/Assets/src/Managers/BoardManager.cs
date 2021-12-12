using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class BoardManager : MonoBehaviour
    {
        public GameObject TilePrefab;
        public bool testing;

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
        Tile currentTile;
        Tile lastConnectedTile;
        bool colorConflict = false;

        private List<Color> colors;

        public void setMap(Logic.Map m)
        {
            map = m;
            _tiles = new Tile[m.Width, m.Height];
            _width = m.Width;
            _height = m.Height;
            _flows = new List<Tile>[m.FlowNumber];
            _tmpFlows = new List<Tile>[m.FlowNumber];

            for (int i = 0; i < m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(j, -i), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i, j].name = $"Tile {i} {j}";
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

        public void setFlowColors(Color[] cs)
        {
            // Cogemos los colores del tema actual
            colors = new List<Color>();
            foreach (Color c in cs)
            {
                colors.Add(c);
            };
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

                _tiles[f.end.x, f.end.y].ChangeColor(colors[i]);
                _tiles[f.end.x, f.end.y].setIsMain(true);
                _tiles[f.end.x, f.end.y].setVisible(true);

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
                    lastConnectedTile = currentTile = activatedTile;
                    if (currentTile.getIsMain())
                    {
                        _flowsIndex = getColorIndex(currentTile.getColor());
                        deactivateMain();
                        _flows[_flowsIndex].Add(currentTile);
                    }
                }
            }
        }

        void ReleaseInput(Vector2 pos)
        {
            lastConnectedTile = null;
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
                //if (!_tiles[boardPos.y, boardPos.x].getIsMain() ||
                //    (_tiles[boardPos.y, boardPos.x].getIsMain() &&
                //    _tiles[boardPos.y, boardPos.x].getColor() == pressedColor)) 

                currentTile = _tiles[boardPos.y, boardPos.x];
            } 
        }

        // Si se pulsa en un Tile main, se desactiva todo el flow que tenia
        private void deactivateMain()
        {
            //desactivamos todas las tiles de la tuberia           
            while (_flows[_flowsIndex].Count > 0)
            {
                _flows[_flowsIndex][0].deactiveAll();
                _flows[_flowsIndex].RemoveAt(0);
            }
        }

        // Desactiva el camino de una tuberia
        private void deactivate(Logic.Directions dir)
        {

            if (lastConnectedTile.getColor() == currentTile.getColor() && !colorConflict){
                // buscamos donde guardar la nueva tuberia cortada
                
                //desactivamos todas las tiles de la tuberia y las aniadimos a la lista de tuberias cortadas
                int index = _flows[_flowsIndex].IndexOf(currentTile);

                for (int i = index +1; i < _flows[_flowsIndex].Count;){
                    _flows[_flowsIndex][i].deactiveAll();
                    _flows[_flowsIndex].RemoveAt(i);
                }
                //while (_flows[_flowsIndex].Count > index + 1)
                //{
                //    _flows[_flowsIndex][index].deactiveAll();
                //    _flows[_flowsIndex].RemoveAt(index);
                //}

                _flows[_flowsIndex][index].notDeactiveIn();
            }
            else//un color a cortado a otro
            {
                //TODO hacer esto
                //buscar el currentIndex en otro _flow que no sea el _flowIndex
                //ver en el index dentro de ese flow y quitar hacia un lado en funcion de la longuitud

                int j = 0;
                while (_tmpFlows[j].Count != 0)
                {
                    j++;
                }

                //_tmpFlows[j].Add(_flows[_flowsIndex][index]);



                colorConflict = false;
            }
            
        }


        // Comprueba si hay que cambiar el color de una tuberia
        private bool canActivate()
        {
            return lastConnectedTile != null && currentTile != lastConnectedTile &&
                (!currentTile.getIsMain() ||
                currentTile.getIsMain() && currentTile.getColor() == pressedColor)
                && lastConnectedTile.getColor() != Color.black;
        }

        void ProcessTileChange()
        {
            if(canActivate())
            {
                int i = 0;
                while( i < _flows.Length)
                {
                    if (_flows[i].Contains(lastConnectedTile))
                        break;
                    i++;
                }
                _flowsIndex = i;
                bool deActivate = false;
                // Compare positions
                Vector2Int lastPos = lastConnectedTile.getBoardPos(),
                    newPos = currentTile.getBoardPos();

                bool legalMove = false;
               
                Logic.Directions fromDir = Logic.Directions.Right;
                Logic.Directions toDir = Logic.Directions.Right;
                if (lastPos.y == newPos.y - 1 && lastPos.x == newPos.x)//has ido a la derecha
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Right, true);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Left, true);
                    legalMove = true;
                }
                else if (lastPos.y == newPos.y + 1 && lastPos.x == newPos.x)//has ido a la izquierda
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Left, true);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Right, true);
                    legalMove = true;

                }
                else if(lastPos.x == newPos.x + 1 && lastPos.y == newPos.y)//has ido arriba
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Up, true);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Down, true);
                    legalMove = true;
                }
                else if (lastPos.x == newPos.x - 1 && lastPos.y == newPos.y)//has ido abajo
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Down, true);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    currentTile.ChangeColor(pressedColor);
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Up, true);
                    legalMove = true;
                }


                // Si entra aqui es que la tile tenia un color diferente al nuevo
                // y hay que "borrar" el camino que tenia esa tuberia, se ha cortado el flujo
                if (deActivate)
                {
                    deactivate(fromDir);
                    lastConnectedTile = currentTile;
                }
                else if (legalMove)
                {
                    if(!_flows[_flowsIndex].Contains(currentTile))
                        _flows[_flowsIndex].Add(currentTile);
                    else 
                    {
                        lastConnectedTile.deactiveIn();
                        if (!currentTile.getIsMain())
                            currentTile.notDeactiveIn();
                        else
                            currentTile.deactiveAll();
                        _flows[_flowsIndex].Remove(lastConnectedTile);
                        
                    }
                    lastConnectedTile = currentTile;
                }
                    



            }
            Debug.Log(_flows[_flowsIndex].Count);
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

        int getColorIndex(Color c)
        {
            int index = -1; 
            int i = 0;
            while (index == -1 && i < colors.Count)
            {
                if (c == colors[i])
                {
                    index = i;
                    break;
                }
                ++i;
            }
            return index;
        }
    }

}