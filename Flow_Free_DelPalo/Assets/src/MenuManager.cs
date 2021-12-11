using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class MenuManager : MonoBehaviour
    {
        public int categoryTest;
        public int packTest;

        void Start()
        {
        }

        private void Update()
        {
            // Gestiona la seleccion de categoria y pack dentro de la categoria
            // ademas del scroll vertical

            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager gm = GameManager.getInstance();
                gm.setLevelPack(categoryTest, packTest);
                gm.ChangeScene("LevelSelectionTest");
            }
        }
    }
}