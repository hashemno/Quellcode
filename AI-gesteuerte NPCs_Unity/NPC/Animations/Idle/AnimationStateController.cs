using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Idle
{

    public class AnimationStateController : MonoBehaviour
    {
        Animator animator; // Reference to the Animator component that controls animations
        bool canTrigger = true; // Flag to check if the animation can be triggered
        float cooldownDuration = 3.0f; // Duration of the cooldown before the animation can be triggered again

        void Start()
        {
            animator = GetComponent<Animator>();
            Debug.Log("Component found");
        }

        void Update()
        {
            if (canTrigger)
            {
                // When Enter is pressed -> Nod the head
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StartCoroutine(TriggerWithCooldown("isAgreeing"));
                }
            }
        }

        // Cooldown animation after trigger
        IEnumerator TriggerWithCooldown(string triggerName)
        {
            canTrigger = false;
            animator.SetTrigger(triggerName);

            yield return new WaitForSeconds(cooldownDuration);

            canTrigger = true;
        }
    }

}