using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Idle
{
    public enum IdleAnimations
    {
        Idle1,
        Idle2,
        Idle3,
        Idle4,
        Idle5,
        Idle6,
        Idle7
        // Add more idle animations here if needed
    }
    public class IdleAnimationSelector : MonoBehaviour
    {
        public Animator animator;
        public IdleAnimations selectedAnimation = IdleAnimations.Idle1; //Standart Animation

        public float boredTimeThreshold = 5.0f; // Time in seconds after which the character gets bored

        private float idleTimer = 0f;
        private bool isBored = false;

        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (animator == null)
            {
                Debug.LogWarning("Animator not assigned or found.");
                return;
            }

            // Set the default idle animation at the start of the game
            SetIdleAnimation(selectedAnimation);
        }

        void Update()
        {
            // Check if the character is in an idle animation
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle 1"))
            {
                // Add time to the idleTimer
                idleTimer += Time.deltaTime;

                // If the idleTimer exceeds the boredTimeThreshold
                if (idleTimer >= boredTimeThreshold && !isBored)
                {
                    // Set the bool parameter IsBored to true
                    animator.SetBool("isBored", true);
                    isBored = true;
                }
            }
            else
            {
                // If the character is not in an idle animation, reset the timer
                idleTimer = 0f;
                isBored = false;
                animator.SetBool("isBored", false); // Ensure the bool parameter is reset
            }
        }


        public void SetIdleAnimation(IdleAnimations animation)
        {
            switch (animation)
            {
                case IdleAnimations.Idle1:
                    animator.SetInteger("IdleIndex", 0);
                    break;
                case IdleAnimations.Idle2:
                    animator.SetInteger("IdleIndex", 1);
                    break;
                case IdleAnimations.Idle3:
                    animator.SetInteger("IdleIndex", 2);
                    break;
                case IdleAnimations.Idle4:
                    animator.SetInteger("IdleIndex", 3);
                    break;
                case IdleAnimations.Idle5:
                    animator.SetInteger("IdleIndex", 4);
                    break;
                case IdleAnimations.Idle6:
                    animator.SetInteger("IdleIndex", 5);
                    break;
                case IdleAnimations.Idle7:
                    animator.SetInteger("IdleIndex", 6);
                    break;

                // Add cases for additional idle animations if needed
                default:
                    Debug.LogError("Unknown animation type: " + animation);
                    break;
            }
            animator.SetBool("isBored", false);
            isBored = false;
            idleTimer = 0f;
        }
    }
}