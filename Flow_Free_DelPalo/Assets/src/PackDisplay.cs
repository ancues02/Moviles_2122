using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class PackDisplay : MonoBehaviour
    {
        const int LEVELS_PER_PAGE = 30;

        public GameObject buttonPref;

        public Transform buttonGroup;
        public Text pageName;
        public void setAttributes(LevelPack pack, int pageIndex)
        {
            pageName.text = pack.pages[pageIndex].name;
            LevelSelectionButton lvlButton;
            for (int j = 0; j < LEVELS_PER_PAGE; j++)
            {
                int levelIndex = j + pageIndex * LEVELS_PER_PAGE;
                lvlButton = Instantiate(buttonPref, buttonGroup).GetComponent<LevelSelectionButton>();
                lvlButton.setLevelNumber(pack.Maps[j + pageIndex * LEVELS_PER_PAGE].LevelNumber);
                lvlButton.setColor(pack.pages[pageIndex].color);
                lvlButton.setOnClick(() => {
                    GameManager.getInstance().setSelectedLevel(levelIndex);
                    GameManager.getInstance().ChangeScene("Game Board");
                });
            }                  
        }
    }
}
