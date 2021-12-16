using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class PackDisplay : MonoBehaviour
    {

        public GameObject buttonPref;

        public Transform buttonGroup;
        public Text pageName;
        public void setAttributes(LevelPack pack, PackData pData, int pageIndex)
        {
            pageName.text = pack.pages[pageIndex].name;
            LevelSelectionButton lvlButton;
            for (int j = 0; j < Page.LEVELS_PER_PAGE; j++)
            {
                int levelIndex = j + pageIndex * Page.LEVELS_PER_PAGE;
                lvlButton = Instantiate(buttonPref, buttonGroup).GetComponent<LevelSelectionButton>();
                lvlButton.setLevelNumber(pack.Maps[levelIndex].LevelNumber);
                lvlButton.setColor(pack.pages[pageIndex].color);
                lvlButton.setBlocked(pData.lastUnlockedLevel < levelIndex);
                lvlButton.setOnClick(() => {
                    GameManager.getInstance().setSelectedLevel(levelIndex);
                    GameManager.getInstance().ChangeScene("Game Board");
                });

                if (pData.bestMoves[levelIndex] > -1)
                {   
                    if(pData.bestMoves[levelIndex] == pack.Maps[levelIndex].FlowNumber)
                        lvlButton.setPerfect();
                    else
                        lvlButton.setComplete();
                }       
            }                  
        }
    }
}
