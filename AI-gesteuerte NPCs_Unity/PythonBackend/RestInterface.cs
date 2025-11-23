using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace PythonBackend
{
    public class RestInterface : MonoBehaviour
    {
        private static string baseUrl = "http://127.0.0.1:5000"; // localhost

        [SerializeField]
        private AudioSource audioSource; // Object that plays the audio in the scene

        [System.Serializable]
        public class TtsRequest
        {
            public int personaId;
            public string text; // The text to be converted to speech
        }

        [System.Serializable]
        public class TtsResponse
        {
            public string message; // The response message from TTS
        }

        // Sends a request to the given endpoint with the provided request data and executes the response handler
        private IEnumerator SendRequest(string endpoint, TtsRequest requestData, Action<UnityWebRequest> onResponse)
        {
            string json = JsonUtility.ToJson(requestData);
            Debug.Log("Data to json  :" + json);
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/{endpoint}", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest(); // Wait for the request to complete
                onResponse.Invoke(request); // Invoke the response handler with the result
            }
        }
        //Interface that receive an Object o , convert it to json text Format and post it 
        private IEnumerator SendRequest(string endpoint, object o, Action<UnityWebRequest> onResponse)
        {
            string json = JsonUtility.ToJson(o);
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/{endpoint}", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                onResponse.Invoke(request);
            }
        }

        // Sends the input text to the server and triggers the callback with the response
        public IEnumerator SendInfos(string text, Action<TtsResponse> callback)
        {
            TtsRequest ttsRequest = new TtsRequest { text = text };
            yield return SendRequest("sendInfos", ttsRequest, (response) =>
            {
                if (response.result == UnityWebRequest.Result.Success)
                {
                    callback(JsonUtility.FromJson<TtsResponse>(response.downloadHandler.text)); // Parse and pass the response
                }
                else
                {
                    Debug.LogError($"Request error: {response.error}");
                }
            });
        }

        // Sends the input text to the server for generating a GPT prompt and handles the response
        public IEnumerator PromptGPT(int personaId, string text, Action<TtsResponse> callback)
        {
            TtsRequest ttsRequest = new TtsRequest { personaId= personaId,text = text };
            yield return SendRequest("promptGPT", ttsRequest, (response) =>
            {
                if (response.result == UnityWebRequest.Result.Success)
                {
                    callback(JsonUtility.FromJson<TtsResponse>(response.downloadHandler.text));
                }
                else
                {
                    Debug.LogError($"Request error: {response.error}");
                }
            });
        }

        // Fetches audio data from the server based on the input text and plays it
        public IEnumerator FetchAudioFromServer(string text, Action<byte[]> callback)
        {
            TtsRequest ttsRequest = new TtsRequest { text = text };
            yield return SendRequest("speak", ttsRequest, (response) =>
            {
                if (response.result == UnityWebRequest.Result.Success)
                {
                    byte[] audioData = response.downloadHandler.data;
                    audioSetup(audioData); // Setup and play the audio
                    callback.Invoke(audioData); // Pass the audio data to the callback
                }
                else
                {
                    Debug.LogError($"Request error: {response.error}");
                }
            });
        }

        // Sets up the audio data by creating an AudioClip and playing it through the AudioSource
        private void audioSetup(byte[] audioData)
        {
            AudioClip audioClip = CreateAudioClipFromWAV(audioData);
            if (audioClip != null)
            {
                audioSource.clip = audioClip; // Assign the AudioClip to the AudioSource
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Failed to create AudioClip.");
            }
        }

        // Creates an AudioClip from a WAV byte array by extracting the audio data
        public AudioClip CreateAudioClipFromWAV(byte[] wavData)
        {
            int channels = BitConverter.ToInt16(wavData, 22);
            int sampleRate = BitConverter.ToInt32(wavData, 24);
            int bitDepth = BitConverter.ToInt16(wavData, 34);
            int offset = 44; // WAV header size

            int sampleCount = (wavData.Length - offset) / (bitDepth / 8); // Calculate the number of samples
            float[] audioData = new float[sampleCount];

            // Convert the raw byte data to normalized audio samples
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(wavData, offset + (i * 2));
                audioData[i] = sample / 32768f; // Normalize sample to [-1, 1]
            }

            // Create and return the AudioClip
            AudioClip audioClip = AudioClip.Create("ReceivedAudioClip", sampleCount, channels, sampleRate, false);
            audioClip.SetData(audioData, 0);
            return audioClip;
        }

        public IEnumerator sendObject(object o, Action<TtsResponse> callback)
        {

            yield return SendRequest("send_persona", o, (response) =>
            {
                if (response.result == UnityWebRequest.Result.Success)
                {
                    callback(JsonUtility.FromJson<TtsResponse>(response.downloadHandler.text));
                }
                else
                {
                    Debug.LogError($"Request error: {response.error}");
                }
            });
        }

    }
}