using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        public BoardManager board;

        void Start()
        {
        }


        void Update()
        {
            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager gm = GameManager.getInstance();
                gm.ChangeScene("MenuTest");
            }
        }
    }
}