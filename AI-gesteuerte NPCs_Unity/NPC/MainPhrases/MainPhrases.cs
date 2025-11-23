using System.Collections;       // For using IEnumerator and Coroutines
using UnityEngine;              // For Unity-specific classes like MonoBehaviour, AudioClip, Debug

namespace MainPhrases           // Namespace to organize code logically
{
    public class MainPhrases : MonoBehaviour
    {
        // References to other components
        [SerializeField]
        private NPCSpeechBubble.SpeechBubbleText _speechBubbleText; // Reference to the speech bubble UI

        [SerializeField]
        private PythonBackend.RestInterface restInterface; // Reference to a REST interface for TTS/audio

        [Tooltip("Gib hier den Text ein")]
        [TextArea(3, 10)]
        public string userInput; // User input to be sent to the backend

        private bool isAudioPlaying = false; // Flag to prevent overlapping audio

        // Public read-only properties to access private fields
        public NPCSpeechBubble.SpeechBubbleText SpeechBubbleText => _speechBubbleText;
        public PythonBackend.RestInterface RestInterface => restInterface;

        // Initialization of component references
        public void Initialize()
        {
            // Find SpeechBubbleText if not assigned
            if (_speechBubbleText == null)
            {
                _speechBubbleText = FindObjectOfType<NPCSpeechBubble.SpeechBubbleText>();
            }

            // Find RestInterface if not assigned
            if (restInterface == null)
            {
                restInterface = FindObjectOfType<PythonBackend.RestInterface>();
                if (restInterface == null)
                {
                    Debug.LogError("RestInterface not found in the scene. Please add it to a GameObject.");
                }
            }
        }

        // Unity Start lifecycle method
        protected void Start()
        {
            Initialize(); // Ensure all references are set
        }

        // Method to react when player enters a zone
        [ContextMenu("Print User Input")] // Can be called from the Unity Inspector for testing
        public void ReactToZoneEnter()
        {
            // Check for empty input
            if (string.IsNullOrEmpty(userInput))
            {
                Debug.LogError("User input is empty.");
                return;
            }

            // Check if RestInterface is assigned
            if (restInterface == null)
            {
                Debug.LogError("RestInterface is not assigned.");
                return;
            }

            // Call backend asynchronously to fetch audio
            StartCoroutine(restInterface.FetchAudioFromServer(userInput, (audioData) =>
            {
                if (audioData != null)
                {
                    // Convert WAV data to AudioClip
                    AudioClip audioClip = restInterface.CreateAudioClipFromWAV(audioData);
                    if (audioClip != null)
                    {
                        // Update speech bubble if available
                        if (_speechBubbleText != null)
                        {
                            _speechBubbleText.ChangeText(userInput);
                        }

                        // Play audio if not already playing
                        if (!isAudioPlaying)
                        {
                            isAudioPlaying = true;
                            restInterface.GetComponent<AudioSource>().PlayOneShot(audioClip);
                            StartCoroutine(ResetAudioFlag(audioClip.length)); // Reset flag after clip ends
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to fetch or parse audio data from server.");
                }
            }));
        }

        // Coroutine to reset the audio playing flag after the clip finishes
        private IEnumerator ResetAudioFlag(float delay)
        {
            yield return new WaitForSeconds(delay);
            isAudioPlaying = false;
        }

        // Method to stop audio when player leaves a zone
        [ContextMenu("Stop Audio on Zone Leave")]
        public void ReactToZoneLeave()
        {
            if (restInterface != null && restInterface.GetComponent<AudioSource>() != null)
            {
                AudioSource audioSource = restInterface.GetComponent<AudioSource>();
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    isAudioPlaying = false; // Reset flag
                }
            }
            else
            {
                Debug.LogWarning("AudioSource or RestInterface is not assigned.");
            }
        }
    }
}
