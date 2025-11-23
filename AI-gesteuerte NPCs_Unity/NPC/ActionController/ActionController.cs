using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MouthSystem;
using Movement;


public class NPCActions : MonoBehaviour, INPCAction
{

    private SplinePath _splinePath; 
    private MouthMovement _mouthMovement;    

    // to stop the SplinePath Action  
    private bool _splinePathActiv = false;     

    // Successfully retrieving the action components from the respective classes
    private void Start()
    {
        _splinePath = GetComponent<SplinePath>();
        _mouthMovement = GetComponent<MouthMovement>();

        if (_splinePath != null)
        { 
            _splinePath.SetupSpline();
        }
        else
        {
            Debug.Log("NPCActions requires SplinePath component");
        }

        if (_mouthMovement = null)
        {
            Debug.Log("NPCActions requires Mouthmovement component");
        }
    }
	
    // For actions that get repeated on a fixed time interval
    private void FixedUpdate()
    {
        if (_splinePath != null && _splinePathActiv)
        {
            _splinePath.FixedUpdate();
        }
    }


    // Move the NPC along the spline
    public void Move()
    {
        if (_splinePath != null)
        {
            _splinePathActiv = true;
        }
    }

    // Start the mouth movement (speak)
    public void Mouth()
    {
        if (_mouthMovement != null)
        {
            _mouthMovement.SetSpeaking(true);  // Start speaking
        }
    }


    // Stop the current actions
    public void StopAction()
    {
        if (_splinePath != null)
        {
            _splinePathActiv = false; // Stop the movement along the spline
        }

        if (_mouthMovement != null)
        {
            _mouthMovement.SetSpeaking(false); // Stop mouth movement (stop speaking)
        }
    }
	
}
