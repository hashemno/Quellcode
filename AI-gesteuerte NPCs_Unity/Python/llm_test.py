import unittest
import requests
from sendllm import send_llm

# Test case class for testing the send_llm function
class SendLLMTestCase(unittest.TestCase):
    def test_llm_availability(self):
        """Test the sendllm method."""
        response = send_llm("Test prompt")

        # Assert the response
        self.assertEqual(response.status_code, 200)

if __name__ == '__main__':
    unittest.main()