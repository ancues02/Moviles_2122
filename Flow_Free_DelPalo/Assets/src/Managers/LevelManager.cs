using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        [Tooltip ("El BoardManager de la escena")]
        public BoardManager board;

        [Tooltip("El script AdsInterstitial del objeto que lo contiene en la escena")]
        public AdsInterstitial adsInterstitial;

        private void Start()
        {
            if (!board )
                Debug.LogError("Falta referencia BoardManager en LevelManager");
            if(!adsInterstitial)
                Debug.LogWarning("Falta referencia de adsInterstitial en LevelManager");
        }
        public void LevelEnded()
        {
            if(adsInterstitial)
                adsInterstitial.ShowAd();
        }

        public void NextLevel()
        {            
            //TODO checkear que esta desbloqueado el siguiente nivel
            GameManager gm = GameManager.getInstance();
            gm.nextLevel();
            gm.ChangeScene("Game Board");           

        }

        public void PreviousLevel()
        {
            GameManager gm = GameManager.getInstance();
            gm.prevLevel();
            gm.ChangeScene("Game Board");
        }

        //TODO QUITAR
        void Update()
        {
            // Para probar cambios de escenas
            if (Input.GetKeyDown(KeyCode.Space))
            {              
                NextLevel();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PreviousLevel();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager gm = GameManager.getInstance();               
                gm.ChangeScene("Game Board");
            }
        }
    }
}