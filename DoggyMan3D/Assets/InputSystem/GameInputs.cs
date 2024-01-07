using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class GameInputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;
	public bool attack;
	public bool esc;
	public bool key1;
	public bool key2;
	public bool key3;
	public bool key4;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
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

	public void OnEsc(InputValue value)
	{
		EscInput(value.isPressed);
	}

	public void OnAttack(InputValue value)
	{
		AttackInput(value.isPressed);
	}

	public void OnKey1(InputValue value)
	{
		Key1Input(value.isPressed);
	}

	public void OnKey2(InputValue value)
	{
		Key2Input(value.isPressed);
	}

	public void OnKey3(InputValue value)
	{
		Key3Input(value.isPressed);
	}

	public void OnKey4(InputValue value)
	{
		Key4Input(value.isPressed);
	}
#endif


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

	public void EscInput(bool newEscState)
	{
		esc = newEscState;
	}

	public void AttackInput(bool newAttack)
	{
		attack = newAttack;
	}

	public void Key1Input(bool newValue)
	{
		key1 = newValue;
	}

	public void Key2Input(bool newValue)
	{
		key2 = newValue;
	}

	public void Key3Input(bool newValue)
	{
		key3 = newValue;
	}

	public void Key4Input(bool newValue)
	{
		key4 = newValue;
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
