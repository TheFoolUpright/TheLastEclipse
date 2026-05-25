using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool walk;
		public bool changeVisual;

        // True only on the exact frame the jump button is pressed
        // (used for jump buffering so we don't miss input)
        public bool jumpPressedThisFrame;
        // Tracks the previous frame's sprint button state
        // (used to detect a button press instead of a hold)
        public bool walkToggleButtonHeldLastFrame;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        public bool PauseMenu { get; set; } // add this with the other public bools


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnChangeVisual(InputValue value)
		{
			ChangeVisual(value.isPressed);
		}


        public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif

        public void OnPauseMenu(InputValue value)
        {
            PauseMenuInput(value.isPressed);
        }

        public void PauseMenuInput(bool newPauseMenuState)
        {
            PauseMenu = newPauseMenuState;
        }
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void ChangeVisual(bool isPressed)
		{
			changeVisual = isPressed;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
            // Detect the exact frame the jump button is pressed
            // (button was NOT pressed before, but IS pressed now)
            if (newJumpState && !jump)
                // Store a one-frame "jump pressed" event
                // This lets us buffer the jump even if we aren't grounded yet
                jumpPressedThisFrame = true;

            // Store whether the button is currently being held down
            jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
            // Detect the exact moment the sprint button is pressed
            if (newSprintState && !walkToggleButtonHeldLastFrame)
                // Toggle sprint on/off each time the button is pressed
                walk = !walk;

            // Store the current button state for the next frame
            walkToggleButtonHeldLastFrame = newSprintState;
        }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

        private void LateUpdate()
        {
            // Reset the one-frame jump press so it only lasts a single frame
            // This prevents the jump from triggering multiple times
            jumpPressedThisFrame = false;
        }
    }
	
}