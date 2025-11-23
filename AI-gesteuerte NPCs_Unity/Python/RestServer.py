from flask import Flask, jsonify, request, make_response
from flask_cors import CORS
import chatgpt
from sendllm import send_llm
import tts
from Persona import Persona

# Initialize the Flask app and CORS
app = Flask(__name__)
CORS(app)

# Global variables
chat_history = []
persona_dict = {}

# Route for GPT interaction
@app.route('/promptGPT', methods=['POST'])
def promptGPT():
    # Check if the request is in JSON format
    if not request.is_json:
        return jsonify({'error': 'Unsupported Media Type. Content-Type must be application/json'}), 415
    
    # Get the content of the request
    content = request.get_json()
    inputstring = content.get('text', '')

    if inputstring:
        # Update chat history: Add user message
        chat_history.append({'role': 'user', 'content': inputstring})

        # Get GPT response
        response = send_llm(chat_history)
        if response and response.status_code == 200:
            # Extract the GPT response from the API response
            llm_response = response.json().get('choices', [{}])[0].get('message', {}).get('content', '')
            # Add GPT response to chat history
            chat_history.append({'role': 'assistant', 'content': llm_response})
            return jsonify({'status': 'success', 'message': llm_response}), 200
        else:
            return jsonify({'error': 'Failed to get response from LLM'}), 500
    
    return jsonify({'error': 'No text provided'}), 400

# Route to perform Text-to-Speech (TTS)
@app.route('/speak', methods=['POST'])
def speak():
    # Check if the request is in JSON format
    if not request.is_json:
        return jsonify({'error': 'Unsupported Media Type. Content-Type must be application/json'}), 415
    
    # Get the content of the request
    content = request.get_json()
    text = content.get('text', '')

    if text:
        # Convert text to speech and generate audio
        audio_buffer = tts.text_to_speech(text)
        # Prepare the response with the audio file
        response = make_response(audio_buffer.read())
        response.headers.set('Content-Type', 'audio/wav')
        response.headers.set('Content-Disposition', 'attachment', filename='speech.wav')
        return response, 200
    
    return jsonify({'error': 'No text provided'}), 400

# Route to add a new Persona
@app.route('/send_persona', methods=['POST'])
def add_persona():
    # Retrieve the JSON data
    data = request.get_json()
    if not data:
        return jsonify({"error": "Invalid data"}), 400

    # Extract persona data from the request
    try:
        identification = data["id"]
        name = data["personaName"]
        age = data["age"]
        genre = data["genre"]
        introvertion = data["introvertion"]
        nervousness = data["nervousness"]
    except KeyError as e:
        return jsonify({"error": f"Missing field: {e.args[0]}"}), 400

    # Create a new Persona object and add it to the dictionary
    new_persona = Persona(identification, name, age, genre, introvertion, nervousness)
    persona_dict[new_persona.id] = new_persona

    # Prepare a system message for LLM
    persona_message = {
        "role": "system",
        "content": f"New Persona added: ID - {identification}, Name - {name}, Age - {age}, Genre - {genre}, "
                   f"Introvertion - {introvertion}, Nervousness - {nervousness}"
    }
    chat_history.append(persona_message)  # Update global chat history

    # Send persona details to LLM
    response = send_llm(chat_history)
    if response and response.status_code == 200:
        llm_response = response.json().get('choices', [{}])[0].get('message', {}).get('content', '')
        return jsonify({'status': 'success', "message": "Persona added and sent successfully", "llm_response": llm_response}), 201
    else:
        return jsonify({'error': 'Persona added, but failed to send to LLM'}), 500


# Route to update a document
@app.route('/sendInfos', methods=['POST'])
def send_infos():
    # Check if the request is in JSON format
    if not request.is_json:
        return jsonify({'error': 'Unsupported Media Type. Content-Type must be application/json'}), 415
    
    # Get the content of the request
    content = request.get_json()
    input_string = content.get('text', '')
    
    if input_string:
        # Call chatgpt to update the document
        answer = chatgpt.update_document(input_string)
        return jsonify({'status': 'success', 'message': answer}), 200
    
    return jsonify({'error': 'No text provided'}), 400

# Main function to run the app
if __name__ == '__main__':
    app.run(host='0.0.0.0', debug=True)
