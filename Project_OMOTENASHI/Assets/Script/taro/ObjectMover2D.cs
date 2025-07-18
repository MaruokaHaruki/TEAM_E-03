using UnityEngine;
using DG.Tweening; // DOTweenのアセットを使用するために必要です

public class ObjectMover2D : MonoBehaviour
{
    [Header("アニメーション設定")]
    [Tooltip("On状態の時にY座標をどれだけずらすか")]
    [SerializeField]
    private float yOffset = 2f;

    [Tooltip("アニメーションの時間（秒）")]
    [SerializeField]
    private float duration = 0.4f;

    [Tooltip("アニメーションの変化の仕方（行きも戻りも同じ動き）")]
    [SerializeField]
    private Ease ease = Ease.OutBack; // 行きと戻りで共通のEaseを使用

    private Vector3 startPosition;       // アニメーション前の初期座標
    private bool isStateOn = false;      // 現在の状態 (true: On, false: Off)

    void Awake()
    {
        // 起動時のワールド座標を初期座標として保存
        startPosition = transform.position;
    }

    void Update()
    {
        // Spaceキーが押された瞬間を検出したら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 状態を切り替えるメソッドを呼び出す
            ToggleState();
        }
    }

    /// <summary>
    /// オブジェクトの状態を切り替えます。
    /// </summary>
    public void ToggleState()
    {
        // ★重要：既存のアニメーションをその場で停止させる
        // これにより、アニメーションの途中でもスムーズに反転できる
        transform.DOKill();

        // 現在の状態を反転させる
        isStateOn = !isStateOn;

        // On状態なら目標位置を計算し、Off状態なら初期位置に戻す
        Vector3 targetPosition = isStateOn ? startPosition + new Vector3(0, yOffset, 0) : startPosition;

        // 計算した目標位置へアニメーションを開始
        transform.DOMove(targetPosition, duration).SetEase(ease);
    }
}