using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        public BoardManager board;
        
        public void nextLevel()
        {
            GameManager gm = GameManager.getInstance();
            gm.nextLevel();
            gm.ChangeScene("Game Board");
        }

        public void previousLevel()
        {

            GameManager gm = GameManager.getInstance();

            gm.prevLevel();
            gm.ChangeScene("Game Board");
        }
        

        void Update()
        {
            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                nextLevel();

            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                previousLevel();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager gm = GameManager.getInstance();
                
                gm.ChangeScene("Game Board");

            }
        }
    }
}