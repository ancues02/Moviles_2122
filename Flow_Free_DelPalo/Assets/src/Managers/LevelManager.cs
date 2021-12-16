using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class LevelManager : MonoBehaviour
    {
        [Tooltip ("El BoardManager de la escena")]
        public BoardManager board;

        [Tooltip("El script AdsInterstitial del objeto que lo contiene en la escena")]
        public AdsInterstitial adsInterstitial;

        [Tooltip("El texto del canvas del nivel")]
        public Text levelText;

        [Tooltip("El texto del canvas del tamanio del tablero")]
        public Text sizeText;

        [Tooltip("Boton de siguiente nivel de la parte inferior del canvas")]
        public Button nextLevelButton;

        [Tooltip("Boton de nivel anterior de la parte inferior del canvas")]
        public Button prevLevelButton;

        [Tooltip ("Color de los botones desactivados")]
        [SerializeField]
        Color disableColor;

        private void Start()
        {
            if (!board )
                Debug.LogError("Falta referencia BoardManager en LevelManager");

            if(!adsInterstitial || !sizeText || !levelText || !prevLevelButton || !nextLevelButton)
                Debug.LogWarning("Falta alguna referencia en LevelManager");

        }
        public void LevelEnded()
        {
#if UNITY_ANDROID
            if(adsInterstitial)
                adsInterstitial.ShowAd();
#elif UNITY_STANDALONE_WIN
            NextLevel();
#endif
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

        /// <summary>
        /// Pone los valores iniciales a textos y botones del canvas
        /// </summary>
        /// <param name="levelNumber"> el nivel que se esta jugando</param>
        /// <param name="width"> el ancho del tablero</param>
        /// <param name="height"> el alto del tablero</param>
        /// <param name="firstLevelPack"> si hay nivel anterior</param>
        /// <param name="lastLevelPack"> si hay nivel siguiente</param>
        public void InitialParams(int levelNumber, int width, int height, bool firstLevelPack ,bool lastLevelPack)
        {
            levelText.text = "Level" + levelNumber;
            sizeText.text = width + "x" +height;
            if (firstLevelPack)
            {
                prevLevelButton.interactable = false;
                prevLevelButton.GetComponent<Image>().color = disableColor;
            }
            if (lastLevelPack)
            {
                nextLevelButton.interactable = false;
                nextLevelButton.GetComponent<Image>().color = disableColor;
            }
        }

    }
}