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
        Vector2 vectorOffset;

        //numero de pistas usadas en este nivel
        int numHintUsed = 0;

        int totalFlows, moves;
        public int best { get; set; }

        bool win = false;
        bool continuePlaying = false;
        bool stop = false;

        enum GamePhase { begin, mid, end, NONE };
        GamePhase gameState = GamePhase.NONE;

        private List<Color> colors;

        public List<Animable> _compleet;
        public Transform _compleetParent;

        Logic.LogicGame lg;

        private void Start()
        {
            if (!flowText || !movesText || !pipesText || !hintText
                || !TilePrefab || !lvlManager || !winPanel || !adsRewarded)
                Debug.LogError("Falta alguna referencia en BoardManager!");
            else
            {
                moves = 0;
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
                float ratio = GetVerticalRatio();
                y *= ratio; // Lo de la UI? No sé como vamos a hacerlo
                _cameraSize = new Vector2(x, y);
            }
            else Debug.LogError("No hay cámara, ¿qué esperabas que pasara?");
        }

        float GetVerticalRatio()
        {
            float sWidth = Screen.width;    // resolución de la pantalla
            float cWidth = _canvasSize.GetComponent<CanvasScaler>().referenceResolution.x;  // resolución del canvas base
            float ratioed = sWidth / cWidth;    // Ratio
            float tHeight = _topSize.rect.height * ratioed; // De base a actual
            float bHeight = _botSize.rect.height * ratioed; // Ditto
            float total = Screen.height - (tHeight + bHeight);  // Píxeles libres
            return total / Screen.height;   // Porcentaje libre
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
            

            for (int i = 0; i < m.Width; ++i)
            {
                for (int j = 0; j < m.Height; ++j)
                {
                    _tiles[i, j] = Instantiate(TilePrefab, new Vector2(i, -j), Quaternion.identity, transform).GetComponent<Tile>();
                    _tiles[i, j].name = $"Tile {i} {j}";
                    _tiles[i, j].setVisible(false);
                    _tiles[i, j].setBoardPos(new Vector2Int(i, j));
                    _tiles[i, j].ChangeColor(Color.black);
                }
            }

            foreach (int tile in map.Voids)
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

            foreach (Tile t in _tiles)
            {
                Vector2Int tPos = t.getBoardPos();
                if (!t.getIsVoid() && ((tPos.x - 1 < 0 && m.isBordered()) || (tPos.x - 1 >= 0 && _tiles[tPos.x - 1, tPos.y].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Left);
                if (!t.getIsVoid() && ((tPos.x + 1 >= m.Width && m.isBordered()) || (tPos.x + 1 < m.Width && _tiles[tPos.x + 1, tPos.y].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Right);
                if (!t.getIsVoid() && ((tPos.y - 1 < 0 && m.isBordered()) || (tPos.y - 1 >= 0 && _tiles[tPos.x, tPos.y - 1].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Up);
                if (!t.getIsVoid() && ((tPos.y + 1 >= m.Height && m.isBordered()) || (tPos.y + 1 < m.Height && _tiles[tPos.x, tPos.y + 1].getIsVoid())))
                    t.setActiveWall(Logic.Directions.Down);
            }

            totalFlows = map.FlowNumber;
            //totalNumPipes = m.Width * m.Height;
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

            //flowText.text = "flows: 0/" + totalFlows;
            lg = new Logic.LogicGame(_tiles, _width, _height, map, colors, _baseRatio, vectorOffset, pointer);
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
        // les pone su color y tambien en los a?adimos a los flows del boardManager
        private void setMainTiles()
        {
            List<Logic.Map.Flow> flows = map.Flows;
            int i = 0;
            foreach (Logic.Map.Flow f in map.Flows)
            {
                _tiles[f.start.x, f.start.y].ChangeColor(colors[i]);
                _tiles[f.start.x, f.start.y].setIsMain(true);
                _tiles[f.start.x, f.start.y].setVisible(true);
                

                _tiles[f.end.x, f.end.y].ChangeColor(colors[i]);
                _tiles[f.end.x, f.end.y].setIsMain(true);
                _tiles[f.end.x, f.end.y].setVisible(true);

                i++;
            }
        }

        /// <summary>
        /// Modifica el texto de pipes del canvas
        /// </summary>
        private void CheckPipes()
        {
           pipesText.text = "pipe: " + lg.CheckPipes() + "%";
        }

        /// <summary>
        /// Modifica el texto de flows del canvas
        /// </summary>
        private void CheckFlows()
        {
            flowText.text = "flows: " + lg.CheckFlows() + "/" + totalFlows;
        }

        /// <summary>
        /// Modifica el texto de moves del canvas
        /// </summary>
        private void CheckMoves()
        {
            movesText.text = "moves:" + lg.CheckMoves() + " best: ";
            movesText.text += best == -1 ? "-" : best + "";
        }

        /// <summary>
        /// Modifica el texto de pistas del canvas
        /// </summary>
        public void CheckHints()
        {
            hintText.text = GameManager.getInstance().GetHints() + " x ";
        }

        private void CheckTexts()
        {
            CheckMoves();
            CheckFlows();
            CheckPipes();
        }

        public void Stop()
        {
            stop = true;
        }

        private void Win()
        {
            if (!stop)
            {
                winPanel.SetActive(true);
                bool perfect = lg.WinAttributes(out moves);
                GameManager.getInstance().LevelComplete(moves);
                panelMovesText.text = "You completed the level\n" + " in " + moves + " moves";
                foreach (Tile t in _tiles)
                    if (t.getIsMain()) t.animableSprites[1].Pulse();

                if (perfect) _compleet[0].Pulse();
                else _compleet[1].Pulse();
            }
        }


        /// <summary>
        /// Pone una pista, se llama al pulsar el boton de pista 
        /// Comprueba que te quedan pistas y avisa al GameManager para que actualice el valor y lo guarde
        /// Tambien descartamos que si el nivel tiene 4 flows y has pedido 4 pistas, si pides otra no te quite pistas
        /// </summary>
        public void DoHint()
        {
            if (numHintUsed < totalFlows)
            {
                if (GameManager.getInstance().UseHint())
                {
                    numHintUsed++;
                    if (lg.DoHint())
                    {
                        win = true;
                        Win();
                    }
                }
                else
                {
                    //Si no quedan pistas, mostrar anuncio
                    adsRewarded.ShowAd();
                }
            }
            CheckHints();
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

                            _compleetParent.transform.Translate(-new Vector2(vectorOffset.x + (0.5f * _baseRatio.x),
                                vectorOffset.y - (0.5f * _baseRatio.y)));

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
                                lg.PressInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            else if (Input.GetMouseButtonUp(0))
                            {
                                if (!continuePlaying)
                                    win = lg.ReleaseInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                                else
                                    continuePlaying = false;
                            }
                            else if (Input.GetMouseButton(0))
                                lg.DragInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                            
                            CheckTexts();
                            if (win)
                            {
                                Win();
                            }
                        }
#elif UNITY_ANDROID
                        if (Input.touches.Length > 0 && !win)
                        {
                            Touch input = Input.GetTouch(0);
                            switch (input.phase)
                            {
                                case TouchPhase.Began:
                                    lg.PressInput(Camera.main.ScreenToWorldPoint(input.position));
                                    break;
                                case TouchPhase.Moved:
                                case TouchPhase.Stationary:
                                    lg.DragInput(Camera.main.ScreenToWorldPoint(input.position));
                                    break;
                                case TouchPhase.Ended:
                                    if (!continuePlaying)
                                        win = lg.ReleaseInput(Camera.main.ScreenToWorldPoint(input.position));
                                    else
                                        continuePlaying = false;
                                    break;
                            }
                            CheckTexts();
                            if (win)
                            {
                                Win();
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

    }
}