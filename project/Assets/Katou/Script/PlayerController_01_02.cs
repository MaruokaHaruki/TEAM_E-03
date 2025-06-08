using UnityEngine;

public class PlayerController_01_02 : MonoBehaviour
{
    /// <summary>
    /// 移動ベクトル
    /// </summary>
    private Vector2 MoveVec;

    /// <summary>
    /// 常に影響する移動ベクトル（基本移動速度）
    /// </summary>
    [SerializeField, Header("常に影響する移動ベクトル")] private Vector2 AlwaysVec;

    // Kキー及びDキー連打による加速システム用の変数
    [SerializeField, Header("パワー最大値")] private float maxPower = 100f;
    [SerializeField, Header("1回のキー入力でのパワー増加量")] private float powerIncreaseAmount = 10f;
    [SerializeField, Header("1秒あたりのパワー減少量")] private float powerDecreaseRate = 5f;
    [SerializeField, Header("パワーを速度に変換する係数")] private float powerToSpeedFactor = 0.1f;
    private float currentPower = 0f;

    /// <summary>
    /// Kキーを使用するかのフラグ（falseの場合はDキー）
    /// </summary>
    [SerializeField] private bool KeyKFlag;

    /// <summary>
    /// プレイヤーのHP
    /// </summary>
    public int Hp;

    /// <summary>
    /// 時間管理用変数
    /// </summary>
    float time;

    /// <summary>
    /// 無敵状態の判定時間
    /// </summary>
    [SerializeField] private float CheckTime;

    /// <summary>
    /// 通常状態のマテリアル
    /// </summary>
    [SerializeField] private Material IdeMaterial;
    
    /// <summary>
    /// 無敵状態のマテリアル
    /// </summary>
    [SerializeField] private Material InvincibleMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hp = 1;
        currentPower = 0f; // 初期パワーを0に設定
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        if (time < CheckTime) {
            // 無敵状態(七色にひかる)
            // スプライトの色を七色に変更する
            GetComponent<SpriteRenderer>().material = InvincibleMaterial;
            if (InvincibleMaterial != null) // Nullチェックを追加
            {
                GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1)); // HSVToRGBのHは0-1の範囲が良いのでPingPongを使用
            }
        }
        else {
            // 通常状態(元の色に戻る)
            //GetComponent<SpriteRenderer>().material = IdeMaterial;
        }

        MoveVec = Vector2.zero;
        MoveVec += AlwaysVec;

        // キーが押されていない場合、パワーを減少させる
        KeyCode targetKey = KeyKFlag ? KeyCode.K : KeyCode.D;
        if (!Input.GetKey(targetKey)) {
            currentPower -= powerDecreaseRate * Time.deltaTime;
            if (currentPower < 0) {
                currentPower = 0;
            }
        }

        // KキーまたはDキーの入力でパワー増加
        if (KeyKFlag) {
            if (Input.GetKeyDown(KeyCode.K)) {
                currentPower += powerIncreaseAmount;
                if (currentPower > maxPower) {
                    currentPower = maxPower;
                }
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.D)) {
                currentPower += powerIncreaseAmount;
                if (currentPower > maxPower) {
                    currentPower = maxPower;
                }
            }
        }

        // パワーを速度に変換してMoveVecに加算
        float poweredSpeed = currentPower * powerToSpeedFactor;
        // プレイヤーの向きに応じて加速方向を決定
        MoveVec.x += Mathf.Sign(AlwaysVec.x) * poweredSpeed;


        // 位置更新
        this.transform.position += (Vector3)MoveVec * Time.deltaTime;

        // HPが0以下になったら破棄
        if (Hp <= 0) {
            Destroy(this.gameObject);
            if (SceneManagerScript.Instance) {
                if (KeyKFlag) {
                    SceneManagerScript.Instance.winnerName = "PlayerB";
                }
                else {
                    SceneManagerScript.Instance.winnerName = "PlayerA";
                }
                SceneManagerScript.Instance.FadeOutScene("Result");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁との衝突処理
        if (collision.gameObject.tag == "Wall")
        {
            if (!OnGroundCheckObject(collision.gameObject)) {
                time = 0; // 無敵時間リセット
                SetMoveVecs(collision.transform.position.x > this.transform.position.x);
                currentPower = 0; // 壁に衝突したらパワーリセット
            }
        }

        // 他のプレイヤーとの衝突処理
        if (collision.gameObject.tag == "Player")
        {
            // 無敵状態なら相手を破棄
            if (time < CheckTime)
            {
                Destroy(collision.gameObject);
            }
            if (!OnGroundCheckObject(collision.gameObject)) {

                // 衝突位置に応じて移動方向を設定し、少し離す
                if (collision.transform.position.x < this.transform.position.x) {
                    SetMoveVecs(false);
                    this.transform.position += new Vector3(0.1f, 0f);
                }
                else {
                    SetMoveVecs(true);
                    this.transform.position += new Vector3(-0.1f, 0f);
                }
                currentPower = 0; // 他プレイヤーに衝突したらパワーリセット
            }
        }
    }

    /// <summary>
    /// 取得したオブジェクトが自分の下にある場合true
    /// </summary>
    public bool OnGroundCheckObject(GameObject checkObject) {
        if ((GetObjectSize(checkObject, 1) > GetObjectSize(this.gameObject, -1)) &&
            (GetObjectSize(checkObject, -1) < GetObjectSize(this.gameObject, 1)) &&
            (checkObject.transform.position.y < this.transform.position.y)) {
            return true;
        }
        return false;
    }

    private float GetObjectSize(GameObject getObject, float direction) {
        return ((getObject.transform.lossyScale.x * 0.5f * direction) + getObject.transform.position.x);
    }

    /// <summary>
    /// 移動方向を設定する
    /// </summary>
    /// <param name="leftVecFlag">左方向に移動するかのフラグ</param>
    private void SetMoveVecs(bool leftVecFlag)
    {
        if (!leftVecFlag)
        {
            // 右方向の移動設定
            AlwaysVec.x = Mathf.Abs(AlwaysVec.x);
            // KeyVec.x = Mathf.Abs(KeyVec.x); // 削除またはコメントアウト
            // SetKeyVec.x = Mathf.Abs(SetKeyVec.x); // 削除またはコメントアウト
            // KeyPlusAmount = Mathf.Abs(KeyPlusAmount); // 削除またはコメントアウト
            // KeyMinusAmount = -Mathf.Abs(KeyMinusAmount); // 削除またはコメントアウト

            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            // 左方向の移動設定
            AlwaysVec.x = -Mathf.Abs(AlwaysVec.x);
            // KeyVec.x = -Mathf.Abs(KeyVec.x); // 削除またはコメントアウト
            // SetKeyVec.x = -Mathf.Abs(SetKeyVec.x); // 削除またはコメントアウト
            // KeyPlusAmount = -Mathf.Abs(KeyPlusAmount); // 削除またはコメントアウト
            // KeyMinusAmount = Mathf.Abs(KeyMinusAmount); // 削除またはコメントアウト

            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    /// <summary>
    /// 移動方向を取得する
    /// </summary>
    /// <returns>移動方向（1:右, -1:左）</returns>
    public float GetMoveDirection()
    {
        return Mathf.Sign(AlwaysVec.x);
    }

    /// <summary>
    /// 移動ベクトルを取得する
    /// </summary>
    /// <returns>現在の移動ベクトル</returns>
    public Vector2 GetMoveVec()
    {
        return MoveVec;
    }
}