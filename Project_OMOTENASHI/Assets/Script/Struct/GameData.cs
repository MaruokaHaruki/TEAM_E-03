using UnityEngine;

[System.Serializable]
public struct GameData
{
    //========================================
    // 共通処理
    public bool flag;
    public int Health;
    public bool DoubleJump;
    public bool Stomp;
    public bool ReverseJump;

    
    //========================================
    // プレイヤーA管理
    public int PlayerAScore;

    //========================================
    // プレイヤーB管理
    public int PlayerBScore;
};

