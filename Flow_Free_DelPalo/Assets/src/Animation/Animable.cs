using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animable : MonoBehaviour
{
    private bool _animationAvailable = true;

    public void Wiggle()
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Wiggle(GetComponent<SpriteRenderer>(), setAvailable));
        }
    }

    public void Pulse()
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Pulse(GetComponent<SpriteRenderer>(), setAvailable));
        }
    }

    public void Pulse(Color color)
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Pulse(GetComponent<SpriteRenderer>(), color,setAvailable));
        }
    }

    void setAvailable() { _animationAvailable = true; }
}
