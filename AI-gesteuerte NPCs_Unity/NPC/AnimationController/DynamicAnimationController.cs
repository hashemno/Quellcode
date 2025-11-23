using System.Collections;
using System.Collections.Generic;
using IAnimation;
using UnityEngine;


namespace AnimationController
{


// This implements the interface for dynamic animation 
public class DynamicAnimationController : MonoBehaviour, IDynamicAnimation
{
    private Movement.SplinePath _splinePath;

    void Start()
    {
        _splinePath = GetComponent<Movement.SplinePath>();

    }

    public void Idle()
    {
        Debug.Log("Hello Test Idle");
    }

    public void Walk()
    {
        if (_splinePath != null)
        {
            _splinePath.Walk(); 
        }
    }

    public void Grab()
    {
        Debug.Log("Hello Test Grab");
    }

}

}