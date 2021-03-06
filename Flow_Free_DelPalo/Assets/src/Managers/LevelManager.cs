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

        [Tooltip("El script AdsManager del objeto que lo contiene en la escena")]
        public AdsManager adManager;

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


        public ChangeStateSprite completeLevelImage;

        private void Start()
        {
            if (!board )
                Debug.LogError("Falta referencia BoardManager en LevelManager");

            if(!adManager || !sizeText || !levelText || !prevLevelButton || !nextLevelButton)
                Debug.LogWarning("Falta alguna referencia en LevelManager");

        }
        public void LevelEnded()
        {
            if (adManager)
                adManager.ShowInterstitialAd();
        }

        
        /// <summary>
        /// Avisa al boardManager para que haga su animacion de desaparecer y poder cambiar de nivel
        /// </summary>
        /// <param name="next"></param>
        public void ChangeLevel(bool next)
        {
            board.ChangeLevel(next);
        }
        /// <summary>
        /// Avisa al GameManager para cambiar de nivel
        /// Avisa al BoardManager para que no haga mas cosas
        /// </summary>
        public void NextLevel()
        {            
            GameManager gm = GameManager.getInstance();
            board.Stop();
            if (gm.NextLevel())
                gm.ChangeScene("Game Board");
            else
                gm.ChangeScene("Menu");
        }

        /// <summary>
        /// Cambia el nivel anterior, avisa al BoardManager para que no haga nada mas
        /// </summary>
        public void PreviousLevel()
        {
            GameManager gm = GameManager.getInstance();
            board.Stop();
            gm.PrevLevel();
            gm.ChangeScene("Game Board");
        }

        /// <summary>
        /// Pone los valores iniciales a textos y botones del canvas
        /// </summary>
        /// <param name="levelNumber"> el nivel que se esta jugando</param>
        /// <param name="width"> el ancho del tablero</param>
        /// <param name="height"> el alto del tablero</param>
        /// <param name="flowNomber"> el numero de flows en el nivel</param>
        /// <param name="firstLevelPack"> si hay nivel anterior</param>
        /// <param name="lastLevelPack"> si hay nivel siguiente</param>
        /// <param name="bestMove"> El mejor movimiento en ese nivel</param>
        public void InitialParams(int levelNumber, int width, int height, int flowNomber,bool firstLevelPack, bool lastLevelPack, int bestMove, Color categoryColor)
        {
            levelText.text = "Level" + levelNumber;
            levelText.color = categoryColor;
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
            board.best = bestMove;
            if (flowNomber == bestMove)
            {
                completeLevelImage.setPerfect();
            }
            else if (flowNomber < bestMove)
            {
                completeLevelImage.setComplete();
            }
            else
                completeLevelImage.setNone();

        }

        /// <summary>
        /// Metodo que se llama cuando se pide una pista y no quedan
        /// Se podria mostrar un panel con lo que quiera hacer el jugador (en el juego es comprar pistas)
        /// pero lanzamos directamente un anuncio
        /// </summary>
        void NoHints()
        {
            adManager.ShowRewardedAd();
        }

        public void DoHint()
        {
            if (GameManager.getInstance().GetHints() > 0)
            {
                board.DoHint();
            }
            else
                NoHints();
        }

    }
}