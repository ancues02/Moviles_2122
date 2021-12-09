using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        int levelIndex;

        void Start()
        {
            GameManager gameManager = GameManager.getInstance();
            //levelIndex = gameManager.getLevel();
        }
    }
}