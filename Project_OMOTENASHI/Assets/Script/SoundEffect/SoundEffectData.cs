using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectData : MonoBehaviour
{
    /// <summary>タイトルサウンドナンバー</summary>
    public enum TITLE_SOUND_NUMBER
    {
        /// <summary>開始音</summary>
        START = 0,
    }

    /// <summary>ラウンドサウンドナンバー</summary>
    public enum ROUND_SOUND_NUMBER
    {
        /// <summary>最初</summary>
        GAME_START = 0,
        /// <summary>カウントダウン</summary>
        COUNT = 1,
        /// <summary>ラウンド開始時</summary>
        ROUND_START = 2,
        /// <summary>ラウンド終了時</summary>
        ROUND_END = 3,
    }

    /// <summary>ゲームサウンドナンバー</summary>
    public enum GAME_SOUND_NUMBER
    {
        /// <summary>ジャンプ</summary>
        JUMP = 0,
        /// <summary>着地</summary>
        LANDING = 1,
        /// <summary>加速</summary>
        ACCELERATION = 2,
        /// <summary>踏んだ時</summary>
        TREAD = 3,
        /// <summary>後ろから刺した時</summary>
        STAB_FROM_BEHIND = 4,
        /// <summary>走る</summary>
        RUN = 5,
        /// <summary>一定以上の速度</summary>
        FAST_SPEED = 6,
        /// <summary>アイテム取得</summary>
        GET_ITEM = 7,
        /// <summary>壁に衝突</summary>
        CPLLISON_WALL = 8,
    }

    public enum RESULT_SOUND_NUMBER
    {
        /// <summary>リザルト開始</summary>
        START = 0,
        /// <summary>勝敗判定</summary>
        JUDGMENT_VICTORY = 1,
        /// <summary>終了</summary>
         END = 2,
    }

    /// <summary>音</summary>
    [Header("サウンドエフェクト")] public AudioClip[] SetClip;

    /* 例
    internal void SetSoundEffect(int number)
    {
        SoundManager.Instance.PlaySE(SetClip[number]);
    }*/
}
/*
【タイトル】
・タイトルBGM
・ボタンを押したときの音
・ボタンをホバーしたときの音

【ラウンド開始前】
・操作方法が出るときの音
・ラウンド開始前の音（レディファイ）
・カウントダウン音
・スタート音

【ラウンド中】
・ジャンプ音
・着地音
・加速の音
・踏んだ時
・後ろから刺したときの音
・走ってる音
・速さが○○以上のときの音
・アイテム取得の音
・アイテム使用中のBGM ?
・ラウンド終了の音 ?
・壁に衝突したときの音
・対戦中のBGM（ラウンドごとに変える？）?

【ラウンド終了】
・死んで画面ズーム後に「カンカンカーン」みたいな決着の音*/