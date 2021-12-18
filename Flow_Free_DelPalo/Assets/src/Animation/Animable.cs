using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animable : MonoBehaviour
{
    private bool _animationAvailable = true;    // Indica si no hay una animacion en curso

    /// <summary>
    /// Hace que el sprite del objeto vibre.
    /// </summary>
    public void Wiggle()
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Wiggle(GetComponent<SpriteRenderer>(), setAvailable));
        }
    }

    /// <summary>
    /// Hace que el sprite del objeto haga una pulsacion.
    /// </summary>
    public void Pulse()
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Pulse(GetComponent<SpriteRenderer>(), setAvailable));
        }
    }

    /// <summary>
    /// Hace que el sprite del objeto haga una pulsacion.
    /// </summary>
    /// <param name="color">Color inicial del sprite</param>
    public void Pulse(Color color)
    {
        if (_animationAvailable)
        {
            _animationAvailable = false;
            StartCoroutine(FlowFree.CoroutineAnimator.Pulse(GetComponent<SpriteRenderer>(), color,setAvailable));
        }
    }

    /// <summary>
    /// Deja libre la animacion del objeto para que otras se puedan reproducir
    /// </summary>
    void setAvailable() { _animationAvailable = true; }
}
