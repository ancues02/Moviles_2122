using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Tile : MonoBehaviour, System.ICloneable
    {
        public Tile(Tile t)
        {
            isMain = t.isMain;
            boardPos = t.boardPos;
            tileColor = t.tileColor;
            inIndex = t.inIndex;
            outIndex = t.outIndex;
        }

        Vector2Int boardPos;

        Color tileColor;

        bool isVoid = false;

        bool isMain = false;

        public List<Sprite> sprites;

        public Sprite hintedSprite;

        [Tooltip("Sprite del circulo")]
        public SpriteRenderer renderSprite;

        [Tooltip("Bordes del tile y circulo pequeño")]
        public GameObject[] childrens;

        [Tooltip("Caminos del tile, deben ser hijos")]
        public SpriteRenderer[] childrensPaths;

        [Tooltip("Paredes de tile, deben ser hijos")]
        public GameObject[] childrensWalls;

        //Los indices de los path de entrada o salida
        public int inIndex = -1, outIndex = -1;


        void Start()
        {
            if (renderSprite == null) 
                Debug.LogError("Espabila, que te falta una referencia");            
            if(childrensPaths.Length < 4)
                Debug.LogError("Espabila, que te faltan referencias de los paths");
            if (childrens.Length < 4)
                Debug.LogError("Espabila, que te faltan referencias de los hijos");

            renderSprite.sprite = sprites[0];
            //renderSprite.color = Color.black;
           
        }

        public bool getIsMain() { return isMain; }
        public void setIsMain(bool isFlow)
        {
            isMain = isFlow;
            renderSprite.color = tileColor;
        }

        public bool getIsVoid() { return isVoid; }

        public void setIsVoid()
        {
            isVoid = true;
            foreach(GameObject go in childrens)
                go.SetActive(false);
            renderSprite.color = Color.black;
        }

        // Cambia el color que tiene en ese momento el tile
        public void ChangeColor(Color color)
        {
            tileColor = color;
            
        }

        // Devuelve el color que tiene en ese momento el tile
        public Color getColor()
        {
            return tileColor;
        }

        public void SmallCircleSetActive(bool active)
        {
            childrens[4].GetComponent<SpriteRenderer>().color = tileColor;
            childrens[4].SetActive(active);
        }

        // Activa o desactiva el renderSprite 
        public void setVisible(bool visible)
        {
            renderSprite.enabled = visible;
        }

        // Pone la posicion que tiene el tile en el tablero
        public void setBoardPos(Vector2Int pos)
        {
            boardPos = pos;
        }

        // Devuelve la posicion del tile en el tablero
        public Vector2Int getBoardPos()
        {
            return boardPos;
        }

        // Activa la linea de arriba del tile
        public void activeTop()
        {
            childrens[3].SetActive(true);
        }

        // Activa la linea de la izquierda del tile
        public void activeLeftLimit()
        {
            childrens[2].SetActive(true);
        }

        public void setActiveWall(Logic.Directions dir)
        {
            childrensWalls[(int)dir].SetActive(true);
        }

        public bool getActiveWall(Logic.Directions dir)
        {
            return childrensWalls[(int)dir].activeSelf;
        }

        /// <summary>
        /// Activa un camino en funcion la direccion que le pasan
        /// </summary>
        /// <param name="dir"> la direccion a la que se quiere activar el camino</param>
        /// <param name="c"> el color con el que se mueve</param>
        /// <returns>Devuelve true si ha tenido que desactivar algun camino</returns>
        private bool active(Logic.Directions dir, Color c)
        {
            bool fail = false;
            switch (dir)
            {
                case Logic.Directions.Right:
                    childrensPaths[0].enabled = true;
                    if (c != tileColor)
                    {
                        inIndex = 0;
                        if (tileColor != Color.black)
                            fail = true;
                    }
                    else if (!isMain)
                    {
                        fail = true;
                    }

                    if (outIndex != -1)
                        fail = true;

                    if (tileColor != Color.black)
                        outIndex = 0;

                    childrensPaths[0].color = tileColor = c;
                    break;
                
                case Logic.Directions.Down:
                    childrensPaths[1].enabled = true;
                    if (c != tileColor)
                    {
                        inIndex = 1;
                        if (tileColor != Color.black)
                            fail = true;
                    }
                    else if (!isMain)
                    {
                        fail = true;
                    }
                    if (outIndex != -1)
                        fail = true;
                    if (tileColor != Color.black)
                        outIndex = 1;

                    childrensPaths[1].color = tileColor = c;
                    break;
                
                case Logic.Directions.Left:
                    childrensPaths[2].enabled = true;
                    if (c != tileColor)
                    {
                        inIndex = 2;
                        if (tileColor != Color.black)
                            fail = true;
                    }
                    else if (!isMain)
                    {
                        fail = true;
                    }
                    if (outIndex != -1)
                        fail = true;
                    if (tileColor != Color.black)
                        outIndex = 2;

                    childrensPaths[2].color = tileColor = c;
                    break;
                
                case Logic.Directions.Up:
                    childrensPaths[3].enabled = true;
                    if (c != tileColor)
                    {
                        inIndex = 3;
                        if (tileColor != Color.black)
                            fail = true;
                    }
                    else if (!isMain)
                    {
                        fail = true;
                    }
                    if (outIndex != -1)
                        fail = true;
                    if (tileColor != Color.black)
                        outIndex = 3;

                    childrensPaths[3].color = tileColor = c;
                    break;
                
            }
            return  fail;
        }

        /// <summary>
        /// Desactiva todos los caminos que tiene
        /// </summary>        
        public void DeactiveAll()
        {
            foreach (SpriteRenderer sp in childrensPaths)
            {
                sp.enabled = false;
                sp.color = Color.black;
            }
            if(!isMain)
                tileColor = Color.black;
            childrens[4].SetActive(false);
            //childrens[5].SetActive(false);
            inIndex = outIndex = -1;
        }


        /// <summary>
        /// Desactiva todos los caminos que tiene menos el de entrada
        /// </summary>
        public void NotDeactiveIn()
        {
            for(int i = 0; i < childrensPaths.Length; ++i)
            {
                if (i != inIndex)
                {
                    childrensPaths[i].enabled = false;
                    childrensPaths[i].color = Color.black;
                }
            }
           
            outIndex = -1;
        }

        /// <summary>
        /// Desactiva el camino de entrada
        /// </summary>
        public void DeactiveIn()
        {
            if (inIndex != -1)
            {
                childrensPaths[inIndex].enabled = false;
                childrensPaths[inIndex].color = Color.black;
                inIndex = -1;
            }
        }

        /// <summary>
        /// Activa los caminos de entrada y salida y pone el color al tile
        /// </summary>
        /// <param name="inIndex_"> el indice de entrada</param>
        /// <param name="outIndex_"> el indice de salida</param>
        /// <param name="c"> el color que se quiere poner</param>
        public void ActiveInOut(int inIndex_, int outIndex_, Color c)
        {
            tileColor = c;
            if (inIndex_ != -1 && inIndex_ < childrensPaths.Length)
            {
                inIndex = inIndex_;
                childrensPaths[inIndex].enabled = true;
                childrensPaths[inIndex].color = tileColor;
            }

            if (outIndex_ != -1 && outIndex_ < childrensPaths.Length)
            {
                outIndex = outIndex_;
                childrensPaths[outIndex].enabled = true;
                childrensPaths[outIndex].color = tileColor;
            }            

        }

        /// <summary>
        /// Intercambia sus valores de entrada y salida de camino
        /// </summary>
        public void Swap()
        {            
            int tmp = inIndex;
            inIndex = outIndex;
            outIndex = tmp;                    
        }

        /// <summary>
        /// Desactiva el camino de salida
        /// </summary>
        public void DeactiveOut()
        {
            if (outIndex != -1)
            {
                childrensPaths[outIndex].enabled = false;
                childrensPaths[outIndex].color = Color.black;
            }
            outIndex = -1;

        }

        ///// <summary>
        ///// Desactiva todos los caminos menos el que recibe
        ///// </summary>
        //public void notDeactivateAll(Logic.Directions dir)
        //{
        //    SpriteRenderer SpriteR = null;
        //    switch (dir)
        //    {
        //        case Logic.Directions.Right:
        //            SpriteR = childrensPaths[0];
        //            break;

        //        case Logic.Directions.Down:
        //            SpriteR = childrensPaths[1];
        //            break;

        //        case Logic.Directions.Left:
        //            SpriteR = childrensPaths[2];
        //            break;

        //        case Logic.Directions.Up:
        //            SpriteR = childrensPaths[3];
        //            break;

        //    }
        //    foreach (SpriteRenderer sp in childrensPaths)
        //        if(sp != SpriteR)
        //            sp.enabled = false;
        //}

        // Desactiva en esa direccion
        private void deactive(Logic.Directions dir)
        {
            switch (dir)
            {
                case Logic.Directions.Right:
                    childrensPaths[0].enabled = false;
                    break;

                case Logic.Directions.Down:
                    childrensPaths[1].enabled = false;
                    break;

                case Logic.Directions.Left:
                    childrensPaths[2].enabled = false;
                    break;

                case Logic.Directions.Up:
                    childrensPaths[3].enabled = false;
                    break;

            }
        }


        /// <summary>
        ///  Modifica un camino en funcion a la direccion que recibe.
        /// </summary>
        /// <param name="dir"> Direccion a comprobar</param>
        /// <param name="active_">  Si es true lo activa, si es false lo desactiva </param>
        /// <returns> Devuelve true si ha activado un camino y tiene que desactivar otro 
        /// ya sea por cambio de color, tres direcciones ...</returns>        
        public bool modify(Logic.Directions dir, bool active_, Color c)
        {
            if (!active_)
            {
                deactive(dir);
                return false;
            }
            else
                return active(dir, c);
        }

        public void resetTile(int inIndex_, int outIndex_, Color c)
        {
            DeactiveAll();
            tileColor = c;
            inIndex = inIndex_;
            if (inIndex != -1)
            {
                childrensPaths[inIndex].enabled = true;
                childrensPaths[inIndex].color = c;
            }
            outIndex = outIndex_;
            if(outIndex != -1)
            {
                childrensPaths[outIndex].color = c;
                childrensPaths[outIndex].enabled = true;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}