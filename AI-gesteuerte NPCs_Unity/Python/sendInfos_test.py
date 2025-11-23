import unittest
from unittest.mock import patch, MagicMock
from flask import Flask, jsonify, request, make_response
from RestServer import app

class SendInfosTestCase(unittest.TestCase):
    def setUp(self):
        """Create and setup all needed Objects"""
        self.app = app
        self.client = self.app.test_client()
        self.app.testing = True
    
    def test_sendInfos_valid_text(self):
        """Test the /sendInfos route with valid text input."""
        # Make a POST request to the route using the Flask test client
        response = self.client.post(
            '/sendInfos', 
            json={'text': 'hello'}
        )

        # Assert the response from the Flask route
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.json.get('status'), 'success')
        self.assertNotEqual(response.json.get('message', ''), '')

    def test_sendInfos_missing_text(self):
        """Test the /sendInfos route with missing 'text' key."""
        response = self.client.post(
            '/sendInfos', 
            json={}
        )

        # Assert the response
        self.assertEqual(response.status_code, 400)
        self.assertIn('error', response.json)
        self.assertEqual(response.json.get('error'), 'No text provided')
        
    def test_sendInfos_no_json(self):
        """Test the /sendInfos route with no JSON data."""
        self.response = self.client.post(
            '/sendInfos',
            data="Non-JSON data",
            content_type="text/plain"
        )

        # Assert the response
        self.assertEqual(self.response.status_code, 415) # 415 for unsupported media type
        self.assertIn('error', self.response.json)

    def tearDown(self):
        """Delete all Objects"""
        del self.app
        del self.client

if __name__ == '__main__':
    unittest.main()