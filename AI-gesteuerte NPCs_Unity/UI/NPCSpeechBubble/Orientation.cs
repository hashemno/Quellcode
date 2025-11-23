using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPCSpeechBubble
{
    public class Orientation : MonoBehaviour
    {
        new Camera camera;
        bool cameraExists = true;

        void Start()
        {
            camera = Camera.main;
            if (camera == null)
            {
                cameraExists = false;
                Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
            }
        }

        void FixedUpdate()
        {
            if (cameraExists)
            {
                // Aligning the object with the Z-axis towards the MainCamera
                transform.LookAt(camera.transform.position);
            }
        }
    }
}