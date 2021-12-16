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

        int numPacks;
        public void setAttributes(Category cat, CategoryData cData, int catIndx)
        {
            categoryBackground.color = new Color(cat.categoryColor.r, cat.categoryColor.g, cat.categoryColor.b, 0.6f);
            categoryBar.color = cat.categoryColor;
            categoryName.text = cat.categoryName;
            numPacks = cat.packs.Length;

            MenuButton packButton;
            for (int i = 0; i < cat.packs.Length; i++)
            {
                packButton = Instantiate(buttonPref, transform).GetComponent<MenuButton>();
                packButton.setPackName(cat.packs[i].packName);
                packButton.setPackTextColor(cat.categoryColor);
                packButton.setPackLevels(cData.packs[i].completedLevels, cat.packs[i].getTotalLevels());
                packButton.setBlocked(cData.packs[i].blocked);
                
                int packIndex = i;
                packButton.setOnClick(() => {
                    GameManager.getInstance().setLevelPack(catIndx, packIndex);
                    Debug.Log(catIndx + " " +  packIndex);
                    GameManager.getInstance().ChangeScene("Level Select");             
                });
            }
        }
    }
}
