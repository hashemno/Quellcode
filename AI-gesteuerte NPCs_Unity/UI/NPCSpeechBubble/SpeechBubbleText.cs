using UnityEngine;
using TMPro;
using MouthSystem;
using System.Collections;

namespace NPCSpeechBubble
{
    public class SpeechBubbleText : MonoBehaviour
    {
        public MouthMovement mouthMovement; 
        private TMP_Text speechBubbleText;

        void Start()
        {
            // Retrieve the TextMeshPro component
            speechBubbleText = GetComponent<TextMeshProUGUI>();

            if (speechBubbleText == null)
            {
                Debug.LogError("TextMeshProUGUI could not be found.");
                return;
            }
        }

        // Change the text in the speech bubble
        public void ChangeText(string text)
        {
            if (speechBubbleText != null)
            {
                speechBubbleText.text = text;

                // If text is set, activate mouth movement
                if (!string.IsNullOrEmpty(text))
                {
                    mouthMovement.SetSpeaking(true);
                    StartCoroutine(StopMouthAfterSpeech(text));
                }
                else
                {
                    mouthMovement.SetSpeaking(false);
                }
            }
        }

        // Stop mouth movement after the text has been read
        private IEnumerator StopMouthAfterSpeech(string text)
        {
            
            yield return new WaitForSeconds(text.Length / 10f); 

            // Stop mouth movement once the text has been fully spoken
            mouthMovement.SetSpeaking(false);
        }

        public string GetCurrentText()
        {
            return speechBubbleText != null ? speechBubbleText.text : string.Empty;
        }

    }
}
