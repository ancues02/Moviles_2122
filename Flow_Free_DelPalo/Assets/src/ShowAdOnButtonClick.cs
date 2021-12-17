using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFree
{
    public class ShowAdOnButtonClick : MonoBehaviour
    {
        public enum ADType { Rewarded, Instertitial};
        public ADType adType;
        public Button button;
        public AdsManager adManager;
        void Awake()
        {
            
            button.onClick.AddListener(() =>
            {
                //GameManager.getInstance().ChangeScene(sceneName);
                if (adType == ADType.Rewarded)
                {
                    adManager.ShowRewardedAd();
                }
                else
                    adManager.ShowInterstitialAd();
            });

        }
    }
}
