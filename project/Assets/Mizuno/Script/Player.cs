using UnityEngine;

public class Player : MonoBehaviour
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
        // �e�X�g�p�F�L�[�ŃX�e�[�g�ύX
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetState(State.Idle);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetState(State.Run);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetState(State.Jump);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetState(State.Attack);
    }

    public void SetState(State newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            Debug.Log("State changed to: " + newState);
        }
    }
}
