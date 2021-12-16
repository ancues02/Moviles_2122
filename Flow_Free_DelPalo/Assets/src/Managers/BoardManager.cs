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

        [Tooltip("El script AdsRewarded del objeto que lo contiene en la escena")]
        public AdsRewarded adsRewarded;

        public SpriteRenderer pointer;     

        [Tooltip("El texto del canvas de flows")]
        public Text flowText;

        [Tooltip("El texto del canvas de moves")]
        public Text movesText;

        [Tooltip("El texto del canvas de pipe")]
        public Text pipesText;

        [Tooltip("El texto del canvas de pistas")]
        public Text hintText;

        [Tooltip("El texto del panel final de movimientos")]
        public Text panelMovesText;

        private Vector2 _cameraSize;
        private Vector2 _availableSize;
        private Vector2 _baseRatio;
        public RectTransform _canvasSize;
        public RectTransform _topSize;
        public RectTransform _botSize;

        [Tooltip("El panel del canvas de de ganar")]
        public GameObject winPanel;

        private Tile[,] _tiles;
        private int _width, _height;
        private Logic.Map map;

        //los caminos que hay en el tablero durante el juego
        private List<Tile>[] _flows;
        private int _flowsIndex;//el indice de la tuberia que se esta modificando
        private int _circleFlowsIndex = -1;//el indice de la tuberia que tiene el circulo pequenio

        private List<List<Tile>> _tmpFlows;//flows que se han cortado mientras estas pulsando

        Vector2 vectorOffset;
        Color pressedColor = Color.black,
            lastMoveColor = Color.black;
        Tile currentTile;
        Tile lastConnectedTile;
        bool colorConflict = false;
        bool legalMove = false;
        bool changes = false; 

        int totalFlows, totalNumPipes;
        int numFlows, moves, numPipes;
        public int best { get; set; }

        bool win = false;
        bool continuePlaying = false;

        enum GamePhase { begin, mid, end, NONE };
        GamePhase gameState = GamePhase.NONE;

        private List<Color> colors;

        private List<int> _hintIndexs;
        private bool _usingHint = false;    // Para que las hints no se animen
        private bool _animColor = false;    // Para que el release no haga wiggle si no tocaste nada

        private void Start()
        {
            if (!flowText || !movesText || !pipesText || !hintText
                || !TilePrefab || !lvlManager || !winPanel || !adsRewarded)
                Debug.LogError("Falta alguna referencia en BoardManager!");
            else
            {
                moves = 0;
                //flowText.text = "flows: 0/" + ;
                movesText.text = "moves:" + moves + " best: ";
                movesText.text += best == -1 ? "-" : best+"";
                pipesText.text = "pipe: 0%";
                CheckHints();
            }
        }

        public void GetCameraSize()
        {
            if (Camera.main)
            {
                float y = Camera.main.orthographicSize * 2;
                float x = y * Camera.main.aspect;
                x *= 0.99f; // Ligero margen
                float ratio = (_canvasSize.rect.height - (_topSize.rect.height + _botSize.rect.height)) / _canvasSize.rect.height;
                y *= ratio; // Lo de la UI? No sé como vamos a hacerlo
                _cameraSize = new Vector2(x, y);
            }
            else Debug.LogError("No hay cámara, ¿qué esperabas que pasara?");
        }

        public void SetMap(Logic.Map m)
        {
            
             map = m;
            _tiles = new Tile[m.Width, m.Height];
            _width = m.Width;
            _height = m.Height;

            float camAspect = _cameraSize.x / _cameraSize.y;
            float mapAspect = _width / (float)_height;

            if (camAspect >= 1)
                if (mapAspect >= camAspect)
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
            numFlows = moves = 0;

            _hintIndexs = new List<int>();
            for (int i = 0; i < m.FlowNumber; ++i)
                _hintIndexs.Add(i);

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
                }
            }

            foreach(int tile in map.Voids)
            {
                _tiles[tile % m.Width, tile / m.Width].setIsVoid();
            }

            // Muros del nivel
            foreach (Vector2Int pair in map.Walls)
            {
                Vector2Int A = new Vector2Int(pair.x % m.Width, pair.x / m.Width);
                Vector2Int B = new Vector2Int(pair.y % m.Width, pair.y / m.Width);
                if (A.x == B.x - 1 && A.y == B.y) //A|B
                {
                    _tiles[A.x, A.y].setActiveWall(Logic.Directions.Right);
                    _tiles[B.x, B.y].setActiveWall(Logic.Directions.Left);
                }
                else if (A.x == B.x + 1 && A.y == B.y)  //B|A
                {
                    _tiles[A.x, A.y].setActiveWall(Logic.Directions.Left);
                    _tiles[B.x, B.y].setActiveWall(Logic.Directions.Right);
                }
                else if (A.y == B.y + 1 && A.x == B.x)  //B/A
                {
                    _tiles[A.x, A.y].setActiveWall(Logic.Directions.Up);
                    _tiles[B.x, B.y].setActiveWall(Logic.Directions.Down);
                }
                else if (A.y == B.y - 1 && A.x == B.x)  //A/B
                {
                    _tiles[A.x, A.y].setActiveWall(Logic.Directions.Down);
                    _tiles[B.x, B.y].setActiveWall(Logic.Directions.Up);
                }
            }

            foreach(Tile t in _tiles)
            {
                Vector2Int tPos = t.getBoardPos();
                if(!t.getIsVoid() && ((tPos.x - 1 < 0 && m.isBordered()) || (tPos.x - 1 >= 0 && _tiles[tPos.x - 1, tPos.y].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Left);
                if (!t.getIsVoid() && ((tPos.x + 1 >= m.Width && m.isBordered()) || (tPos.x + 1 < m.Width && _tiles[tPos.x + 1, tPos.y].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Right);
                if (!t.getIsVoid() && ((tPos.y - 1 < 0 && m.isBordered()) || (tPos.y - 1 >= 0 && _tiles[tPos.x, tPos.y - 1].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Up);
                if (!t.getIsVoid() && ((tPos.y + 1 >= m.Height && m.isBordered()) || (tPos.y + 1 < m.Height && _tiles[tPos.x, tPos.y + 1].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Down);
            }

            totalFlows = map.FlowNumber;
            totalNumPipes = m.Width * m.Height;
            setMainTiles();
            vectorOffset = new Vector2((-m.Width / 2f) * _baseRatio.x, ((-m.Height / 2f) * _baseRatio.y) + m.Height * _baseRatio.y);
            
            //if(beginWithAnimation)
            transform.localScale = new Vector3(0, _baseRatio.y, 1);
            transform.position = new Vector2(0, vectorOffset.y - (0.5f * _baseRatio.y));
            gameState = GamePhase.begin;
            //else
            //transform.localScale = new Vector3(_baseRatio.x, _baseRatio.y, 1);
            //transform.position = new Vector2(vectorOffset.x + (0.5f * _baseRatio.x), vectorOffset.y - (0.5f * _baseRatio.y));
            //gameState = GamePhase.mid;

            flowText.text = "flows: 0/" + totalFlows;
        }

        public void SetFlowColors(Color[] cs)
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
            foreach (Logic.Map.Flow f in map.Flows)
            {
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
                PressTile(boardPos);
                if (_tiles[boardPos.x, boardPos.y].getColor() != Color.black)
                {
                    pointer.transform.position = new Vector3(pos.x, pos.y, -2);
                    pointer.enabled = true;
                    pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.5f);
                }
            }
        }

        // Presiona una tile específica
        void PressTile(Vector2Int boardPos)
        {
            Tile activatedTile = _tiles[boardPos.x, boardPos.y];
            if (activatedTile.getColor() != Color.black)
            {
                _animColor = true;
                pressedColor = activatedTile.getColor();
                lastConnectedTile = currentTile = activatedTile;
                _flowsIndex = GetColorIndex(currentTile.getColor());

                // pistas
                if (CheckIfHintedFlow(_flowsIndex))
                    PutStars(_flowsIndex, false);

                if (currentTile.getIsMain())
                {
                    DeactivateMain();
                    if(!_usingHint && _animColor) playWiggleOnExtreme(currentTile);
                }
                else
                {
                    //desactivar el circulo pequenio
                    int lastInd = _flows[_flowsIndex].Count - 1;

                    //si no eres el final, desactivar el resto
                    if (_flows[_flowsIndex][lastInd] != activatedTile)
                    {
                        DeactivateItSelf(Logic.Directions.None);
                        changes = true;
                    }
                }
                
            }
        }

        void playWiggleOnExtreme(Tile start)
        {
            int index = GetColorIndex(start.getColor());
            Vector2Int target = map.Flows[index].start;
            if (start.getBoardPos() == target) 
                target = map.Flows[index].end;
            _tiles[target.x, target.y].animations.Wiggle();
        }

        void ReleaseInput(Vector2 pos)
        {
            lastConnectedTile = null;
            //Aumentar numero de movimientos si ha habido algun cambio
            //y es un color diferente al anterior con el que se ha aumentado el numero de movimientos
            if ((/*_flows[_flowsIndex].Count > 1 ||*/ changes) && lastMoveColor != pressedColor)
            {
                lastMoveColor = pressedColor;
                moves++;
                CheckMoves();                
            }

            //Desactivar o activar el circulo pequenio solo si se ha cambiado alguna tuberia
            if (changes)
            {
                //desactivar el que esta activo (el anterior que se ha activado)
                if (_circleFlowsIndex != -1 && _flows[_circleFlowsIndex].Count > 1)
                    _flows[_circleFlowsIndex][_flows[_circleFlowsIndex].Count - 1].SmallCircleSetActive(false);
               //activar el nuevo, el de la tuberia que se ha modificado
                if (_flows[_flowsIndex].Count > 1)
                {
                    _flows[_flowsIndex][_flows[_flowsIndex].Count - 1].SmallCircleSetActive(true);
                    _circleFlowsIndex = _flowsIndex;
                }
                changes = false;
            }

            // quitar informacion de tuberias cortadas
            while (_tmpFlows.Count != 0)
                _tmpFlows.RemoveAt(0);

            pointer.enabled = false;

            // pistas
            if (CheckIfHintedFlow(_flowsIndex))
                PutStars(_flowsIndex, true);

            if (!_usingHint && _animColor && _flows[_flowsIndex].Count > 0)
                if(_flows[_flowsIndex][_flows[_flowsIndex].Count - 1].getIsMain())
                    _flows[_flowsIndex][_flows[_flowsIndex].Count - 1].animations.Pulse();
                else
                    _flows[_flowsIndex][_flows[_flowsIndex].Count - 1].animations.SmallWiggle();

            // comprobar si se ha ganado
            if (numFlows == totalFlows)
            {
                //para saltarme el frame de pulsar la cruz del panel y seguir jugando
                //si hacemos esto detecta el release de la pulsacion y vuelve a ganar
                if (continuePlaying)
                {
                    continuePlaying = false;
                    return;
                }
                win = true;
                winPanel.SetActive(true);
                GameManager.getInstance().LevelComplete(moves);
                panelMovesText.text = "You completed the level\n"+" in " + moves +" moves";
                foreach (Tile t in _tiles)
                    if (t.getIsMain()) t.animations.Pulse();
            }

            _animColor = false;
        }

        // Mira si es uno en el que se ha hecho una pista
        bool CheckIfHintedFlow(int flowsindex)
        {
           
            bool wasHinted = !_hintIndexs.Contains(flowsindex);
            bool isCorrect = _flows[flowsindex].Count == map.Flows[flowsindex].flowPoints.Count;
            int i = 0;
            while (isCorrect && i < _flows[flowsindex].Count)
            {
                isCorrect = map.Flows[flowsindex].flowPoints.Contains(_flows[flowsindex][i].getBoardPos());
                ++i;
            }
            return wasHinted && isCorrect;
        }

        // Pone o no estrellas en los extremos main
        void PutStars(int flowsindex, bool toggle)
        {
            if (_flows[flowsindex].Count > 0)
            {
                if (_flows[flowsindex][0].getIsMain()) _flows[flowsindex][0].childrens[5].SetActive(toggle);
                if (_flows[flowsindex][_flows[flowsindex].Count - 1].getIsMain()) _flows[flowsindex][_flows[flowsindex].Count - 1].childrens[5].SetActive(toggle);
            }
        }

        void PorcessDragInput(Vector2 pos)
        {
            Vector2Int boardPos = getBoardTile(pos);
            if (boardPos.x >= 0 && boardPos.x < _width &&
                boardPos.y >= 0 && boardPos.y < _height)
            {
                DragTile(boardPos);
                if (currentTile.getIsMain())
                {
                    if(currentTile.getColor() == pressedColor )
                    {
                        if(_flows[_flowsIndex].Count > 2)
                            pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.75f);                        
                    }
                    else
                        pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.25f);

                }
                else
                    pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.5f);
            }
            else pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.25f);
            pointer.transform.position = new Vector3(pos.x, pos.y, -2);
        }

        // Hace drag sobre una tile específica
        void DragTile(Vector2Int boardPos)
        {
            currentTile = _tiles[boardPos.x, boardPos.y];
        }

        private Vector2Int getBoardTile(Vector2 pos)
        {
            int x = Mathf.FloorToInt((pos - vectorOffset).x / _baseRatio.x);
            int y = Mathf.FloorToInt(-(pos - vectorOffset).y / _baseRatio.y);
            return new Vector2Int(x, y);
        }

        // Si se pulsa en un Tile main, se desactiva todo el flow que tenia
        private void DeactivateMain()
        {

            if (_flows[_flowsIndex].Count > 0)
                changes = true;
            
            //desactivamos todas las tiles de la tuberia           
            while (_flows[_flowsIndex].Count > 0)
            {
                _flows[_flowsIndex][0].DeactiveAll();
                _flows[_flowsIndex].RemoveAt(0);
            }
            CheckPipes();
            CheckFlows();
            
        }

        /// <summary>
        /// Modifica el texto de pipes del canvas
        /// </summary>
        private void CheckPipes()
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
        private void CheckFlows()
        {
            numFlows = 0;
            for (int i = 0; i < _flows.Length; ++i)
            {
                if (_flows[i].Count > 2 && _flows[i][_flows[i].Count - 1].getIsMain())
                    numFlows++;
            }
            flowText.text = "flows: " + numFlows + "/" + totalFlows;

        }

        /// <summary>
        /// Modifica el texto de moves del canvas
        /// </summary>
        private void CheckMoves()
        {
            movesText.text = "moves:" + moves + " best: ";
            movesText.text += best == -1 ? "-" : best + "";
        }

        /// <summary>
        /// Modifica el texto de pistas del canvas
        /// </summary>
        public void CheckHints()
        {
            hintText.text = GameManager.getInstance().GetHints() + " x ";
        }


        
        /// <summary>
        /// Cuando se corta una tuberia con otro color
        /// Se guarda el estado de la tuberia antes de ser cortada
        /// </summary>
        /// <param name="ind"> el indice del flow que se ha cortado</param>
        private void CutFlow(int ind)
        {
            //comprobar si se ha cortado uno con pista, entonces quitarla
            if (CheckIfHintedFlow(ind))
            {
                Debug.Log("Desactivar");
                PutStars(ind, false);
            }
            //no hacer nada si ya se ha guardado durante ese drag del flow que se ha cortado la tuberia
            foreach (List<Tile> t in _tmpFlows)
            {
                if (t[0].getColor() == _flows[ind][0].getColor())
                    return;
            }

            //lista temporal donde guardar una copia clonada del flow cortado
            List<Tile> tmpList = new List<Tile>();
            //copiar el estado de ese flow a la lista de flows cortados para poder restaurar su estado
            for (int i = 0; i < _flows[ind].Count; ++i)
            {
                Tile t = _flows[ind][i];
                Tile tmp = (Tile)t.Clone();
                //donde se ha cortado volver a poner bien los caminos 
                //de donde has venido y a donde has ido en ese tile
                if (t == currentTile)
                {
                    tmp.DeactiveAll();
                    int outInd = -1;
                    if (i + 1 < _flows[ind].Count)
                        outInd = (_flows[ind][i + 1].inIndex + 2) % 4;
                    tmp.ActiveInOut((tmpList[tmpList.Count - 1].outIndex + 2) % 4, outInd, tmpList[tmpList.Count - 1].getColor());
                }
                tmpList.Add(tmp);
            }

            _tmpFlows.Add(tmpList);
        }

        /// <summary>
        /// Desactiva si una tuberia a cortado a otra
        /// </summary>
        private void DeactivateByColor(Logic.Directions dir)
        {
            int ind = 0;
            while (!_flows[ind].Contains(currentTile) )
            {
                ind++;
            }
            CutFlow(ind);

            int index = _flows[ind].IndexOf(currentTile);

            bool halfUp = _flows[ind].Count / 2 <= index;
            int i = 0;
            //te cortan por la primera mitad y estaba finalizada la tuberia
            if (!halfUp && _flows[ind][_flows[ind].Count - 1].getIsMain())
            {
                for (int j = 0; j <= index && index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].DeactiveAll();
                    _flows[ind].RemoveAt(i);
                }
                _flows[ind][0].DeactiveIn();
                _flows[ind].Reverse();
                foreach (Tile t in _flows[ind])
                {
                    t.Swap();
                }
                _tmpFlows[_tmpFlows.Count - 1].Reverse();
                foreach (Tile t in _tmpFlows[_tmpFlows.Count - 1])
                {
                    t.Swap();
                }
            }//te cortan por la segunda mitad y estaba finalizada la tuberia
            else if (_flows[ind][_flows[ind].Count - 1].getIsMain())
            {
                i = index;
                for (int j = 0; j <= index && index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].DeactiveAll();
                    _flows[ind].RemoveAt(i);

                }
                _flows[ind][_flows[ind].Count - 1].DeactiveOut();

            }//te cortan por la segunda mitad y estaba finalizada la tuberia
            else
            {
                i = index;
                for (int j = 0; index < _flows[ind].Count; j++)
                {
                    _flows[ind][i].DeactiveAll();
                    _flows[ind].RemoveAt(i);

                }
                _flows[ind][_flows[ind].Count - 1].DeactiveOut();
            }

            _flows[_flowsIndex].Add(currentTile);
            currentTile.modify(dir, true, pressedColor);
            colorConflict = false;
        }

        /// <summary>
        /// Reactiva un camino que se habia cortado durante ese mismo movimiento
        /// </summary>
        /// <param name="toCheck"></param>
        private void ReactivateFlow(List<Tile> toCheck)
        {
            for (int i = _tmpFlows.Count - 1; i >= 0; --i)
            {
                List<Tile> list = _tmpFlows[i];
                Debug.Log(toCheck.Count);
                bool reactivate = false;
                foreach (Tile tile in toCheck)
                {
                    foreach (Tile tmp in list)
                    {
                        if (tmp.getBoardPos() == tile.getBoardPos())
                        {
                            int ind = GetColorIndex(list[0].getColor());
                            int j = _flows[ind].Count - 1;
                            Tile t = list[j];
                            Tile boardTile = _tiles[t.getBoardPos().x, t.getBoardPos().y];

                            boardTile.resetTile(t.inIndex, t.outIndex, t.getColor());
                            j++;
                            for (; j < list.Count; ++j)
                            {
                                if (!_flows[_flowsIndex].Contains(list[j]))
                                {
                                    t = list[j];
                                    boardTile = _tiles[t.getBoardPos().x, t.getBoardPos().y];

                                    boardTile.resetTile(t.inIndex, t.outIndex, t.getColor());
                                    _flows[ind].Add(boardTile);
                                }
                                else
                                {
                                    _flows[ind][_flows[ind].Count - 1].DeactiveOut();
                                    break;
                                }
                            }
                            // pistas
                            if (CheckIfHintedFlow(ind))
                            {
                                PutStars(ind, true);
                            }
                            reactivate = true;
                             
                        }
                        
                    }
                    if (reactivate)
                        break;

                }

            }

        }

        /// <summary>
        /// Desactiva si una tuberia se ha cortado a si misma
        /// </summary>
        /// <param name="dir"> La direccion en la que se ha movido</param>
        private void DeactivateItSelf(Logic.Directions dir)
        {
            int index = _flows[_flowsIndex].IndexOf(currentTile);

            List<Tile> toCheck = new List<Tile>();
            int i = index + 1;
            while( i < _flows[_flowsIndex].Count)
            {
                _flows[_flowsIndex][i].DeactiveAll();
                toCheck.Add(_flows[_flowsIndex][i]);
                _flows[_flowsIndex].RemoveAt(i);
            }
            if (dir != Logic.Directions.None)
            {
                toCheck.Reverse();
                ReactivateFlow(toCheck);
            }

            if (!_flows[_flowsIndex][index].getIsMain())
                _flows[_flowsIndex][index].NotDeactiveIn();
            if (_flows[_flowsIndex].Count == 1)
            {
                _flows[_flowsIndex][0].DeactiveAll();
                _flows[_flowsIndex].RemoveAt(0);
            }
        }

        /// <summary>
        /// Desactiva el camino de una tuberia
        /// </summary>
        /// <param name="dir"> La direccion en la que se ha ido</param>
        private void Deactivate(Logic.Directions dir)
        {

            if (lastConnectedTile.getColor() == currentTile.getColor() && !colorConflict)
            {
                DeactivateItSelf(dir);
            }
            else//un color a cortado a otro
            {
                DeactivateByColor(dir);
            }
            CheckFlows();
            changes = true;
        }


        // Comprueba si hay que cambiar el color de una tuberia
        private bool CanActivate()
        {
            return lastConnectedTile != null && currentTile != lastConnectedTile &&
                (!currentTile.getIsMain() ||
                currentTile.getIsMain() && currentTile.getColor() == pressedColor)
                && lastConnectedTile.getColor() != Color.black
                && !IsThereWall(lastConnectedTile, currentTile);
        }

        bool IsThereWall(Tile AT, Tile BT)
        {
            bool ret = false;

            Vector2Int A = AT.getBoardPos();
            Vector2Int B = BT.getBoardPos();
            if (A.x == B.x - 1 && B.y == B.y) //A|B
                ret = _tiles[A.x, A.y].getActiveWall(Logic.Directions.Right) || _tiles[B.x, B.y].getActiveWall(Logic.Directions.Left);
            else if (A.x == B.x + 1 && A.y == B.y)  //B|A
                ret = _tiles[A.x, A.y].getActiveWall(Logic.Directions.Left) || _tiles[B.x, B.y].getActiveWall(Logic.Directions.Right);
            else if (A.y == B.y + 1 && A.x == B.x)  //B/A
                ret = _tiles[A.x, A.y].getActiveWall(Logic.Directions.Up) || _tiles[B.x, B.y].getActiveWall(Logic.Directions.Down);
            else if (A.y == B.y - 1 && A.x == B.x)  //A/B
                ret = _tiles[A.x, A.y].getActiveWall(Logic.Directions.Down) || _tiles[B.x, B.y].getActiveWall(Logic.Directions.Up);

            return ret;
        }

        void ProcessTileChange()
        {
            if (CanActivate())
            {
                if (_flows[_flowsIndex].Count == 0)
                    _flows[_flowsIndex].Add(lastConnectedTile);
                if (lastConnectedTile != _flows[_flowsIndex][0] && lastConnectedTile.getIsMain() && _flows[_flowsIndex].Contains(lastConnectedTile) && !_flows[_flowsIndex].Contains(currentTile))
                    return;
                if(_circleFlowsIndex != -1 && _flows[_circleFlowsIndex].Count > 1)
                    _flows[_circleFlowsIndex][_flows[_circleFlowsIndex].Count - 1].SmallCircleSetActive(false);
                int i = 0;
                while (i < _flows.Length)
                {
                    if (_flows[i].Contains(lastConnectedTile))
                        break;
                    i++;
                }
                changes = true;
                _flowsIndex = i;
                bool deActivate = false;
                // Compare positions
                Vector2Int lastPos = lastConnectedTile.getBoardPos(),
                    newPos = currentTile.getBoardPos();

                legalMove = false;

                Logic.Directions fromDir = Logic.Directions.Right;
                if ((lastPos.x == newPos.x - 1 && lastPos.y == newPos.y) || _flows[_flowsIndex].Contains(currentTile))//has ido a la derecha
                {
                    lastConnectedTile.modify( Logic.Directions.Right, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Left, true, pressedColor);
                    legalMove = true;
                }
                else if ((lastPos.x == newPos.x + 1 && lastPos.y == newPos.y) || _flows[_flowsIndex].Contains(currentTile))//has ido a la izquierda
                {
                    lastConnectedTile.modify( Logic.Directions.Left, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Right, true, pressedColor);
                    legalMove = true;

                }
                else if ((lastPos.y == newPos.y + 1 && lastPos.x == newPos.x) || _flows[_flowsIndex].Contains(currentTile))//has ido arriba
                {
                    lastConnectedTile.modify( Logic.Directions.Up, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Down, true, pressedColor);
                    legalMove = true;
                }
                else if ((lastPos.y == newPos.y - 1 && lastPos.x == newPos.x) || _flows[_flowsIndex].Contains(currentTile))//has ido abajo
                {
                    lastConnectedTile.modify( Logic.Directions.Down, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir = Logic.Directions.Up, true, pressedColor);
                    legalMove = true;
                }

                if (currentTile.getIsMain() && _flows[_flowsIndex].Count > 1)
                    currentTile.Swap();

                // Si entra aqui es que la tile tenia un color diferente al nuevo
                // y hay que "borrar" el camino que tenia esa tuberia, se ha cortado el flujo
                if (deActivate)
                {
                    Deactivate(fromDir);
                    lastConnectedTile = currentTile;
                }
                else if (legalMove)
                {
                    if (!_flows[_flowsIndex].Contains(currentTile))
                    {
                        _flows[_flowsIndex].Add(currentTile);
                        numPipes++;
                        if (currentTile.getIsMain())
                            CheckFlows();
                    }
                    else
                    {
                        lastConnectedTile.DeactiveIn();
                        if (!currentTile.getIsMain())
                            currentTile.NotDeactiveIn();
                        else
                            currentTile.DeactiveAll();
                        _flows[_flowsIndex].Remove(lastConnectedTile);

                    }
                    lastConnectedTile = currentTile;
                }
                CheckPipes();
            }

        }

        private void DragInput(Vector3 pos)
        {
            PorcessDragInput(pos);
            ProcessTileChange();
        }

        private void Update()
        {
            switch (gameState)
            {
                case GamePhase.begin:
                    {
                        Vector3 tmp = transform.localScale;
                        tmp.x += _baseRatio.x * Time.deltaTime;
                        transform.localScale = tmp;

                        tmp = transform.localPosition;
                        tmp.x += (vectorOffset.x + (0.5f * _baseRatio.x)) * Time.deltaTime;
                        transform.localPosition = tmp;

                        if (transform.localScale.x >= _baseRatio.x)
                        {
                            transform.localScale = new Vector3(_baseRatio.x, _baseRatio.y, 1);
                            transform.position = new Vector2(vectorOffset.x + (0.5f * _baseRatio.x), vectorOffset.y - (0.5f * _baseRatio.y));
                            gameState = GamePhase.mid;
                        }
                    }
                    break;
                case GamePhase.mid:
                    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        if (!win)
                        {
                            if (Input.GetMouseButtonDown(0))
                                PressInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            if (Input.GetMouseButtonUp(0))
                                ReleaseInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            if (Input.GetMouseButton(0))
                                DragInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        }
#elif UNITY_ANDROID
            if (Input.touches.Length > 0 && !win)
            {
                Touch input = Input.GetTouch(0);
                switch (input.phase)
                {
                    case TouchPhase.Began:
                        PressInput(Camera.main.ScreenToWorldPoint(input.position));
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        DragInput(Camera.main.ScreenToWorldPoint(input.position));
                        break;
                    case TouchPhase.Ended:
                        ReleaseInput(Camera.main.ScreenToWorldPoint(input.position));
                        break;
                }
            }
#endif
                    }
                    break;
                case GamePhase.end:
                    {
                        Vector3 tmp = transform.localPosition;
                        tmp.x += -(vectorOffset.x + (0.5f * _baseRatio.x)) * Time.deltaTime;
                        transform.localPosition = tmp;

                        tmp = transform.localScale;
                        tmp.x -= _baseRatio.x * Time.deltaTime;
                        transform.localScale = tmp;

                        if (tmp.x <= 0)
                        {
                            lvlManager.LevelEnded();
                            gameState = GamePhase.NONE;
                        }
                    }
                    break;
                case GamePhase.NONE:
                    break;
            }
        }

        /// <summary>
        /// Dependiendo del color, calcula indice en la lista de colores que tenemos
        /// </summary>
        /// <param name="c"> el color a checkear</param>
        /// <returns> devuelve el indice del color en nuestra lista de colores</returns>
        int GetColorIndex(Color c)
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

        /// <summary>
        /// Cuando en el panel se llama a cambiar de nivel
        /// </summary>
        public void ChangeLevel()
        {
            gameState = GamePhase.end;
            winPanel.SetActive(false);
        }

        /// <summary>
        /// Se llama al "ganar" y activarse el panel y el jugador pulsa en la cruz
        /// </summary>
        public void ContinuePlaying()
        {
            win = false;
            continuePlaying = true;
            winPanel.SetActive(false);
        }

        /// <summary>
        /// Pone una pista, se llama al pulsar el boton de pista 
        /// Comprueba que te quedan pistas y avisa al GameManager para que actualice el valor y lo guarde
        /// </summary>
        public void DoHint()
        {
            if (_hintIndexs.Count > 0  )
            {
                if (GameManager.getInstance().UseHint())
                {
                    int index = _hintIndexs[Random.Range(0, _hintIndexs.Count)]; // Color/flow aleatorio
                    _hintIndexs.Remove(index);
                    _usingHint = true;

                    //Simulamos el recorrido del flow como si estuviesemos drageando
                    if (_flows[index].Count > 0)
                    {
                        PressTile(_flows[index][0].getBoardPos());
                    }
                    
                    PressTile(map.Flows[index].flowPoints[0]);

                    for (int i = 1; i < map.Flows[index].flowPoints.Count; ++i)
                    {
                        DragTile(map.Flows[index].flowPoints[i]);
                        ProcessTileChange();
                    }

                    ReleaseInput(new Vector2(0, 0));

                    _usingHint = false;
                    CheckHints();
                }
                else
                {
#if UNITY_ANDROID
                    //Si no quedan pistas, mostrar anuncio
                    adsRewarded.ShowAd();
#endif
                }
            }
            
        }

    }
}