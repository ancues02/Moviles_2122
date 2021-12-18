using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * TODO: Esto se puede poner en los demas botones
 * + comprobar errores
 */
namespace FlowFree
{
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
