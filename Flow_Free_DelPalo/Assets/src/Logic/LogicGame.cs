using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree.Logic
{
    public class LogicGame 
    {
        SpriteRenderer _pointer;
        Vector2 _baseRatio;
        Tile[,] _tiles;
        int _width, _height;
        Map _map;

        //los caminos que hay en el tablero durante el juego
        List<Tile>[] _flows;
        int _flowsIndex;//el indice de la tuberia que se esta modificando
        int _circleFlowsIndex = -1;//el indice de la tuberia que tiene el circulo pequenio

        List<List<Tile>> _tmpFlows;//flows que se han cortado mientras estas pulsando
        List<Tile> _touchFlow; //el flow que se ha tocado, para ver si ha tenido cambios y aniadir un movimiento

        Vector2 _vectorOffset;
        Color pressedColor = Color.black,
            lastMoveColor = Color.black;
        Tile currentTile;
        Tile lastConnectedTile;
        bool colorConflict = false;
        bool legalMove = false;
        bool changes = false;

        int totalFlows, totalNumPipes;
        int numFlows, moves, numPipes;

        List<Color> _colors;

        List<int> _hintIndexs;
        bool _usingHint = false;    // Para que las hints no se animen
        bool _animColor = false;    // Para que el release no haga wiggle si no tocaste 

        public LogicGame(Tile[,] tiles, int width, int height,
            Map map, List<Color> colors, Vector2 baseRatio, Vector2 vectorOffset,
            SpriteRenderer pointer)
        {
            _tiles = tiles;
            _width = width;
            _map = map;
            _height = height;
            _colors = colors;
            _pointer = pointer;
            _baseRatio = baseRatio;
            _vectorOffset = vectorOffset;
            _flows = new List<Tile>[map.FlowNumber];
            _tmpFlows = new List<List<Tile>>();
            _touchFlow = new List<Tile>();
            for (int i = 0; i< map.FlowNumber; ++i)
            {
                _flows[i] = new List<Tile>();
            }
            _hintIndexs = new List<int>();
            for (int i = 0; i < map.FlowNumber; ++i)
                _hintIndexs.Add(i);
            totalFlows = map.FlowNumber;
            totalNumPipes = _width * _height;
        }

        /// <summary>
        /// Cuando se presiona, ver si es sobre el tablero y 
        /// calcular la tile sobre la que se esta pulsando y activar el puntero en ese color
        /// </summary>
        /// <param name="pos"> La posicion en la que se ha presionado</param>
        public void PressInput(Vector2 pos)
        {
            Vector2Int boardPos = GetBoardTile(pos);
            if (boardPos.x >= 0 && boardPos.x < _width &&
                boardPos.y >= 0 && boardPos.y < _height)
            {
                PressTile(boardPos);
                if (_tiles[boardPos.x, boardPos.y].getColor() != Color.black)
                {
                    _pointer.transform.position = new Vector3(pos.x, pos.y, -2);
                    _pointer.enabled = true;
                    _pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.5f);
                }
            }
        }

        /// <summary>
        /// Presiona una tile especifica
        /// </summary>
        /// <param name="boardPos"> La posicion de la tile que se ha pulsado</param>
        void PressTile(Vector2Int boardPos)
        {
            Tile activatedTile = _tiles[boardPos.x, boardPos.y];
            if (activatedTile.getColor() != Color.black)
            {
                _animColor = true;
                pressedColor = activatedTile.getColor();
                lastConnectedTile = currentTile = activatedTile;
                _flowsIndex = GetColorIndex(currentTile.getColor());
                foreach(Tile t in _flows[_flowsIndex])
                {
                    _touchFlow.Add((Tile)t.Clone());
                }
                // pistas
                if (CheckIfHintedFlow(_flowsIndex))
                    PutStars(_flowsIndex, false);

                if (currentTile.getIsMain())
                {
                    DeactivateMain();
                    if (!_usingHint && _animColor)
                    {
                        Vector2Int target = _map.Flows[_flowsIndex].start;
                        _tiles[target.x, target.y].animableSprites[0].Wiggle();
                        target = _map.Flows[_flowsIndex].end;
                        _tiles[target.x, target.y].animableSprites[0].Wiggle();
                    }
                }
                else
                {
                    //desactivar el circulo pequenio
                    int lastInd = _flows[_flowsIndex].Count - 1;

                    //si no eres el final, desactivar el resto
                    if (_flows[_flowsIndex][lastInd] != activatedTile)
                    {
                        DeactivateItSelf(Directions.None);
                        changes = true;
                    }
                }
            }
        }

        public bool ReleaseInput(Vector2 pos)
        {
            lastConnectedTile = null;
            //Aumentar numero de movimientos si ha habido algun cambio en la tuberia que has tocado
            //y es un color diferente al anterior con el que se ha aumentado el numero de movimientos
            bool diff = false;
            if (_touchFlow.Count != _flows[_flowsIndex].Count)
                diff = true;
            else
            {
                foreach (Tile t in _touchFlow)
                {
                    if (!_flows[_flowsIndex].Contains(t))
                    {
                        diff = true;
                        break;
                    }
                }
            }
            _touchFlow.Clear();

            if (diff && lastMoveColor != pressedColor)
            {
                lastMoveColor = pressedColor;
                moves++;
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

            _pointer.enabled = false;

            // pistas
            if (CheckIfHintedFlow(_flowsIndex))
                PutStars(_flowsIndex, true);

            if (!_usingHint && _animColor && _flows[_flowsIndex].Count > 0)
                if (_flows[_flowsIndex][_flows[_flowsIndex].Count - 1].getIsMain())
                    _flows[_flowsIndex][_flows[_flowsIndex].Count - 1].animableSprites[1].Pulse();
                else
                    _flows[_flowsIndex][_flows[_flowsIndex].Count - 1].animableSprites[2].Wiggle();

           
            _animColor = false;
            return numFlows == totalFlows;
        }

        public bool WinAttributes(out int moves)
        {
            moves = this.moves;
            return moves > _flows.Length;
        }

        // Mira si es uno en el que se ha hecho una pista
        bool CheckIfHintedFlow(int flowsindex)
        {
            bool wasHinted = !_hintIndexs.Contains(flowsindex);
            bool isCorrect = _flows[flowsindex].Count == _map.Flows[flowsindex].flowPoints.Count;
            int i = 0;
            while (isCorrect && i < _flows[flowsindex].Count)
            {
                isCorrect = _map.Flows[flowsindex].flowPoints.Contains(_flows[flowsindex][i].getBoardPos());
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

        /// <summary>
        /// Procesa que se este drageando sobre el tablero de juego
        /// Consigue la Tile del tablero sobre la que se esta drageando
        /// Si se dragea sobre un main de tu mismo color se unen las tuberias, tambien se modifica el alpha del puntero 
        /// </summary>
        /// <param name="pos"> posicion en la que se esta drageando</param>
        void ProcessDragInput(Vector2 pos)
        {
            Vector2Int boardPos = GetBoardTile(pos);
            if (boardPos.x >= 0 && boardPos.x < _width &&
                boardPos.y >= 0 && boardPos.y < _height)
            {
                DragTile(boardPos);
                if (currentTile.getIsMain())
                {
                    if (currentTile.getColor() == pressedColor)
                    {
                        if (_flows[_flowsIndex].Count > 2)
                            _pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.75f);
                    }
                    else
                       _pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.25f);

                }
                else
                    _pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.5f);
            }
            else _pointer.color = new Color(pressedColor.r, pressedColor.g, pressedColor.b, 0.25f);
            _pointer.transform.position = new Vector3(pos.x, pos.y, -2);
        }

        // Hace drag sobre una tile específica
        void DragTile(Vector2Int boardPos)
        {
            currentTile = _tiles[boardPos.x, boardPos.y];
        }

        /// <summary>
        /// En funcion a la posicion que recibe, se calcula la posicion que seria respecto al tablero
        /// </summary>
        /// <param name="pos">la posicion en la que se ha pulsado</param>
        /// <returns> Devuelve la posicion en el tablero</returns>
        private Vector2Int GetBoardTile(Vector2 pos)
        {
            int x = Mathf.FloorToInt((pos - _vectorOffset).x / _baseRatio.x);
            int y = Mathf.FloorToInt(-(pos - _vectorOffset).y / _baseRatio.y);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Si se pulsa en un Tile main, se desactiva todo el flow que tenia
        /// </summary>
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
        }

        /// <summary>
        /// Modifica el texto de pipes del canvas
        /// </summary>
        public int CheckPipes()
        {
            numPipes = 0;
            for (int i = 0; i < _flows.Length; ++i)
            {
                numPipes += _flows[i].Count;
            }
            return Mathf.RoundToInt((numPipes / (float)totalNumPipes) * 100);
            
        }

        /// <summary>
        /// Modifica el texto de flows del canvas
        /// </summary>
        public int CheckFlows()
        {
            numFlows = 0;
            for (int i = 0; i < _flows.Length; ++i)
            {
                if (_flows[i].Count > 2 && _flows[i][_flows[i].Count - 1].getIsMain())
                    numFlows++;
            }
            return numFlows;
        }

        /// <summary>
        /// Modifica el texto de moves del canvas
        /// </summary>
        public int CheckMoves()
        {
            return moves;
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
                PutStars(ind, false);
            }
            //no hacer nada si ya se ha guardado durante ese drag del flow que se ha cortado la tuberia
            foreach (List<Tile> t in _tmpFlows)
            {
                if (t[0].getColor() == _flows[ind][0].getColor())
                {
                    return;
                }
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
        /// Desactiva si una tuberia a cortado a otra. Si es la primera vez cortada 
        /// en ese drag, se aniade a la lista de tuberias cortadas y se guarda su estado
        /// </summary>
        private void DeactivateByColor(Logic.Directions dir)
        {
            //encontrar el indice del flow cortado
            int ind = 0;
            while (!_flows[ind].Contains(currentTile))
            {
                ind++;
            }
            //guardar el estado de la tuberia cortada(si es necesario)
            CutFlow(ind);

            //encontrar el indice de las tuberias cortadas
            int tmpInd = 0;
            while (!_tmpFlows[tmpInd].Contains(currentTile))
            {
                tmpInd++;
            }

            int index = _flows[ind].IndexOf(currentTile);
            
            bool halfUp = _flows[ind].Count / 2 <= index;
            for (int j = 0; j < _flows[ind].Count; ++j)
            {
                Tile tmp = _flows[ind][j];
                //donde se ha cortado volver a poner bien los caminos 
                //de donde has venido y a donde has ido en ese tile
                if (tmp == currentTile)
                {

                    if (_flows[ind][_flows[ind].Count - 1].getIsMain())
                    {
                        if(halfUp && !_flows[ind][j - 1].getIsMain())
                            _flows[ind][j - 1].animableSprites[1].Pulse(_flows[ind][0].getColor());
                        else if(!_flows[ind][j + 1].getIsMain())
                            _flows[ind][j + 1].animableSprites[1].Pulse(_flows[ind][0].getColor());
                    }

                }
            }

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
                //volteamos el sentido de la tuberia
                _flows[ind].Reverse();
                foreach (Tile t in _flows[ind])
                {
                    t.Swap();
                }
                _tmpFlows[tmpInd].Reverse();
                foreach (Tile t in _tmpFlows[tmpInd])
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
        /// <param name="toCheck">Del flow que estas drageando, los tiles que estaban puestos de ese color
        /// y al cortarte a ti mismo han desaparecido</param>
        private void ReactivateFlow(List<Tile> toCheck)
        {
            //Comprobar en todos los flows cortados
            for (int i = _tmpFlows.Count - 1; i >= 0; --i)
            {
                List<Tile> list = _tmpFlows[i];
                bool reactivate = false;
                //Comprobar por todos los tiles que se han borrado y puede que antes hubiese otro flow
                foreach (Tile tile in toCheck)
                {
                    if(list.Contains(tile))
                    {
                        int ind = GetColorIndex(list[0].getColor());
                        int j = _flows[ind].Count - 1;
                        Tile t = list[j];
                        Tile boardTile = _tiles[t.getBoardPos().x, t.getBoardPos().y];

                        boardTile.resetTile(t.inIndex, t.outIndex, t.getColor());
                        j++;
                        while( j < list.Count )
                        {
                            if (!_flows[_flowsIndex].Contains(list[j]))
                            {
                                t = list[j];
                                boardTile = _tiles[t.getBoardPos().x, t.getBoardPos().y];

                                boardTile.resetTile(t.inIndex, t.outIndex, t.getColor());
                                if (!_flows[ind].Contains(boardTile))
                                    _flows[ind].Add(boardTile);
                            }
                            else
                            {
                                _flows[ind][_flows[ind].Count - 1].DeactiveOut();
                                break;
                            }
                            ++j;
                        }
                        // pistas
                        if (CheckIfHintedFlow(ind))
                        {
                            PutStars(ind, true);
                        }
                        break;
                    }
                    
                }

            }
        }

        /// <summary>
        /// Desactiva si una tuberia se ha cortado a si misma
        /// </summary>
        /// <param name="dir"> La direccion en la que se ha movido</param>
        private void DeactivateItSelf(Directions dir)
        {
            int index = _flows[_flowsIndex].IndexOf(currentTile);
            List<Tile> toCheck = new List<Tile>();
            int i = index + 1;
            while (i < _flows[_flowsIndex].Count)
            {
                _flows[_flowsIndex][i].DeactiveAll();
                toCheck.Add(_flows[_flowsIndex][i]);
                _flows[_flowsIndex].RemoveAt(i);
            }

            if (dir != Directions.None)
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
        private void Deactivate(Directions dir)
        {

            if (lastConnectedTile.getColor() == currentTile.getColor() && !colorConflict)
            {
                DeactivateItSelf(dir);
            }
            else//un color a cortado a otro
            {
                DeactivateByColor(dir);
            }
            changes = true;
        }


        // Comprueba si hay que cambiar el color de una tuberia
        /// <summary>
        /// Comprobaciones para poder dragear.
        /// Basicamente que no estes sobre un main de otro color, no estes sobre ti mismo
        /// no haya muros entre las dos tiles 
        /// </summary>
        /// <returns></returns>
        private bool CanActivate()
        {
            return lastConnectedTile != null && currentTile != lastConnectedTile &&
                (!currentTile.getIsMain() ||
                currentTile.getIsMain() && currentTile.getColor() == pressedColor)
                && lastConnectedTile.getColor() != Color.black
                && !IsThereWall(lastConnectedTile, currentTile);
        }

        /// <summary>
        /// Comprueba si hay muro
        /// </summary>
        /// <param name="AT">Posicion de una tile</param>
        /// <param name="BT">Posicion de la otra tile</param>
        /// <returns>Devuelve true si hay muro entre las dos tiles</returns>
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

        /// <summary>
        /// Procesa el drag como tal.
        /// Tiene muchas comprobaciones previas para poder dragear
        /// </summary>
        void ProcessTileChange()
        {
            if (CanActivate())
            {
                if (_flows[_flowsIndex].Count == 0)
                    _flows[_flowsIndex].Add(lastConnectedTile);
                if (lastConnectedTile != _flows[_flowsIndex][0] && lastConnectedTile.getIsMain() && _flows[_flowsIndex].Contains(lastConnectedTile) && !_flows[_flowsIndex].Contains(currentTile))
                    return;
                if (_circleFlowsIndex != -1 && _flows[_circleFlowsIndex].Count > 1)
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

                Directions fromDir = Directions.Right;
                Directions toDir = Directions.Right;
                //Comprueba en que direccion te mueves.
                //Solo dejamos mover a uno de distancia de donde estabas drageando
                if (lastPos.x == newPos.x - 1 && lastPos.y == newPos.y)//has ido a la derecha
                {
                    toDir = Directions.Right;
                    fromDir = Directions.Left;
                }
                else if (lastPos.x == newPos.x + 1 && lastPos.y == newPos.y)//has ido a la izquierda
                {
                    toDir = Directions.Left;
                    fromDir = Directions.Right;
                }
                else if (lastPos.y == newPos.y + 1 && lastPos.x == newPos.x)//has ido arriba
                {
                    toDir = Directions.Up;
                    fromDir = Directions.Down;
                }
                else if (lastPos.y == newPos.y - 1 && lastPos.x == newPos.x)//has ido abajo
                {
                    toDir = Directions.Down;
                    fromDir = Directions.Up;
                }

                //si te has movido a uno de distancia o sobre ti mismo
                if(toDir != fromDir || _flows[_flowsIndex].Contains(currentTile))
                {
                    lastConnectedTile.modify(toDir, true, pressedColor);
                    colorConflict = currentTile.getColor() != Color.black && currentTile.getColor() != pressedColor;
                    deActivate = currentTile.modify(fromDir , true, pressedColor);
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
                else if (legalMove)//si te has movido de manera "legal", es decir a una casilla adyacente o sobre un flow de tu color
                {
                    if (!_flows[_flowsIndex].Contains(currentTile))//tuberia nueva
                    {
                        _flows[_flowsIndex].Add(currentTile);
                        numPipes++;
                    }
                    else//estas cortando a tu misma tuberia
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
            }

        }

        /// <summary>
        /// Encargado de dragear, primero procesa el drag con la posicion del puntero y luego
        /// actualiza el estado del tablero en funcion a ese proceso de drag
        /// </summary>
        /// <param name="pos"></param>
        public void DragInput(Vector3 pos)
        {
            ProcessDragInput(pos);
            ProcessTileChange();
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
            while (index == -1 && i < _colors.Count)
            {
                if (c == _colors[i])
                {
                    index = i;
                    break;
                }
                ++i;
            }
            return index;
        }

        /// <summary>
        /// Pone una pista, se llama al pulsar el boton de pista 
        /// Comprueba que te quedan pistas y avisa al GameManager para que actualice el valor y lo guarde
        /// </summary>
        /// <returns> Devuelve si se ha ganado o no</returns>
        public bool DoHint()
        {
            if (_hintIndexs.Count > 0)
            {
                int index = _hintIndexs[Random.Range(0, _hintIndexs.Count)]; // Color/flow aleatorio
                _hintIndexs.Remove(index);
                _usingHint = true;

                //Simulamos el recorrido del flow como si estuviesemos drageando
                if (_flows[index].Count > 0)
                {
                    PressTile(_flows[index][0].getBoardPos());
                }

                PressTile(_map.Flows[index].flowPoints[0]);

                for (int i = 1; i < _map.Flows[index].flowPoints.Count; ++i)
                {
                    DragTile(_map.Flows[index].flowPoints[i]);
                    ProcessTileChange();
                }

                CheckFlows();
                bool win = ReleaseInput(new Vector2(0, 0));

                _usingHint = false;

                return win;
            }
            return false;
        }

    }
}