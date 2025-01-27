using System;
using Photon.Pun;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviourPunCallbacks
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private void Update()
		{
			//Debug.Log("Input System " + photonView.IsMine);
		}
		
		public void OnMove(InputValue value)
		{
			if (photonView.IsMine)
			{
				MoveInput(value.Get<Vector2>());
				//Debug.Log("Move input for local player: " + PhotonNetwork.NickName + " | Value: " + value.Get<Vector2>());
			}
		}

		public void OnLook(InputValue value)
		{
			if (photonView.IsMine && cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
				//Debug.Log("Look input for local player: " + PhotonNetwork.NickName + " | Value: " + value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			if (photonView.IsMine)
			{
				JumpInput(value.isPressed);
				//Debug.Log("Jump input for local player: " + PhotonNetwork.NickName + " | Value: " + value.isPressed);
			}
		}

		public void OnSprint(InputValue value)
		{
			if (photonView.IsMine)
			{
				SprintInput(value.isPressed);
				//Debug.Log("Sprint input for local player: " + PhotonNetwork.NickName + " | Value: " + value.isPressed);
			}
		}

		public void OnAim(InputValue value)
		{
			if (photonView.IsMine)
			{
				AimInput(value.isPressed);
				//Debug.Log("Aim input for local player: " + PhotonNetwork.NickName + " | Value: " + value.isPressed);
			}
		}
		
		public void OnShoot(InputValue value)
		{
			if (photonView.IsMine)
			{
				ShootInput(value.isPressed);
				//Debug.Log("Shoot input for local player: " + PhotonNetwork.NickName + " | Value: " + value.isPressed);
			}
		}
		

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}
		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}
		
		

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}