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
        public void setAttributes(Category cat, int catIndx)
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
                packButton.setPackLevels(150); // TODO: No poner esto a pelo
                int packIndex = i;
                packButton.setOnClick(() => {
                    GameManager.getInstance().setLevelPack(catIndx, packIndex);
                    GameManager.getInstance().ChangeScene("Level Select");             
                });
            }
        }

        // metodo por que no va de otra forma
        public float getActualHeight()
        {
            float aux = (((RectTransform)transform).sizeDelta.y + numPacks * ((RectTransform)buttonPref.transform).sizeDelta.y);
            return aux;
        }
    }
}
