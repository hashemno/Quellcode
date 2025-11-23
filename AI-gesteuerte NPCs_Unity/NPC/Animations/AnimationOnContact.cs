using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{

    public class AnimationOnContact : MonoBehaviour
    {
        private Animator mAnimator;
        private float lastPlayTime;
        [SerializeField]
        private float cooldown = 5f; // Cooldown for the animation 



        // Start is called before the first frame update
        void Start()
        {
            mAnimator = GetComponent<Animator>();
            lastPlayTime = -cooldown;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void playanimation() // Plays the animation when the cooldown for the animation is over
        {
            if (mAnimator != null && Time.time >= lastPlayTime + cooldown)
            {
                mAnimator.SetTrigger("TrContact");
                lastPlayTime = Time.time;
            }
        }
    }


}