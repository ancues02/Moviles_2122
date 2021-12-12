using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        public BoardManager board;

        void Update()
        {
            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager gm = GameManager.getInstance();
                gm.nextLevel();
                gm.ChangeScene("Game Board");

            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GameManager gm = GameManager.getInstance();
                
                gm.prevLevel();
                gm.ChangeScene("Game Board");

            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager gm = GameManager.getInstance();
                
                gm.ChangeScene("Game Board");

            }
        }
    }
}