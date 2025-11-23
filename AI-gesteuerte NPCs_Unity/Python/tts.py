import pyttsx3
import io

# language : en_US, de_DE, ...
# gender : VoiceGenderFemale, VoiceGenderMale
def text_to_speech(text):
    engine = pyttsx3.init()
        
    engine.setProperty('voice', 'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\TTS_MS_EN-US_ZIRA_11.0')

    # Temporary path for the generated audio file
    temp_file_path = 'generated_speech.wav'
    # Save the generated speech to a file
    engine.save_to_file(text, temp_file_path)
    engine.runAndWait()

    # Read the saved audio file into a buffer
    with open(temp_file_path, 'rb') as f:
        audio_buffer = io.BytesIO(f.read())
        return audio_buffer
