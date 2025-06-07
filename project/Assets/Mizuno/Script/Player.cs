using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        Attack
    }

    private Vector3 _velocity;
    public State CurrentState { get; private set; } = State.Idle;

    void Update()
    {
        // テスト用：数字キーでステート変更
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) SetState(State.Idle);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) SetState(State.Run);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) SetState(State.Jump);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) SetState(State.Attack);

        transform.position += _velocity * Time.deltaTime;
    }

    public void SetState(State newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            Debug.Log("現在の状態: " + newState);
        }
    }

    private void OnMove(InputValue value)
    {
        // MoveActionの入力値を取得
        var axis = value.Get<Vector2>();

        // 移動速度を保持
        _velocity = new Vector3(axis.x, 0, axis.y);
    }
}
