using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Cambia al nivel anterior o posterior l pulsar el boton
     */
    public class DoHintOnButtonClick : MonoBehaviour
    {
        
        [SerializeField]
        LevelManager lvlManager;
        public Button button;
        void Awake()
        {
            button.onClick.AddListener(() => {
                lvlManager.DoHint();
            });
        }
    }
}
