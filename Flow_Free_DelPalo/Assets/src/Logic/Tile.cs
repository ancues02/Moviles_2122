using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    /**
    *  Tile logica con sus datos correspondientes
    */
    public class Tile : MonoBehaviour, System.ICloneable, System.IEquatable<Tile>
    {
        /// <summary>
        /// Constructor que copia los datos de otra tile
        /// </summary>
        /// <param name="t">Tile de la que se copian datos</param>
        public Tile(Tile t)
        {
            isMain = t.isMain;
            boardPos = t.boardPos;
            tileColor = t.tileColor;
            inIndex = t.inIndex;
            outIndex = t.outIndex;
        }

        Vector2Int boardPos;    // Posicion en el mapa

        Color tileColor;    // Color de la tile

        bool isVoid = false;    // Si es un vacio

        bool isMain = false;    // Si es un final o inicio de flow

        public List<Sprite> sprites;    // Sprites asocioados a la tile
        
        public List<Animable> animableSprites;  // Animables asociados a la tile

        public Sprite hintedSprite; // Sprite usado para la pista

        [Tooltip("Sprite del circulo")]
        public SpriteRenderer renderSprite;

        [Tooltip("Bordes del tile y circulo pequenio")]
        public GameObject[] childrens;

        [Tooltip("Caminos del tile, deben ser hijos")]
        public SpriteRenderer[] childrensPaths;

        [Tooltip("Paredes de tile, deben ser hijos")]
        public GameObject[] childrensWalls;

        //Los indices (direcciones) de los path de entrada o salida
        public int inIndex = -1, outIndex = -1;

        /// <summary>
        /// Comrpueba si las referncias estan en orden
        /// Asigna su sprite principal
        /// </summary>
        void Start()
        {
            if (renderSprite == null) 
                Debug.LogError("Espabila, que te falta una referencia");            
            if(childrensPaths.Length < 4)
                Debug.LogError("Espabila, que te faltan referencias de los paths");
            if (childrens.Length < 4)
                Debug.LogError("Espabila, que te faltan referencias de los hijos");

            renderSprite.sprite = sprites[0];           
        }

        /// <summary>
        /// Devuelve si es main
        /// </summary>
        /// <returns>Si es main</returns>
        public bool getIsMain() { return isMain; }

        /// <summary>
        /// Asigna si es main segun un valor dado
        /// Asigna el color que usaran a sus sprites
        /// </summary>
        /// <param name="isFlow">Si es main o no</param>
        public void setIsMain(bool isFlow)
        {
            isMain = isFlow;
            renderSprite.color = tileColor;
            childrens[6].GetComponent<SpriteRenderer>().color = tileColor;
        }

        /// <summary>
        /// Devuelve si es un vacio
        /// </summary>
        /// <returns>Si e sun vacio</returns>
        public bool getIsVoid() { return isVoid; }

        /// <summary>
        /// Asigna la tile a vacio
        /// </summary>
        public void setIsVoid()
        {
            isVoid = true;
            foreach(GameObject go in childrens)
                go.SetActive(false);
            renderSprite.color = Color.black;
        }

        /// <summary>
        /// Cambia el color asigando a la tile
        /// </summary>
        /// <param name="color">Color que se asigna</param>
        public void ChangeColor(Color color)
        {
            tileColor = color;
            
        }

        /// <summary>
        /// Devuelve el color asignado a la tile
        /// </summary>
        /// <returns>Color asignado a la tile</returns>
        public Color getColor()
        {
            return tileColor;
        }

        /// <summary>
        /// Activa o desactiva el sprite del circulo 
        /// pequenio del extremo de flow no conectado
        /// </summary>
        /// <param name="active">Si es activo o no</param>
        public void SmallCircleSetActive(bool active)
        {
            childrens[4].GetComponent<SpriteRenderer>().color = tileColor;
            childrens[4].GetComponent<SpriteRenderer>().enabled = active;
        }

        /// <summary>
        /// Cambia visibilidad del circulo 
        /// principal de la tile
        /// </summary>
        /// <param name="visible">Si es viisble o no</param>
        public void setVisible(bool visible)
        {
            renderSprite.enabled = visible;
        }

        /// <summary>
        /// Asigna la posicion en el tablero
        /// </summary>
        /// <param name="pos">Posicion en el tablero</param>
        public void setBoardPos(Vector2Int pos)
        {
            boardPos = pos;
        }

        /// <summary>
        /// Devuelve la posicion de la tile en el tablero
        /// </summary>
        /// <returns>Posicion de la tile en el tablero</returns>
        public Vector2Int getBoardPos()
        {
            return boardPos;
        }

       
        /// <summary>
        /// Activa la pared de la tile en una direccion
        /// </summary>
        /// <param name="dir">Direccion en la que se activa la pared</param>
        public void setActiveWall(Logic.Directions dir)
        {
            childrensWalls[(int)dir].SetActive(true);
        }

        /// <summary>
        /// Devuelve si una pared esta activa
        /// </summary>
        /// <param name="dir">Direccion de la pared</param>
        /// <returns>Si la pared esta activa</returns>
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
            childrensPaths[(int)dir].enabled = true;
            if (c != tileColor)
            {
                inIndex = (int)dir;
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
                outIndex = (int)dir;

            childrensPaths[(int)dir].color = tileColor = c;
            
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
            childrens[4].GetComponent<SpriteRenderer>().enabled = false;
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

        /// <summary>
        /// Desactiva el camino en una direccion
        /// </summary>
        /// <param name="dir">Direccion en la que se desactive</param>
        private void deactive(Logic.Directions dir)
        {
            childrensPaths[(int)dir].enabled = false;
        }


        /// <summary>
        /// Modifica un camino en funcion a la direccion que recibe.
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

        /// <summary>
        /// Resetea el estado de la tile, asigna los indices
        /// de entrada y salida y un nuevo color a la tile
        /// </summary>
        /// <param name="inIndex_">Indice de entrada</param>
        /// <param name="outIndex_">Indice de salida</param>
        /// <param name="c">Color a asignar</param>
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

        /// <summary>
        /// Clona la tile
        /// </summary>
        /// <returns>La tile clonada en object</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Comprueba si la esta tile y otra estan 
        /// en la misma posicion en el mapa
        /// </summary>
        /// <param name="other">Tile con la que se compara</param>
        /// <returns>Si estan ambas en la misma posicion</returns>
        public bool Equals(Tile other)
        {
            return other.getBoardPos() == boardPos;
        }
    }
}