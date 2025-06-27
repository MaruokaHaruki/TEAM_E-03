using UnityEngine;

[CreateAssetMenu(fileName = "RoundSettings", menuName = "Game/Round Settings")]
public class RoundSettings : ScriptableObject {
    [Header("ラウンド基本設定")]
    [Tooltip("ラウンド番号")]
    public int roundNumber = 1;

    [Tooltip("ラウンド名")]
    public string roundName = "Round 1";

    [Tooltip("勝利時獲得ポイント")]
    public int winPoints = 1;

    [Header("プレイヤー機能設定")]
    [Tooltip("2段ジャンプ機能を有効にするか")]
    public bool enableDoubleJump = false;

    [Tooltip("踏みつけ機能を有効にするか")]
    public bool enableStomp = false;

    [Tooltip("反転ジャンプ機能を有効にするか")]
    public bool enableReverseJump = false;

    [Tooltip("速度交換機能を有効にするか")]
    public bool enableSpeedTransfer = true;

    [Header("ゲームバランス設定")]
    [Tooltip("プレイヤーの最大HP")]
    public int playerMaxHp = 100;

    [Tooltip("基本移動速度")]
    public float baseSpeed = 5.0f;

    [Tooltip("ジャンプ力")]
    public float jumpForce = 4.0f;

    [Header("特殊ルール")]
    [Tooltip("無敵時間")]
    public float invincibilityDuration = 3.0f;

    [Tooltip("踏みつけスタン時間")]
    public float stompStunDuration = 2.0f;
}