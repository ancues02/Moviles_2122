using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree.Logic
{
    /**
    *  Mapa logico con sus datos correspondientes
    */
    public class Map
    {        
        int width = -1, height = -1,    // 5 = mismo ancho y alto, 5:6 = 5 de ancho y 6 de alto
            reserved = 0,   // siempre es 0
            levelNumber = -1,   // el numero del nivel
            flowNumber = -1;    // numero de flujos
        //opcionales
        List<int> bridges;  // separadas por dos puntos son sus ubicaciones
        List<int> voids;    // separadas por dos puntos son sus ubicaciones
        List<Vector2Int> walls; // separadas por dos puntos son sus ubicaciones, cada muro viene dado por dos casillas 2|9 
        bool bordered;  // Si esta rodeado el tablero

        List<Flow> flows;   // las tuberias

        public List<Flow> Flows { get => flows; }   // Lista de tuberias publica
        public int Width { get => width; }      // Anchura del mapa en tiles
        public int Height { get => height; }    // Altura del mapa en tiles
        public int Reserved { get => reserved;  }   // Numero reservado en el parseo
        public int LevelNumber { get => levelNumber;  } // Numero del nivel
        public int FlowNumber { get => flowNumber;  }   // Numero de flows
        public List<int> Bridges { get => bridges;  }   // Lista de puentes
        public List<int> Voids { get => voids; }        // Lista de vacios
        public List<Vector2Int> Walls { get => walls;  }    // Lista de parejas de tiles con paredes entre medias

        /**
         * Tuberia, contiene una lista de puntos que es la solucion a esa tuberia
         * tiene dos puntos, el inicio y el fin
         */
        public struct Flow
        {
            public List<Vector2Int> flowPoints;
            public Vector2Int start, end;

        }

        /// <summary>
        /// Parsea las fila y columna en nivel a partir de una posicion
        /// </summary>
        /// <param name="pos">Posicion a parsear</param>
        /// <returns>La posicion parseada</returns>
        Vector2Int parsePoint(int pos)
        {
            Vector2Int p = new Vector2Int();
            p.x = pos % Width;
            p.y = pos / Width;
            return p;
        }

        /// <summary>
        /// Parsea el nivel a partir de una cadena de texto
        /// </summary>
        /// <param name="str">Cadena de texto con los datos del nivel</param>
        /// <returns>Si el parseo ha sido correcto</returns>
        public bool Parse(string str)
        {
            string[] lvl = str.Split(';');
            levelAttributes(lvl[0]);    // Atributos iniciales y opcionales del nivel
            flows = new List<Flow>();
            Flow flow;
            string[] flowPoints;

            for (int i= 1; i <= FlowNumber; ++i)    // recorre todos los flows
            {
                flow = new Flow();
                flow.flowPoints = new List<Vector2Int>();
                flowPoints = lvl[i].Split(',');

                // inicio de una tuberia
                int tmp = int.Parse(flowPoints[0]);
                Vector2Int pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.start = pos;

                // calcular el recorrido de la tuberia
                for (int j = 1; j < flowPoints.Length - 1; ++j) {   // recorre cada flow
                    tmp = int.Parse(flowPoints[j]);
                    flow.flowPoints.Add(parsePoint(tmp));
                }

                // fin de la tuberia
                tmp = int.Parse(flowPoints[flowPoints.Length - 1]);
                pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.end = pos;
                flows.Add(flow);    // añadir el nuevo flow con todas sus posiciones en el mapa              
            }
            return true;
        }

        /// <summary>
        /// Parsea los atributos del bloque inical del nivel
        /// Datos basicos y opcionales
        /// </summary>
        /// <param name="str"></param>
        void levelAttributes(string str)
        {
            string[] attr = str.Split(',');
            getSize(attr[0]);
            reserved = int.Parse(attr[1]);
            levelNumber = int.Parse(attr[2]);
            flowNumber = int.Parse(attr[3]);
            voids = new List<int>();
            walls = new List<Vector2Int>();
            if (attr.Length > 5)    // checkear opcionales
            {
                //int.Parse(attr[4]);   // (saltandonos puentes, ya que no son necesarios todavia)
                getVoids(attr[5]);
                if (attr.Length > 6) 
                    getWalls(attr[6]);
            }
        }

        /// <summary>
        /// Pone el tamaño del tablero
        /// Y si debe estar rodeado de bordes
        /// </summary>
        /// <param name="str">Texto con los datos del tamaño</param>
        void getSize(string str)
        {
            string[] attr = str.Split(':');
            if (attr.Length == 1)
                width = height = int.Parse(attr[0]);
            else
            {
                width = int.Parse(attr[0]);
                string[] second = attr[1].Split('+');
                height = int.Parse(second[0]);
                bordered = second.Length > 1;
            }
        }

        /// <summary>
        /// Parsea las posiciones de los vacios
        /// </summary>
        /// <param name="str">Texto con los datos de los vacios</param>
        void getVoids(string str)
        {
            string[] attr = str.Split(':');
            for(int i = 0; i < attr.Length && attr[i] != ""; ++i)
            {
                voids.Add(int.Parse(attr[i]));
            }
        }

        /// <summary>
        /// Parsea los tamaños de las paredes
        /// </summary>
        /// <param name="str">Texto con los datos de las paredes</param>
        void getWalls(string str)
        {        
            string[] attr = str.Split(':');
            string[] wall;
            for (int i = 0; i < attr.Length && attr[i] != ""; ++i)
            {
                wall = attr[i].Split('|');
                walls.Add(new Vector2Int(int.Parse(wall[0]), int.Parse(wall[1])));
            }
        }

        /// <summary>
        /// Devuelve si esta rodeado de bordes
        /// </summary>
        /// <returns>Si esta rodeado de paredes</returns>
        public bool isBordered() 
        {
            return bordered;
        }

        /// <summary>
        /// Devuelve si es inicio o final de una tuberia
        /// </summary>
        /// <param name="i">Posicion en x de la tile</param>
        /// <param name="j">Posicion en y de la tile</param>
        /// <returns>Si es incio o final de tuberia</returns>
        public bool IsFlow(int i, int j)
        {
            Vector2Int index = new Vector2Int(i, j);
            foreach(Flow f in flows)
            {
                if (f.end == index || f.start == index)
                {
                    return true;
                }
            }
            return false;
        }

    }
}