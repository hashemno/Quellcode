using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAnimation
{

    public interface Animation
    {

    }
    // IStaticAnimations & IDynamicAnimations beide erben von der Animation Schnittstelle
    public interface IStaticAnimation : Animation
    {

        // Static animations typically involve stationary actions
        void Idle();
        void Wave();
        void Mouthmovement(bool isSpeaking);

    }

    public interface IDynamicAnimation : Animation
    {
        // Dynamic animations involve movement
        void Walk();
        void Grab();
    }


}