using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class BoardManager : MonoBehaviour
    {
        public GameObject TilePrefab;
        public LevelManager lvlManager;

        [Tooltip ("El texto del canvas de flows")]
        public Text flowText;

        [Tooltip("El texto del canvas de moves")]
        public Text movesText;

        [Tooltip("El texto del canvas de pipe")]
        public Text pipesText;

        [Tooltip("El texto del canvas de pistas")]
        public Text hintText;

        private Vector2 _cameraSize;
        private Vector2 _availableSize;
        private Vector2 _baseRatio;

        [Tooltip("El panel del canvas de de ganar")]
        public GameObject winPanel;

        private Tile[,] _tiles;
        private int _width, _height;
        private Logic.Map map;

        //los caminos que hay en el tablero durante el juego
        //cada fila guarda un flow diferente. Siempre tienen como minimo el incio y fin de un flow
        private List<Tile>[] _flows;
        private int _flowsIndex;//el indice de la tuberia que se esta modificando

        private List<List<Tile>> _tmpFlows;//flows que se han cortado mientras estas pulsando

        Vector2 vectorOffset;
        Color pressedColor = Color.black, lastPressedColor = Color.black;
        Tile currentTile;
        Tile lastConnectedTile;
        bool colorConflict = false;

        int totalFlows, best, totalNumPipes;
        int numFlows, moves, numPipes;

        bool win = false;

        bool end = false;

        private List<Color> colors;

        private void Start()
        {
            if (!flowText || !movesText || !pipesText || !hintText
                || !TilePrefab || !lvlManager || !winPanel)
                Debug.LogError("Falta alguna referencia en BoardManager!");
            else
            {
                moves = 0;
                //flowText.text = "flows: 0/" + ;
                movesText.text = "moves:" + moves + " best: -";
                pipesText.text = "pipe: 0%";
                hintText.text = "3 x ";
            }
        }
        
        public void getCameraSize()
        {
            if (Camera.main)
            {
                float y = Camera.main.orthographicSize * 2;
                float x = y * Camera.main.aspect;
                y *= 0.75f;  // Lo de la UI? No sé como vamos a hacerlo
                _cameraSize = new Vector2(x, y);
            }
            else Debug.LogError("No hay cámara, ¿qué esperabas que pasara?");
        }

        public void setMap(Logic.Map m)
        {
            map = m;
            _tiles = new Tile[m.Width, m.Height];
            _width = m.Width;
            _height = m.Height;

            float camAspect = _cameraSize.x / _cameraSize.y;
            float mapAspect = _width / (float)_height;
            
            if (camAspect >= 1)
                if(mapAspect >= camAspect)
                    _availableSize = new Vector2(_cameraSize.x, _cameraSize.x / mapAspect);
                else
                    _availableSize = new Vector2(_cameraSize.y * mapAspect, _cameraSize.y);
            else
                if (mapAspect >= camAspect)
                    _availableSize = new Vector2(_cameraSize.x, _cameraSize.x / mapAspect);
                else
                    _availableSize = new Vector2(_cameraSize.y * mapAspect, _cameraSize.y);

            _baseRatio = new Vector2(_availableSize.x / (float)_width, _availableSize.y / (float)_height);
            _flows = new List<Tile>[m.FlowNumber];
            _tmpFlows = new List<List<Tile>>();
            numPipes = 0;
            numFlows = moves= 0;

            for (int i = 0; i < m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    // Esta bien colocado por el pivot del sprite
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(i, -j), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i, j].name = $"Tile {i} {j}";
                    _tiles[i, j].setVisible(false);
                    _tiles[i, j].setBoardPos(new Vector2Int(i, j));
                    _tiles[i, j].ChangeColor(Color.black);
                    if (i == 0)
                        _tiles[i, j].activeLeftLimit();

                    if (j == 0)
                        _tiles[i, j].activeTop();
                }
            }
            totalFlows = map.FlowNumber;
            totalNumPipes = m.Width* m.Height;
            setMainTiles();
            vectorOffset = new Vector2((-m.Width / 2f) * _baseRatio.x, ((-m.Height / 2f) * _baseRatio.y) + m.Height * _baseRatio.y);
            transform.localScale = new Vector3(_baseRatio.x, _baseRatio.y, 1);
            transform.Translate(new Vector2(vectorOffset.x  + (0.5f * _baseRatio.x), vectorOffset.y  - (0.5f * _baseRatio.y)));
            flowText.text = "flows: 0/" + totalFlows;
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
        // les pone su color y tambien en los a�adimos a los flows del boardManager
        private void setMainTiles()
        {
            List<Logic.Map.Flow> flows = map.Flows;
            int i = 0;
            foreach(Logic.Map.Flow f in map.Flows){
                _tiles[f.start.x, f.start.y].ChangeColor(colors[i]);
                _tiles[f.start.x, f.start.y].setIsMain(true);
                _tiles[f.start.x, f.start.y].setVisible(true);
                _flows[i] = new List<Tile>();

                _tiles[f.end.x, f.end.y].ChangeColor(colors[i]);
                _tiles[f.end.x, f.end.y].setIsMain(true);
                _tiles[f.end.x, f.end.y].setVisible(true);

                i++;
            }
        }

        void PressInput(Vector2 pos)
        {
            Vector2Int boardPos = getBoardTile(pos);
            if (boardPos.x >= 0 && boardPos.x < _width &&
                boardPos.y >= 0 && boardPos.y < _height)
            {
                
                Tile activatedTile = _tiles[boardPos.x, boardPos.y];
                if (activatedTile.getColor() != Color.black)
                {
                    pressedColor = activatedTile.getColor();
                    lastConnectedTile = currentTile = activatedTile;
                    _flowsIndex = getColorIndex(currentTile.getColor());
                    if (currentTile.getIsMain())
                    {
                        deactivateMain();
                        _flows[_flowsIndex].Add(currentTile);
                    }
                    else
                    {
                        if(_flows[_flowsIndex][_flows[_flowsIndex].Count-1] != activatedTile)
                        deactivateItSelf();
                    }
                }
            }
        }

        void ReleaseInput(Vector2 pos)
        {
            lastConnectedTile = null;
            if (pressedColor != lastPressedColor)
            {
                moves++;
                checkMoves();
            }
            lastPressedColor = pressedColor;
            pressedColor = Color.black;
            // quitar informacion de tuberias cortadas
            for(int i = 0; i < _tmpFlows.Count; ++i)
            {
                _tmpFlows[i].Clear();
            }
            if (numFlows == totalFlows)
            {
                win = true;
                winPanel.SetActive(true);
            }
        }

        void DragInput(Vector2 pos)
        {
            Vector2Int boardPos = getBoardTile(pos);
            if (boardPos.x >= 0 && boardPos.x < _width &&
                boardPos.y >= 0 && boardPos.y < _height)
            {
                //if (!_tiles[boardPos.y, boardPos.x].getIsMain() ||
                //    (_tiles[boardPos.y, boardPos.x].getIsMain() &&
                //    _tiles[boardPos.y, boardPos.x].getColor() == pressedColor)) 

                currentTile = _tiles[boardPos.x, boardPos.y];
            } 
        }

        private Vector2Int getBoardTile(Vector2 pos)
        {
            int x = Mathf.FloorToInt((pos - vectorOffset).x / _baseRatio.x);
            int y = Mathf.FloorToInt(-(pos - vectorOffset).y / _baseRatio.y);
            return new Vector2Int(x, y);
        }

        // Si se pulsa en un Tile main, se desactiva todo el flow que tenia
        private void deactivateMain()
        {
            //Debug.Log(_flows[_flowsIndex].Count);
            //desactivamos todas las tiles de la tuberia           
            while (_flows[_flowsIndex].Count > 0)
            {
                _flows[_flowsIndex][0].deactiveAll();
                _flows[_flowsIndex].RemoveAt(0);
            }
            checkPipes();
            checkFlows();
        }

        /// <summary>
        /// Modifica el texto de pipes del canvas
        /// </summary>
        private void checkPipes()
        {
            numPipes = 0;
            for (int i = 0; i < _flows.Length; ++i)
            {
                numPipes += _flows[i].Count;
            }
            int percent = Mathf.RoundToInt((numPipes / (float)totalNumPipes) * 100);
            pipesText.text = "pipe: " + percent + "%";
        }

        /// <summary>
        /// Modifica el texto de flows del canvas
        /// </summary>
        private void checkFlows()
        {
            numFlows = 0;
            for (int i = 0; i < _flows.Length; ++i)
            {
                if(_flows[i].Count > 2 && _flows[i][_flows[i].Count -1].getIsMain())
                    numFlows++;               
            }
            flowText.text = "flows: "+ numFlows + "/" + totalFlows;
            
        }

        /// <summary>
        /// Modifica el texto de moves del canvas
        /// </summary>
        private void checkMoves()
        {

            movesText.text = "moves:"+moves+" best: -";
        }

        /// <summary>
        /// Desactiva si una tuberia a cortado a otra
        /// </summary>
        private void deactivateByColor(Logic.Directions dir)
        {
            int ind = 0;
            while (!_flows[ind].Contains(currentTile) /*|| ind == _flowsIndex*/)
            {
                ind++;
            }
            //_tmpFlows.Add(_flows[ind]);

            int index = _flows[ind].IndexOf(currentTile);

            bool halfUp = _flows[ind].Count / 2 < index;
            int i = 0;
            //te cortan por la primera mitad y estaba finalizada la tuberia
            if (!halfUp && _flows[ind][_flows[ind].Count - 1].getIsMain())
            {
                for (int j = 0; j <= index && index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].deactiveAll();
                    _flows[ind].RemoveAt(i);
                }
                _flows[ind][0].deactiveIn();
                _flows[ind].Reverse();
                foreach(Tile t in _flows[ind])
                {
                    t.swap();
                }
            }//te cortan por la segunda mitad y estaba finalizada la tuberia
            else if (_flows[ind][_flows[ind].Count - 1].getIsMain())
            {
                i = index;
                for (int j = 0; j <= index && index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].deactiveAll();
                    _flows[ind].RemoveAt(i);
                    
                }
                _flows[ind][_flows[ind].Count - 1].deactiveOut();
                
            }//te cortan por la segunda mitad y estaba finalizada la tuberia
            else
            {
                i = index;
                for (int j = 0; index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].deactiveAll();
                    _flows[ind].RemoveAt(i);
                    
                }
                _flows[ind][_flows[ind].Count - 1].deactiveOut();
            }

            //_tmpFlows[j].Add(_flows[_flowsIndex][index]);
            _flows[_flowsIndex].Add(currentTile);
            currentTile.modify(dir, true, pressedColor);
            colorConflict = false;
        }

        /// <summary>
        /// Desactiva si una tuberia se ha cortado a si misma
        /// </summary>
        private void deactivateItSelf()
        {
            // buscamos donde guardar la nueva tuberia cortada

            //desactivamos todas las tiles de la tuberia y las aniadimos a la lista de tuberias cortadas
            int index = _flows[_flowsIndex].IndexOf(currentTile);

            for (int i = index + 1; i < _flows[_flowsIndex].Count;)
            {
                _flows[_flowsIndex][i].deactiveAll();
                _flows[_flowsIndex].RemoveAt(i);
            }

            _flows[_flowsIndex][index].notDeactiveIn();
        }

        // Desactiva el camino de una tuberia
        private void deactivate(Logic.Directions dir)
        {

            if (lastConnectedTile.getColor() == currentTile.getColor() && !colorConflict){
                deactivateItSelf();
            }
            else//un color a cortado a otro
            {
                deactivateByColor(dir);
            }
            checkFlows();

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
                if (lastConnectedTile != _flows[_flowsIndex][0] && lastConnectedTile.getIsMain() && _flows[_flowsIndex].Contains(lastConnectedTile) && !_flows[_flowsIndex].Contains(currentTile)) 
                    return;
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
                if ((lastPos.x == newPos.x - 1 && lastPos.y == newPos.y) || _flows[_flowsIndex].Contains(currentTile))//has ido a la derecha
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Right, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;                    
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Left, true, pressedColor);
                    legalMove = true;
                }
                else if ((lastPos.x == newPos.x + 1 && lastPos.y == newPos.y) || _flows[_flowsIndex].Contains(currentTile))//has ido a la izquierda
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Left, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Right, true, pressedColor);
                    legalMove = true;

                }
                else if((lastPos.y == newPos.y + 1 && lastPos.x == newPos.x) || _flows[_flowsIndex].Contains(currentTile))//has ido arriba
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Up, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Down, true, pressedColor);
                    legalMove = true;
                }
                else if ((lastPos.y == newPos.y - 1 && lastPos.x == newPos.x) || _flows[_flowsIndex].Contains(currentTile))//has ido abajo
                {
                    lastConnectedTile.modify(toDir = Logic.Directions.Down, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Up, true, pressedColor);
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
                    if (!_flows[_flowsIndex].Contains(currentTile))
                    {
                        _flows[_flowsIndex].Add(currentTile);
                        numPipes++;
                        if(currentTile.getIsMain())
                            checkFlows();
                    }
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
                //Debug.Log(_flows[_flowsIndex].Count);
                checkPipes();
            }
            
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
            if (!win)
            {
                if (Input.GetMouseButtonDown(0))
                    PressInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetMouseButtonUp(0))
                    ReleaseInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetMouseButton(0))
                    DragInput();
            }
#else
            foreach(Input inp in Input.touches)
            {
                
            }
#endif
            if (end)
            {
                Vector3 tmp = transform.localPosition;
                tmp.x += 0.02f;
                transform.localPosition = tmp;

                tmp = transform.localScale;
                tmp.x -= 0.01f;
                transform.localScale = tmp;

                if (tmp.x <= 0)
                    lvlManager.nextLevel();
            }
            
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

        public void changeLevel()
        {
            end = true;
            winPanel.SetActive(false);
        }

        
    }

}