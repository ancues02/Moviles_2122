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
        public void setAttributes(Logic.GamePack pack, int pageIndex)
        {
            pageName.text = pack.Pages[pageIndex].name;
            LevelSelectionButton lvlButton;
            for (int j = 0; j < Page.LEVELS_PER_PAGE; j++)
            {
                int levelIndex = j + pageIndex * Page.LEVELS_PER_PAGE;
                lvlButton = Instantiate(buttonPref, buttonGroup).GetComponent<LevelSelectionButton>();
                lvlButton.setLevelNumber(pack.Maps[levelIndex].LevelNumber);
                lvlButton.setColor(pack.Pages[pageIndex].color);
                lvlButton.setBlocked(pack.LastUnlockedLevel < levelIndex);
                lvlButton.setOnClick(() => {
                    GameManager.getInstance().SetSelectedLevel(levelIndex);
                    GameManager.getInstance().ChangeScene("Game Board");
                });

                if (pack.BestMoves[levelIndex] > -1)
                {   
                    if(pack.BestMoves[levelIndex] == pack.Maps[levelIndex].FlowNumber)
                        lvlButton.setPerfect();
                    else
                        lvlButton.setComplete();
                }       
            }                  
        }
    }
}
