using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFree
{
    public class CoroutineAnimator : MonoBehaviour
    {
        public List<SpriteRenderer> animatedSprites;

        private List<Coroutine> animations = new List<Coroutine>();

        private void Start()
        {
            Prod();
        }

        public void Prod()
        {
            for (int i = 0; i < animatedSprites.Count; i++)
                animations.Add(null);
        }

        public void PlayWiggle(int index)
        {
            if (animations[index] == null)
            {
                //StopCoroutine(animations[index]);
                SpriteRenderer rend = animatedSprites[index];
                animations[index] = StartCoroutine(Wiggle(rend));
            }
        }

        public void PlayPulse(int index)
        {
            if (animations[index] == null)
            {
                //StopCoroutine(animations[index]);
                SpriteRenderer rend = animatedSprites[index];
                animations[index] = StartCoroutine(Pulse(rend));
            }
        }

        // vibración del círculo main
        IEnumerator Wiggle(SpriteRenderer wiggled)
        {
            float i = 0;
            Vector3 oldscale = wiggled.transform.localScale;
            while (i < 1f) {
                i += Time.deltaTime;
                wiggled.transform.localScale = new Vector3(((0.2f * Mathf.Sin(6.28319f*i)) + 1f) * oldscale.x, ((0.2f * Mathf.Sin(6.28319f*i)) + 1f) * oldscale.y, oldscale.z);
                yield return null;
            }
            wiggled.transform.localScale = oldscale;

            animations[animatedSprites.IndexOf(wiggled)] = null;

            yield break;
        }

        // pulsación del círculo
        IEnumerator Pulse(SpriteRenderer pulsed)
        {
            float i = 0;
            pulsed.gameObject.SetActive(true);
            Vector3 oldscale = pulsed.transform.localScale;
            while (i < 1f)
            {
                i += Time.deltaTime;
                pulsed.transform.localScale = new Vector3((0.5f + 2 * i) * oldscale.x, (0.5f + 2 * i) * oldscale.y, oldscale.z);
                pulsed.color = new Color(pulsed.color.r, pulsed.color.g, pulsed.color.b, 1f - i);
                yield return null;
            }
            pulsed.transform.localScale = oldscale;
            pulsed.gameObject.SetActive(false);

            animations[animatedSprites.IndexOf(pulsed)] = null;

            yield break;
        }
    }
}
