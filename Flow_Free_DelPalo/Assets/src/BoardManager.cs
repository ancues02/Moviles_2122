using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class BoardManager : MonoBehaviour
    {

        private Tile[,] _tiles;
        private int _width, _height;
        private Logic.Map map;

        public void setMap(Logic.Map m)
        {
            map = m;   
        }

        void Start()
        {
            
        }

    }
}