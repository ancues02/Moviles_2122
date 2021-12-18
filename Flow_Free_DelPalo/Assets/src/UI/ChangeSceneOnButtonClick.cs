using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    /**
     * Cambia la escena cuando se pulse el boton especificado
     */
    public class ChangeSceneOnButtonClick : MonoBehaviour
    {
        public string sceneName;
        public Button button;
        void Awake()
        {
            button.onClick.AddListener(() => {
                GameManager.getInstance().ChangeScene(sceneName);
            });
        }
    }
}
