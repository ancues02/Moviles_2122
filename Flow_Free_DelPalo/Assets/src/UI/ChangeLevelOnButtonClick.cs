using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Cambia al nivel anterior o posterior l pulsar el boton
     */
    public class ChangeLevelOnButtonClick : MonoBehaviour
    {
        public enum ChangeLevelType { Next, Previous};
        [SerializeField]
        ChangeLevelType type;
        [SerializeField]
        LevelManager lvlManager;
        public Button button;
        void Awake()
        {
            button.onClick.AddListener(() => {
                switch (type)
                {
                    case ChangeLevelType.Next:
                        lvlManager.ChangeLevel(true);
                        break;
                    case ChangeLevelType.Previous:
                        lvlManager.ChangeLevel(false);
                        break;
                }
            });
        }
    }
}
