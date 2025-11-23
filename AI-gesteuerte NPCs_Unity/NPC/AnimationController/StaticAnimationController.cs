using System.Collections;
using System.Collections.Generic;
using IAnimation;
using MouthSystem;
using UnityEngine;



namespace AnimationController
{

    // This implements the interface for static animation 
    public class StaticAnimationController : MonoBehaviour, IStaticAnimation
    {

        private MouthSystem.MouthMovement _mouthMovement;


        void Start()
        {
            _mouthMovement = GetComponent<MouthMovement>();
        }

        public void Idle()
        {
            Debug.Log("Hello Test Idle");
        }

        public void Wave()
        {
            Debug.Log("Hello Test Wave");
        }

        public void MouthMovement(bool isSpeaking)
        {   // Checks if the mouth movement exists
            if (_mouthMovement != null)
            {
                _mouthMovement.SetSpeaking(isSpeaking);
            }
            else
            {
                Debug.Log("Mouth Movement is not initialized!");
            }
        }

        void IStaticAnimation.Mouthmovement(bool isSpeaking)
        {
            throw new System.NotImplementedException();
        }
    }

}
