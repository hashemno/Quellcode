import unittest
from unittest.mock import patch, MagicMock
from flask import Flask, jsonify, request, make_response
from RestServer import app

class SpeakTestCase(unittest.TestCase):
    def setUp(self):
        """Create and setup all needed Objects"""
        self.app = app
        self.client = self.app.test_client()
        self.app.testing = True
    
    @patch('RestServer.tts.text_to_speech')  # Mock tts.text_to_speech method
    @patch('os.remove')  # Mock os.remove to avoid actual file deletion
    def test_speak_valid_text(self, mock_os_remove, mock_text_to_speech):
        """Test the /speak route with valid text input."""
        # Mock the behavior of text_to_speech
        mock_audio_buffer = MagicMock()
        mock_audio_buffer.read.return_value = b"FAKE_AUDIO_DATA"
        mock_text_to_speech.return_value = mock_audio_buffer

        # Send POST request to /speak
        response = self.client.post(
            '/speak',
            json={'text': 'Hello, world!'}
        )

        # Assert the response
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.headers.get('Content-Type'), 'audio/wav')
        self.assertEqual(response.data, b"FAKE_AUDIO_DATA")

        # Assert mock calls, returns ok if os.remove and
        mock_text_to_speech.assert_called_once_with('Hello, world!')
        mock_os_remove.assert_called_once_with('generated_speech.wav')

    def test_speak_missing_text(self):
        """Test the /speak route with missing text input."""
        # Send POST request with no data
        response = self.client.post(
            '/speak',
            json={}
        )

        # Assert the response
        self.assertEqual(response.status_code, 400)
        self.assertEqual(response.json, {'error': 'No text provided'})

    def test_speak_no_json(self):
        """Test the /speak route with no JSON data."""
        response = self.client.post(
            '/speak',
            data="Non-JSON data",
            content_type="text/plain"
        )

        # Assert the response
        self.assertEqual(response.status_code, 415) # 415 for unsupported media type
        self.assertIn('error', response.json)

    def tearDown(self):
        """Delete all Objects"""
        del self.app
        del self.client

if __name__ == '__main__':
    unittest.main()