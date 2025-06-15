using UnityEngine;

//=============================================================================
/// プレイヤーの地面チェックを行うクラス
public class GroundCheck : MonoBehaviour
{
    ///--------------------------------------------------------------
    ///						 public変数

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // 地面のタグ
    private string groundTag_ = "Ground";
    //========================================
    // 接触しているかどうかのフラグ
    private bool isGround_ = false;
    // 地面との状態
    private bool isGroundEnter_, isGroundStay_, isGroundExit_;

    ///--------------------------------------------------------------
    ///						 初期化
    void Start()
    {
        
    }
    ///--------------------------------------------------------------
    ///						 更新
    void Update()
    {
        
    }

    ///--------------------------------------------------------------
    ///						 地面との接触状態関数
    // NOTE: この関数は、プレイヤーが地面に接触しているかどうかを返す
public bool IsGround() {
    // 接地状態の更新
    bool currentGroundState = isGround_;
    
    if (isGroundEnter_ || isGroundStay_) {
        currentGroundState = true;
    }
    else if (isGroundExit_) {
        currentGroundState = false;
    }
    // 何もトリガーイベントが発生していない場合は前回の状態を保持しない
    else if (!isGroundEnter_ && !isGroundStay_ && !isGroundExit_) {
        // 1フレーム何も検出されなかった場合は地面から離れたとみなす
        currentGroundState = false;
    }

    isGround_ = currentGroundState;

    // 接地判定のリセット
    isGroundEnter_ = false;
    isGroundStay_ = false;
    isGroundExit_ = false;

    return isGround_;
}

    ///--------------------------------------------------------------
    ///						 判定に接触
    private void OnTriggerEnter2D(Collider2D collision) {
        //========================================
        // 何かと接触している
        //Debug.Log("[INFO] : 足が何かと接触しています");
        //========================================
        // 地面に触れたときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isGroundEnter_ = true;
            // デバッグログを出力
            //Debug.Log("[INFO] : Player is on the ground.");
        }
    }

    ///--------------------------------------------------------------
    ///                     判定に接触し続けている
    private void OnTriggerStay2D(Collider2D collision) {
        //========================================
        // 何かと接触し続けている
        //Debug.Log("[INFO] : 足が何かと接触し続けています");
        //========================================
        // 地面に触れ続けているときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isGroundStay_ = true;
            // デバッグログを出力
            //Debug.Log("[INFO] : Player is still on the ground.");
        }
    }

    ///--------------------------------------------------------------
    ///                     判定から離れた
    private void OnTriggerExit2D(Collider2D collision) {
        //========================================
        // 何かと接触が離れた
        //Debug.Log("[INFO] : 足が何かとの接触を離れました");

        // 地面から離れたときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isGroundExit_ = true;
            // デバッグログを出力
            //Debug.Log("[INFO] : Player has left the ground.");
        }
    }
}
