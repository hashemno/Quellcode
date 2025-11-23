using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Distance;
using TextToSpeech;
using PythonBackend;
using static PythonBackend.RestInterface;
using NPCSpeechBubble;
using Persona;
using System.IO; 
using Chat;
namespace NPCInputField
{

    public class ReadInput : MonoBehaviour
    {
        [SerializeField] InputField inputField; // Reference to the InputField
        private bool isInputFieldVisible = false; // Track the visibility state
        private string userInput;
        [SerializeField] DistanceReaction distanceReaction;
        [SerializeField] SpeechBubbleText SpeechBubble;
        [SerializeField] RestInterface pythonInterface;
        [SerializeField]PersonaHandler personaHandler;

        private Tts textToSpeech;
      
        void Start()
        {
            textToSpeech = GetComponentInParent<Tts>();

            // Ensure an EventSystem exists in the scene
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            // Add an event listener that reacts to pressing Enter
            inputField.onEndEdit.AddListener(HandleInput);

            // Initially, set the input field to be invisible and not interactable
            inputField.gameObject.SetActive(false);
            inputField.interactable = false;
        }

        void FixedUpdate()
        {
            // if player is inside Zone and press 'E'
            if (distanceReaction.GetInsideZone() && Input.GetKey(KeyCode.E))
            {
                inputFieldActivation();
            }

            //if player is outside Zone and inputField is visible 
            if ((!distanceReaction.GetInsideZone()) && isInputFieldVisible)
            {
                inputFieldDeactivation();
            }
        }

        public void SubmitInput()
        {
            string input = inputField.text.Trim();
            
            if (!string.IsNullOrEmpty(input))
            {
                userInput = input;
                HandleInput(input);
            }
        }

        private void HandleInput(string input)
        {
            // Check if Return or Keypad Enter was pressed
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                inputField.text = ""; // Clear the input field
                inputFieldDeactivation();
                // pythonAccessPoint.sendMessage(input); //send input to agent
                StartCoroutine(pythonInterface.PromptGPT(personaHandler.getActualPersonaId(),input, HandleSpeakResponse)); // receive agent answer
            }
                    ChatHistory.SaveChatToHistory("User: "+input);
        }

        private void HandleSpeakResponse(TtsResponse answer)
        {
            SpeechBubble.ChangeText(answer.message);
            textToSpeech.CallTTSScript(answer.message);
             ChatHistory.SaveChatToHistory($"NPC: {answer.message}");
        }
       

        // activate input Field  so you can type inside 
        public void inputFieldActivation()
        {

            isInputFieldVisible = true;
            inputField.gameObject.SetActive(true);
            inputField.enabled = true;
            inputField.interactable = true;
            inputField.ActivateInputField(); // Set the focus to the input field
            inputField.Select(); // Ensure the input field is selected
            Cursor.lockState = CursorLockMode.Locked; //Lock the Cursor in the middle
            Cursor.visible = false; // set Cursor invisible

        }

        //deactivate input Field 
        public void inputFieldDeactivation()
        {
            isInputFieldVisible = false;
            inputField.gameObject.SetActive(false);
            inputField.DeactivateInputField();
            //Cursor.lockState = CursorLockMode.None;// enable Cursor
            //Cursor.visible = true; // set Cursor on visble 
        }

        public bool getIsInputFieldVisible()
        {
            return isInputFieldVisible;
        }
    }
}