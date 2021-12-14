using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class TileAnimations : MonoBehaviour
    {
        [Tooltip("Círculo límite del flujo")]
        public SpriteRenderer MainCirc;

        [Tooltip("Círculo pulsación desde main")]
        public SpriteRenderer PulseCircle;

        [Tooltip("Círculo pequeño")]
        public SpriteRenderer SmallCircle;

        public void Wiggle()
        {
            StopCoroutine("WiggleCoroutine");
            StartCoroutine("WiggleCoroutine");
        }

        public void SmallWiggle()
        {
            StopCoroutine("SmallWiggleCoroutine");
            StartCoroutine("SmallWiggleCoroutine");
        }

        public void Pulse()
        {
            StopCoroutine("PulseCoroutine");
            StartCoroutine("PulseCoroutine");
        }

        // vibración del círculo main
        IEnumerator WiggleCoroutine()
        {
            float i = 0;
            Vector3 oldscale = MainCirc.transform.localScale;
            while (i < 1f) {
                i += Time.deltaTime;
                MainCirc.transform.localScale = new Vector3((0.2f * Mathf.Sin(6.28319f*i)) + 1f, (0.2f * Mathf.Sin(6.28319f*i)) + 1f, 1);
                yield return null;
            }
            MainCirc.transform.localScale = oldscale;
            yield break;
        }

        // vibración del círculo main
        IEnumerator SmallWiggleCoroutine()
        {
            float i = 0;
            Vector3 oldscale = SmallCircle.transform.localScale;
            while (i < 1f)
            {
                i += Time.deltaTime;
                SmallCircle.transform.localScale = new Vector3(((0.2f * Mathf.Sin(6.28319f * i)) + 1f) * 0.7f, ((0.2f * Mathf.Sin(6.28319f * i)) + 1f) * 0.7f, 1);
                yield return null;
            }
            SmallCircle.transform.localScale = oldscale;
            yield break;
        }

        // pulsación del círculo
        IEnumerator PulseCoroutine()
        {
            float i = 0;
            PulseCircle.gameObject.SetActive(true);
            while (i < 1f)
            {
                i += Time.deltaTime;
                PulseCircle.transform.localScale = new Vector3(0.5f + 2 * i, 0.5f + 2 * i, 1);
                PulseCircle.color = new Color(MainCirc.color.r, MainCirc.color.g, MainCirc.color.b, 1f - i);
                yield return null;
            }
            PulseCircle.gameObject.SetActive(false);
            yield break;
        }
    }
}
