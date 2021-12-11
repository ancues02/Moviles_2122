using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class Tile : MonoBehaviour
    {
        public enum ConnectionType
        {
            Normal,
            Wall,
            Void
        }
        struct Connection
        {
            ConnectionType[] sidesType;
        }

        Vector2Int boardPos;

        Color tileColor;

        bool isMain = false;

        public bool getIsMain() { return isMain; }
        public void setIsMain(bool isFlow) {
            isMain = isFlow;
            renderSprite.color = tileColor;
        }
        public List<Sprite> sprites;

        public Sprite hintedSprite;

        [Tooltip("Sprite del circulo")]
        public SpriteRenderer renderSprite;

        [Tooltip("Bordes del tile y circulo peque�o")]
        public GameObject[] childrens;

        [Tooltip("Caminos del tile, deben ser hijos")]
        public SpriteRenderer[] childrensPaths;


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


        // Comprueba si tiene que desactivar algun camino por cambio de color en tile
        // en caso afirmativo, lo desactiva
        // Recibe por parametro el camino que ha sido activado en ese momento
        private bool checkDifferentColor(SpriteRenderer SpriteR)
        {
            bool change = false;
            foreach(SpriteRenderer sR in childrensPaths)
            {
                if(sR != SpriteR)
                {
                    if(sR.color != Color.black && sR.color != SpriteR.color)
                    {
                        sR.enabled = false;
                        change = true;
                    }
                } 
            }
            return change;
        }

        // Comprueba si tiene que desactivar algun camino por tener varios caminos activos
        // en caso afirmativo, lo desactiva
        // Recibe por parametro el camino que ha sido activado en ese momento
        private bool checkManyPaths()
        {
            int pathCont = 0;
            foreach (SpriteRenderer sR in childrensPaths)
            {
                if (sR.enabled)
                    pathCont++;
            }
            return pathCont>2 /*|| isMain && pathCont > 1*/;
        }

        // Activa un camino en funcion la direccion que le pasan
        // Devuelve true si ha tenido que desactivar algun camino
        private bool active(Logic.Directions dir)
        {
            SpriteRenderer sR = null;
            switch (dir)
            {
                case Logic.Directions.Right:
                    childrensPaths[0].enabled = true;
                    childrensPaths[0].color = tileColor;
                    sR = childrensPaths[0];
                    break;
                
                case Logic.Directions.Down:
                    childrensPaths[1].enabled = true;
                    childrensPaths[1].color = tileColor;
                    sR = childrensPaths[1];
                    break;
                
                case Logic.Directions.Left:
                    childrensPaths[2].enabled = true;
                    childrensPaths[2].color = tileColor;
                    sR = childrensPaths[2];
                    break;
                
                case Logic.Directions.Up:
                    childrensPaths[3].enabled = true;
                    childrensPaths[3].color = tileColor;
                    sR = childrensPaths[3];
                    break;
                
            }

            return checkDifferentColor(sR); /*|| checkManyPaths()*/;
        }

        // Desactiva todos los caminos que tiene
        public void deactiveAll()
        {
           foreach(SpriteRenderer sp in childrensPaths)
                    sp.enabled = false;
            
        }

        /// <summary>
        /// Desactiva todos los caminos menos el que recibe
        /// </summary>
        public void notDeactivateAll(Logic.Directions dir)
        {
            SpriteRenderer SpriteR = null;
            switch (dir)
            {
                case Logic.Directions.Right:
                    SpriteR = childrensPaths[0];
                    break;

                case Logic.Directions.Down:
                    SpriteR = childrensPaths[1];
                    break;

                case Logic.Directions.Left:
                    SpriteR = childrensPaths[2];
                    break;

                case Logic.Directions.Up:
                    SpriteR = childrensPaths[3];
                    break;

            }
            foreach (SpriteRenderer sp in childrensPaths)
                if(sp != SpriteR)
                    sp.enabled = false;
        }

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
        public bool modify(Logic.Directions dir, bool active_)
        {

            if (!active_)
            {
                deactive(dir);
                return false;
            }
            else
                return active(dir);
        }


    }
}