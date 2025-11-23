using UnityEngine;

namespace Persona
{
    public enum gender { M, W } 

    [System.Serializable]
    public class Persona : MonoBehaviour
    {
        static int numberOfPersona = 0;

        [SerializeField] private int id;
        [SerializeField] private string personaName;
        [SerializeField] private int age;
        [SerializeField] private gender genre;  
        [Tooltip("Between 0-10")] 
        [SerializeField] private float introvertion;
        [Tooltip("Between 0-10")]
        [SerializeField] private float nervousness;

        public static Persona CreatePersona(GameObject parent, string name, int age, gender genre, float introvert, float nerv)
        {
            GameObject personaObject = new GameObject(name);
            Persona newPersona = personaObject.AddComponent<Persona>();
            numberOfPersona++;

            newPersona.id = numberOfPersona;
            newPersona.personaName = name;
            newPersona.age = age;
            newPersona.genre = genre;
            newPersona.introvertion = introvert;
            newPersona.nervousness = nerv;

            return newPersona;
        }

        public int getId()
        {
            return this.id;
        }
    }
}
