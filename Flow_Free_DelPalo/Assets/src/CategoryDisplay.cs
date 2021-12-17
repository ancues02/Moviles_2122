using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class CategoryDisplay : MonoBehaviour
    {
        public GameObject buttonPref;
        public Text categoryName;

        public Image categoryBackground;
        public Image categoryBar;

        public void setAttributes(Logic.GameCategory cat, int catIndx)
        {
            categoryBackground.color = new Color(cat.Color.r, cat.Color.g, cat.Color.b, 0.6f);
            categoryBar.color = cat.Color;
            categoryName.text = cat.Name;

            MenuButton packButton;
            for (int i = 0; i < cat.PacksArray.Count; i++)
            {
                packButton = Instantiate(buttonPref, transform).GetComponent<MenuButton>();
                packButton.setPackName(cat.PacksArray[i].Name);
                packButton.setPackTextColor(cat.Color);
                packButton.setPackLevels(cat.PacksArray[i].CompletedLevels, cat.PacksArray[i].TotalLevels);
                packButton.setBlocked(cat.PacksArray[i].Blocked);
                
                int packIndex = i;
                packButton.setOnClick(() => {
                    GameManager.getInstance().setLevelPack(catIndx, packIndex);
                    GameManager.getInstance().ChangeScene("Level Select");             
                });

                /*if (cData.packs[i].completedLevels == cData.packs[i].totalLevels)
                {
                    packButton.set(cData.packs[i].blocked);
                }*/
            }
        }
    }
}
