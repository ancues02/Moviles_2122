using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class CoroutineAnimator
    {
        // vibración del círculo main
        public static IEnumerator Wiggle(SpriteRenderer wiggled, System.Action animEnded = null)
        {
            float i = 0;
            Vector3 oldscale = wiggled.transform.localScale;
            while (i < 1f) {
                i += Time.deltaTime;
                wiggled.transform.localScale = new Vector3(((0.2f * Mathf.Sin(6.28319f*i)) + 1f) * oldscale.x, ((0.2f * Mathf.Sin(6.28319f*i)) + 1f) * oldscale.y, oldscale.z);
                yield return null;
            }
            wiggled.transform.localScale = oldscale;

            animEnded?.Invoke();

            yield break;
        }

        // pulsación del círculo
        public static IEnumerator Pulse(SpriteRenderer pulsed, System.Action animEnded = null)
        {
            float i = 0;
            pulsed.enabled = true;
            Vector3 oldscale = pulsed.transform.localScale;
            while (i < 1f)
            {
                i += Time.deltaTime;
                pulsed.transform.localScale = new Vector3((0.5f + 2 * i) * oldscale.x, (0.5f + 2 * i) * oldscale.y, oldscale.z);
                pulsed.color = new Color(pulsed.color.r, pulsed.color.g, pulsed.color.b, 1f - i);
                yield return null;
            }
            pulsed.transform.localScale = oldscale;
            pulsed.enabled = false;

            animEnded?.Invoke();

            yield break;
        }

        // pulsación de cortado, es similar a la de acabar una tuberia pero mas rapida y con el color que le pasamos
        public static IEnumerator Pulse(SpriteRenderer pulsed, Color color,System.Action animEnded = null)
        {
            float i = 0;
            pulsed.enabled = true;
            Vector3 oldscale = pulsed.transform.localScale;
            while (i < 0.4f)
            {
                i += Time.deltaTime;
                pulsed.transform.localScale = new Vector3((0.5f + 2 * i) * oldscale.x, (0.5f + 2 * i) * oldscale.y, oldscale.z);
                pulsed.color = new Color(color.r, color.g, color.b, 1f - i);
                yield return null;
            }
            pulsed.transform.localScale = oldscale;
            pulsed.enabled = false;

            animEnded?.Invoke();

            yield break;
        }
    }
}
