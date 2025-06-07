using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    private Vector3 inputVal;
    public enum State
    {
        Idle,
        Run,
        Jump,
        Attack
    }

    private InputSystem_Actions m_Actions;                  // Source code representation of asset.
    private InputSystem_Actions.PlayerActions m_Player;     // Source code representation of action map.

    public State CurrentState { get; private set; } = State.Idle;

    void Update()
    {
        // テスト用：数字キーでステート変更
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) SetState(State.Idle);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) SetState(State.Run);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) SetState(State.Jump);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) SetState(State.Attack);


    }

    public void SetState(State newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            Debug.Log("現在の状態: " + newState);
        }
    }

    public void OnMove(InputValue inputValue)
    {
        inputVal = new Vector3(inputValue.Get<Vector2>().x, 0f, inputValue.Get<Vector2>().y);
        Debug.Log(inputValue);
    }
}
