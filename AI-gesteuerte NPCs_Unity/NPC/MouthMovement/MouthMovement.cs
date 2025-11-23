using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MouthSystem
{
    public class MouthMovement : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public string blendShapeName = "MouthOpen";
        [SerializeField] private float speed = 200; // Speed can be adjusted in the Inspector

        // Constants defining the maximum and minimum values for mouth opening
        private int blendShapeIndex;
        private float mouthOpenValue = 0f;
        private bool isOpening = true;
        private bool isSpeaking = false;

        void Start()
        {
            // Get the index of the blend shape controlling the mouth
            blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
        }

        void Update()
        {

            if (isSpeaking)
            {
                // Check if the mouth is currently opening or closing
                if (isOpening)
                {
                    mouthOpenValue += speed * Time.deltaTime;
                    if (mouthOpenValue >= 100f)
                    {
                        mouthOpenValue = 100f;
                        // Switch to closing movement
                        isOpening = false;
                    }
                }
                else
                {
                    mouthOpenValue -= speed * Time.deltaTime;
                    if (mouthOpenValue <= 0f)
                    {
                        mouthOpenValue = 0f;
                        // Switch to opening movement
                        isOpening = true;
                    }
                }
                // Apply the blend shape change to the mesh
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, mouthOpenValue);
            }
        }

        public void SetSpeaking(bool speaking)
        {
            isSpeaking = speaking;
            if (!isSpeaking)
            {
                // Gently close the mouth when not speaking
                mouthOpenValue = Mathf.Lerp(mouthOpenValue, 0f, Time.deltaTime * 5f);  // Smooth transition to closin
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, mouthOpenValue);

            }
        }

        public bool IsSpeaking()
        {
            return isSpeaking;
        }
    }
}