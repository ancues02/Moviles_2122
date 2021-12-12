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
        public void setAttributes(Category cat)
        {
            categoryBackground.color = new Color(cat.categoryColor.r, cat.categoryColor.g, cat.categoryColor.b, 150);
            categoryBar.color = cat.categoryColor;
            categoryName.text = cat.categoryName;
            MenuButton packButton;
            foreach (LevelPack pack in cat.packs)
            {
                packButton = Instantiate(buttonPref, transform).GetComponent<MenuButton>();
                packButton.setPackTextColor(cat.categoryColor);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MenuButton packButton = Instantiate(buttonPref, transform).GetComponent<MenuButton>();
                packButton.setPackTextColor(Color.white);
            }
        }
    }
}
