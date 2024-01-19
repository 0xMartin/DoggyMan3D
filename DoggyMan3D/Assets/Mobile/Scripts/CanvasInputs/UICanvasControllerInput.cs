using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public GameInputs InputSystem;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            InputSystem.SprintInput(virtualMoveDirection.magnitude >= 1.0);
            InputSystem.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            InputSystem.LookInput(virtualLookDirection);
        }

        public void VirtualActivateAttackInput()
        {
            InputSystem.AttackInput(true);
        }

        public void VirtualDeactivateAttackInput()
        {
            InputSystem.AttackInput(false);
        }

        public void VirtualActivateKey1Input()
        {
            InputSystem.Key1Input(true);
        }

        public void VirtualDeactivateKey1Input()
        {
            InputSystem.Key1Input(false);
        }

        public void VirtualActivateKey2Input()
        {
            InputSystem.Key2Input(true);
        }

        public void VirtualDeactivateKey2Input()
        {
            InputSystem.Key2Input(false);
        }

        public void VirtualActivateKey3Input()
        {
            InputSystem.Key3Input(true);
        }

        public void VirtualDeactivateKey3Input()
        {
            InputSystem.Key3Input(false);
        }

        public void VirtualActivateKey4Input()
        {
            InputSystem.Key4Input(true);
        }

        public void VirtualDeactivateKey4Input()
        {
            InputSystem.Key4Input(false);
        }

        public void VirtualActivateEscInput()
        {
            InputSystem.EscInput(true);
        }

        public void VirtualDeactivateEscInput()
        {
            InputSystem.EscInput(false);
        }
        
    }

}
