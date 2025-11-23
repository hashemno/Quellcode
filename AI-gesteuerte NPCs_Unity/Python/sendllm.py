import requests
import json

# Code section relevant for future API key management but currently unsuitable during the development phase

# # load the API key from a file
# def load_api_key(env_var_name="KeyStackItLLM"):
#     api_key = os.getenv(env_var_name)
#     if api_key:
#         return api_key
#     else:
#         print(f"Fehler: Environment variables '{env_var_name}' not set or empty.")
#         return None


def load_api_key(file_path="KeyStackItLLM.txt"):
    try:
        with open(file_path, 'r') as file:
            return file.read().strip()
    except FileNotFoundError:
        print(f"Error: Token file '{file_path}' not found.")
    except PermissionError:
        print(f"Error: No permission to read the file '{file_path}'.")
    except Exception as e:
        print(f"Error: Unexpected error while loading the token file: {e}")
    return None

# Send a request to the LLM API
def send_llm(chat_history, model="neuralmagic/Meta-Llama-3.1-70B-Instruct-FP8", api_key=None, api_key_file="KeyStackItLLM.txt"):
    # Load the API key if not provided
    if not api_key:
        api_key = load_api_key()
        if not api_key:
            print("Error: No API key available.")
            return None

    url = "https://api.openai-compat.model-serving.eu01.onstackit.cloud/v1/chat/completions"

    # HTTP headers
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {api_key}"
    }

    # Check that chat_history is a list
    if not isinstance(chat_history, list):
        print("Error: chat_history must be a list of messages.")
        return None
    
    # Each message should contain 'role' and 'content'
    for message in chat_history:
        if not isinstance(message, dict):
            print(f"Error: Each message should be a dictionary. Invalid message: {message}")
            return None
        if 'role' not in message or 'content' not in message:
            print(f"Error: Missing 'role' or 'content' in message: {message}")
            return None

    # Request payload
    payload = {
        "model": model,
        "messages": chat_history  # Chat history as a list of dictionaries
    }

    # Print the payload for verification
    print("Payload being sent:", json.dumps(payload, indent=4))

    try:
        # Send the request to the API
        response = requests.post(url, headers=headers, json=payload)
        response.raise_for_status()  # Raise HTTP error if status is not 2xx
        return response
    except requests.exceptions.RequestException as e:
        print(f"Error: Request failed: {e}")
    except (KeyError, json.JSONDecodeError) as e:
        print(f"Error: Error processing the response: {e}")
    return None

# Test code for the request with chat history
if __name__ == "__main__":
    # Example chat history
    chat_history = [
        {"role": "user", "content": "Hi, how are you?"},
        {"role": "assistant", "content": "I'm good, thanks for asking! How can I assist you today?"},
        {"role": "user", "content": "Can you tell me a joke?"},
        {"role": "assistant", "content": "Sure! Why don't skeletons fight each other? They don't have the guts!"}
    ]
    
    # Send the request to the API
    response = send_llm(chat_history)
    
    if response:
        print(response.json().get('choices', [{}])[0].get('message', {}).get('content'))
    else:
        print("Error: LLM request failed.")
