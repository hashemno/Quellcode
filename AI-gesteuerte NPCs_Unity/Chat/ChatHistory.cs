using System.IO;          // For file operations like reading, writing, and checking files
using UnityEngine;        // For Unity-specific functions like Debug.Log

namespace Chat           // Namespace to logically group the class
{
    public static class ChatHistory
    {
        // Path to the text file where the chat history will be saved
        private static string chatHistoryFilePath = @"Assets/Scripts/Chat/ChatHistory.txt";

        // Method to save a chat entry to the text file
        public static void SaveChatToHistory(string chatText)
        {
            // Generate a timestamp for the chat entry
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Format the chat text with the timestamp
            string historyEntry = $"{timestamp}: {chatText}\n";

            // Check if the file exists; if not, create it
            if (!File.Exists(chatHistoryFilePath))
                File.Create(chatHistoryFilePath).Close(); // Close() is necessary to avoid file lock

            // Append the new chat entry to the file
            File.AppendAllText(chatHistoryFilePath, historyEntry);

            // Log for debugging purposes
            Debug.Log("Chat history saved.");
        }

        // Method to load the entire chat history from the file
        public static string LoadChatHistory()
        {
            // Check if the file exists
            if (File.Exists(chatHistoryFilePath))
            {
                // Return the entire content of the file
                return File.ReadAllText(chatHistoryFilePath);
            }
            else
            {
                // Log a warning if the file does not exist
                Debug.LogWarning("Chat history file does not exist.");
                return null; // Return null to indicate the absence of the file
            }
        }
    }
}
