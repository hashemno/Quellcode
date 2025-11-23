using UnityEngine;
using PythonBackend;
using System.Collections;
using MouthSystem;
using NPCSpeechBubble;
using UnityEngine.Networking;  

namespace TextToSpeech
{
    [RequireComponent(typeof(RestInterface))]
    public class Tts : MonoBehaviour
    {
        public MouthMovement mouthMovement;
        public SpeechBubbleText speechBubbleText;  
        private RestInterface pythonInterface;

        void Start()
        {
            pythonInterface = gameObject.GetComponent<RestInterface>();
        }

        public void CallTTSScript(string text)
        {
            // Set mouth movement to "not speaking" initially
            mouthMovement.SetSpeaking(false);

            // Insert the text into the speech bubble
            speechBubbleText.ChangeText(text);

            // Send the text to the Python backend and start processing the response
            StartCoroutine(pythonInterface.FetchAudioFromServer(text, HandleSpeakResponse));
        }

        private void HandleSpeakResponse(byte[] wavData)
        {
            // Trigger mouth movement to start "speaking"
            mouthMovement.SetSpeaking(true);

            // Create a temporary path for the WAV data array
            string tempFilePath = System.IO.Path.Combine(Application.persistentDataPath, "tempAudio.wav");
            System.IO.File.WriteAllBytes(tempFilePath, wavData);

            // Load the audio data as an AudioClip
            StartCoroutine(LoadAndPlayAudio(tempFilePath));
        }

        private IEnumerator LoadAndPlayAudio(string filePath)
        {
            // Load the AudioClip with UnityWebRequest
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio loading failed: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                // Play the audio
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource if none exists
                }

                audioSource.PlayOneShot(clip);

                // Close the mouth after playback
                StartCoroutine(CloseMouthAfterDelay(clip.length));
            }
        }

        private IEnumerator CloseMouthAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Stop mouth movement after playback
            mouthMovement.SetSpeaking(false);
        }
    }
}
