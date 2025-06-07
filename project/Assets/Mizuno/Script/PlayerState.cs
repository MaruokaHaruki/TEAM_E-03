using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        Attack
    }
    public State CurrentState { get; private set; } = State.Idle;

    void Update()
    {
        // �e�X�g�p�F�����L�[�ŃX�e�[�g�ύX
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetState(State.Idle);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetState(State.Run);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetState(State.Jump);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SetState(State.Attack);

    }


    public void SetState(State newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            Debug.Log("���݂̏��: " + newState);
        }
    }

}
