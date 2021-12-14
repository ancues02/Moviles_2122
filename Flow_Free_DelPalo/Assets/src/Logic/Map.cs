using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree.Logic
{
    public class Map
    {
        /**
         *  Recibe el mapa en forma de cadena y lo parsea a un mapa de juego
         *  
         *  Devuelve false si hay algun error
         */
        int width = -1, height = -1,    //5 = mismo ancho y alto, 5:6 = 5 de ancho y 6 de alto
            reserved = 0,   //siempre es 0
            levelNumber = -1,   // el numero del nivel
            flowNumber = -1;    // numero de flujos
        //opcionales
        List<int> bridges;  //separadas por dos puntos son sus ubicaciones
        List<int> voids;    //separadas por dos puntos son sus ubicaciones
        List<Vector2Int> walls; //separadas por dos puntos son sus ubicaciones, cada muro viene dado por dos casillas 2|9 
        bool bordered;

        List<Flow> flows;//las tuberias

        public List<Flow> Flows { get => flows; }
        public int Width { get => width; }
        public int Height { get => height; }
        public int Reserved { get => reserved;  }
        public int LevelNumber { get => levelNumber;  }
        public int FlowNumber { get => flowNumber;  }
        public List<int> Bridges { get => bridges;  }
        public List<int> Voids { get => voids; }
        public List<Vector2Int> Walls { get => walls;  }

        //Tuberia, contiene una lista de puntos que es la solucion a esa tuberia
        //tiene dos puntos, el inicio y el fin
        public struct Flow
        {
            public List<Vector2Int> flowPoints;
            public Vector2Int start, end;

        }

        //Le llega un entero y lo parsea a fila/columna del nivel
        Vector2Int parsePoint(int pos)
        {
            Vector2Int p = new Vector2Int();
            p.x = pos % Width;
            p.y = pos / Width;
            return p;
        }

        /*
         * Le llega una cadena que es el nivel
         */ 
        public bool Parse(string str)
        {
            
            /*str = "5,0,1,5;" +//, (puentes) , (vacio) 5:4:6:23, (paredes) 2|7:3|8,
                "18,17,12;" +
                "21,16,11,6;" +
                "3,4,9;" +
                "0,1,2,7,8,13,14,19,24,23,22;" +
                "20,15,10,5";*/

            string[] lvl = str.Split(';');
            levelAttributes(lvl[0]);
            flows = new List<Flow>();
            Flow flow;
            string[] flowPoints;

            for (int i= 1; i <= FlowNumber; ++i)//recorre todos los flows
            {
                flow = new Flow();
                flow.flowPoints = new List<Vector2Int>();
                flowPoints = lvl[i].Split(',');

                //inicio de una tuberia
                int tmp = int.Parse(flowPoints[0]);
                Vector2Int pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.start = pos;

                //calcular el recorrido de la tuberia
                for (int j = 1; j < flowPoints.Length - 1; ++j) {//recorre cada flow
                    tmp = int.Parse(flowPoints[j]);
                    flow.flowPoints.Add(parsePoint(tmp));
                }
                //fin de la tuberia
                tmp = int.Parse(flowPoints[flowPoints.Length - 1]);
                pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.end = pos;
                flows.Add(flow);    //añadir el nuevo flow con todas sus posiciones en el mapa              
            }
            return true;
        }

        void levelAttributes(string str)
        {
            string[] attr = str.Split(',');
            getSize(attr[0]);
            reserved = int.Parse(attr[1]);
            levelNumber = int.Parse(attr[2]);
            flowNumber = int.Parse(attr[3]);
            voids = new List<int>();
            walls = new List<Vector2Int>();
            if (attr.Length > 5)//checkear opcionales
            {
                //int.Parse(attr[4]);//puentes que no queremos
                getVoids(attr[5]);
                if (attr.Length > 6) 
                    getWalls(attr[6]);
            }
        }

        //Pone el tamanio del tablero
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

        void getVoids(string str)
        {
            string[] attr = str.Split(':');
            for(int i = 0; i < attr.Length && attr[i] != ""; ++i)
            {
                voids.Add(int.Parse(attr[i]));
            }
        }

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

        // Devuelve si está rodeado de bordes
        public bool isBordered() 
        {
            return bordered;
        }

        //devuelve si es inicio o fin de una tuberia
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