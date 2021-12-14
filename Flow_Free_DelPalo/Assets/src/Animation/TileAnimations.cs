using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class TileAnimations : MonoBehaviour
    {
        [Tooltip("C�rculo l�mite del flujo")]
        public SpriteRenderer MainCirc;

        [Tooltip("C�rculo pulsaci�n desde main")]
        public SpriteRenderer PulseCircle;

        [Tooltip("C�rculo peque�o")]
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

        // vibraci�n del c�rculo main
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

        // vibraci�n del c�rculo main
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

        // pulsaci�n del c�rculo
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
