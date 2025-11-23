using System.Collections.Generic;
using UnityEngine;
using PythonBackend;
using static PythonBackend.RestInterface;
using System.IO;

namespace Persona
{
    public class PersonaHandler : MonoBehaviour
    {
        protected static int personaNumber = 0;
        protected static int actualPersonaId;
        private bool personaAdded;
        Dictionary<int, Persona> personaDict = new Dictionary<int, Persona>();
        [SerializeField] RestInterface pythonInterface;

        [SerializeField] private string personaName;
        [SerializeField] private int age;
        [SerializeField] private gender gender;
        [Tooltip("Between 0-10")]
        [SerializeField] private float introvertion;
        [Tooltip("Between 0-10")]
        [SerializeField] private float nervousness;
        private List<string> personaHistory = new List<string>();
        private static string historyFilePath = @"Assets/Scripts/NPC/Character/Persona/PersonaHistory.txt"; // File path to save the history
        Persona personaTmp;

        void Start()
        {
#if UNITY_EDITOR

#else
// create Persona Max if not in UNITY_EDITOR
createMax();                

#endif

        }

        //Debug fonction
        public void showAnswer(TtsResponse answer)
        {
            Debug.Log(answer.message);
            if (answer.message == "Persona added and sent successfully")
            {
                personaAdded = true;
                // Save the persona details to the history file
                AddToHistory($"Created Persona: {personaName}, ID: {actualPersonaId}, Age: {age}, gender: {gender}, Introversion: {introvertion}, Nervousness: {nervousness}");
                savePersonainCsharp();
            }
            else
            {
                Debug.Log("Persona not  created in Python.");
            }
        }

        public void createPersona()
        {
            personaTmp = Persona.CreatePersona(this.gameObject, personaName, age, gender, introvertion, nervousness);
            //Send persona to Rest Server and call showAnswer back.
            StartCoroutine(pythonInterface.sendObject(personaTmp, showAnswer));

        }
        public void createMax()
        {
            personaName = "Max";
            age = 22;
            gender = gender.M;
            introvertion = 2;
            nervousness = 2;
            createPersona();
        }

        //Save actual Persona in Dictionnary
        private void savePersonainCsharp()
        {
            personaDict.Add(personaTmp.getId(), personaTmp);
            Debug.Log("Persona added in C#");
            actualPersonaId = personaTmp.getId();
        }

        public void setPersona(int id)
        {
            actualPersonaId = id;
        }
        public bool getpersonaAdded()
        {
            return personaAdded;
        }
        public int getActualPersonaId()
        {
            return actualPersonaId;
        }

        // Adds an entry to the history and saves the file
        private void AddToHistory(string entry)
        {
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string historyEntry = $"{timestamp} - {entry}";
            personaHistory.Add(historyEntry);

            SaveHistoryToFile();
        }

        // Saves the history to the file
        public void SaveHistoryToFile()
        {
            try
            {
                File.WriteAllLines(historyFilePath, personaHistory);
                Debug.Log("History saved to file.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error saving history to file: {ex.Message}");
            }
        }
    }
}