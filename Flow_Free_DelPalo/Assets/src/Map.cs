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
        int width = -1, height = -1,//5 = mismo ancho y alto, 5:6 = 5 de ancho y 6 de alto
            reserved = 0,//siempre es 0
            levelNumber = -1,// el numero del nivel
            flowNumber = -1; // numero de flujos
        //opcionales
        List<int> bridges;//separadas por dos puntos son sus ubicaciones
        List<int> voids;//separadas por dos puntos son sus ubicaciones
        List<int> walls;//separadas por dos puntos son sus ubicaciones, cada muro viene dado por dos casillas 2|9 


        List<Flow> flows;//las tuberias


        public int Width { get => width; }
        public int Height { get => height; }
        public int Reserved { get => reserved;  }
        public int LevelNumber { get => levelNumber;  }
        public int FlowNumber { get => flowNumber;  }
        public List<int> Bridges { get => bridges;  }
        public List<int> Voids { get => voids; }
        public List<int> Walls { get => walls;  }



        //Tuberia, contiene una lista de puntos que es la solucion a esa tuberia
        //tiene dos puntos, el inicio y el fin
        struct Flow
        {
            public List<Vector2> flowPoints;
            public Vector2 start, end;
            public Color color;
            //poner el color
            public void setColor(Color c) {
                this.color = c; 
            }
            public Color getColor() { return color; }
        }

        //Le llega un entero y lo parsea a fila/columna del nivel
        Vector2 parsePoint(int pos)
        {
            Vector2 p = new Vector2();
            p.x = pos / Width;
            p.y = pos % Height;
            return p;
        }

        /*
         * Le llega una cadena que es el nivel
         */ 
        public bool Parse(string str)
        {
            
            str = "5,0,1,5;" +
                "18,17,12;" +
                "21,16,11,6;" +
                "3,4,9;" +
                "0,1,2,7,8,13,14,19,24,23,22;" +
                "20,15,10,5";

            string[] lvl = str.Split(';');
            levelAttributes(lvl[0]);
            flows = new List<Flow>();
            Flow flow;
            string[] flowPoints;

            for (int i= 1; i <= FlowNumber; ++i)//recorre todos los flows
            {
                flow = new Flow();
                flow.flowPoints = new List<Vector2>();
                flow.color = Color.black;
                flowPoints = lvl[i].Split(',');

                //inicio de una tuberia
                int tmp = int.Parse(flowPoints[0]);
                Vector2 pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.start = pos;

                //calcular el recorrido de la tuberia
                for (int j = 1; j < flowPoints.Length-1; ++j) {//recorre cada flow
                    tmp = int.Parse(flowPoints[j]);
                    flow.flowPoints.Add(parsePoint(tmp));
                }
                //fin de la tuberia
                tmp = int.Parse(flowPoints[flowPoints.Length-1]);
                pos = parsePoint(tmp);
                flow.flowPoints.Add(pos);
                flow.end = pos;

                flows.Add(flow);//a�adir el nuevo flow con todas sus posiciones en el mapa
                
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
            if(attr.Length > 4)//checkear opcionales
            {
                
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
                height = int.Parse(attr[1]);
            }
        }

        //devuelve si es inicio o fin de una tuberia
        public bool IsFlow(int i, int j)
        {
            Vector2 index = new Vector2(i, j);
            foreach(Flow f in flows)
            {
                if (f.end == index || f.start == index)
                {
                    return true;
                }
            }
            return false;
        }

        //Pone un color a esa tuberia, 
        //si no tiene devuelve true , si ya tiene devuelve false
        //se usa para poner colores a los inicios/fines de tuberias
        public Color InitialColor(int i, int j, Color c)
        {
            Vector2 index = new Vector2(i, j);
            for (int k =0; k < flows.Count; ++k)
            {

                if (flows[k].end == index || flows[k].start == index)//si es la tuberia que estamos buscando
                {
                    if (flows[k].getColor() == Color.black)//si no se ha puesto color todavia
                    {
                        flows[k].setColor(c);
                        return c;
                    }
                    return Color.black;

                }
            }
            return Color.black;

        }
    }
}