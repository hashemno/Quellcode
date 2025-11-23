using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blink
{

    public class Blink : MonoBehaviour
    
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public string blendShapeName = "Blink";

        private int blendShapeIndex;
        private float blinkValue = 0f;
        private bool isBlinking = false;
        [SerializeField]private float minBlinkRange = 2f; // Minimum blink range
        [SerializeField]private float maxBlinkRange = 4f; // Maximum blink range
        

        void Start()
        {
            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("Error: SkinnedMeshRenderer is null", this);
            }
            else { 

            blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
            // Start blink animation
            StartCoroutine(BlinkRoutine());
            }

            

        }

    
        IEnumerator BlinkRoutine()
        { 
            isBlinking = true;

            while (isBlinking) { 

                // Eyes are closing
                while (blinkValue < 100f)
                {
                    blinkValue += Time.deltaTime * 400f;
                    skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blinkValue);
                    yield return null;
                }
                blinkValue = 100f;
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blinkValue); // Eyes are closed (blinkValue = 100f)

                // Eyes are opening
                while (blinkValue > 0f)
                {
                    blinkValue -= Time.deltaTime * 400f;
                    skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blinkValue);
                    yield return null;
                }

                blinkValue = 0f;
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blinkValue); // Eyes are open (blinkValue = 0f))

                yield return new WaitForSeconds(Random.Range(minBlinkRange, maxBlinkRange)); // Blink interval, a small pause before the next blink

            }
        }

    }
}

