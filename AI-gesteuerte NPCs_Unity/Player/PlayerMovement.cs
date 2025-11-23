using UnityEngine;
using NPCInputField;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private Camera playerCamera;
        [SerializeField] ReadInput readInput;

        [SerializeField]
        private float walkSpeed = 7f;

        [SerializeField]
        private float jumpPower = 5f;

        [SerializeField]
        private float gravity = 10f;

        [SerializeField]
        private float lookSpeed = 2f;

        const float lookXLimit = 45f; // Limit for looking up and down
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0; // Camera rotation
        private CharacterController characterController;

        private bool isMovementEnabled = true; // Track the movement state

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor in the center of the scree
            Cursor.visible = false; // Hide the cursor
        }

        void FixedUpdate()
        {
            // Move the player if movement is enabled
            if (isMovementEnabled)
            {
                movement();
            }

            // Disable movement if the input field is visible
            if (readInput.getIsInputFieldVisible())
            {
                isMovementEnabled =false;
            }
            else 
            {
                isMovementEnabled =true;
            }

        }

        void movement()
        {
            // Calculate forward and right directions relative to the player's rotation
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float currentSpeedX = walkSpeed * Input.GetAxis("Vertical");
            float currentSpeedY = walkSpeed * Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;

            // Update the movement direction
            moveDirection = (forward * currentSpeedX) + (right * currentSpeedY);

            // Perform jumping
            if (Input.GetButton("Jump") && characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity if not grounded
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            // Manage mouse look for vertical rotation (up and down)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        
        public bool getIsMovementEnabled()
        {
            return isMovementEnabled;
        }
    }
}