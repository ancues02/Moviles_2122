using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class MenuButton : MonoBehaviour
    {
        public Button button;
        public Text packNameText;
        public void setPackTextColor(Color c)
        {
            packNameText.color = c;
        }
    }
}
