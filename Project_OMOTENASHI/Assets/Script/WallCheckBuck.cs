using UnityEngine;

public class WallCheckBuck : MonoBehaviour {
    ///--------------------------------------------------------------
    ///						 public変数

    ///--------------------------------------------------------------
    ///						 private変数
    //========================================
    // 地面のタグ
    private string groundTag_ = "Ground";
    // 接触しているかどうかのフラグ
    private bool isHitWallBuck_ = false;
    // 地面との状態
    private bool isHitWallBuckEnter_, isHitWallBuckStay_, isHitWallBuckExit_;
    //========================================
    // 敵のタグ
    private string enemyTag_ = "Player";
    // 敵にあたっているかどうか
    private bool isHitEnemy_ = false;
    // 敵との状態
    private bool isHitEnemyEnter_, isHitEnemyStay_, isHitEnemyExit_;

    ///--------------------------------------------------------------
    ///						 初期化
    void Start() {

    }
    ///--------------------------------------------------------------
    ///						 更新
    void Update() {

    }

    ///--------------------------------------------------------------
    ///						 地面との接触状態関数
    // NOTE: この関数は、プレイヤーが地面に接触しているかどうかを返す
    public bool IsHitWallBuck() {
        //========================================
        // 接地状態の更新
        if (isHitWallBuckEnter_ || isHitWallBuckStay_) {
            // 地面に接触している場合はフラグを立てる
            isHitWallBuck_ = true;
        }
        else if (isHitWallBuckExit_) {
            // 地面から離れた場合はフラグを下げる
            isHitWallBuck_ = false;
        }

        //========================================
        // 接地判定のリセット
        isHitWallBuckEnter_ = false;
        isHitWallBuckStay_ = false;
        isHitWallBuckExit_ = false;

        //========================================
        // 接地状態を返す
        return isHitWallBuck_;
    }

    ///--------------------------------------------------------------
    ///						 敵との接触状態関数
    public bool IsHitEnemyBuck() {
        //========================================
        // 敵に接触している状態の更新
        if (isHitEnemyEnter_ || isHitEnemyStay_) {
            // 敵に接触している場合はフラグを立てる
            isHitEnemy_ = true;
        }
        else if (isHitEnemyExit_) {
            // 敵から離れた場合はフラグを下げる
            isHitEnemy_ = false;
        }
        //========================================
        // 接地判定のリセット
        isHitEnemyEnter_ = false;
        isHitEnemyStay_ = false;
        isHitEnemyExit_ = false;
        //========================================
        // 接地状態を返す
        return isHitEnemy_;
    }

    ///--------------------------------------------------------------
    ///						 判定に接触
    private void OnTriggerEnter2D(Collider2D collision) {

        //========================================
        // 地面に触れたときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isHitWallBuckEnter_ = true;
        }
        //========================================
        // 敵に触れたときの処理
        if (collision.gameObject.CompareTag(enemyTag_)) {
            // 敵に触れたときのフラグを立てる
            isHitEnemyEnter_ = true;
        }
    }

    ///--------------------------------------------------------------
    ///                     判定に接触し続けている
    private void OnTriggerStay2D(Collider2D collision) {
        //========================================
        // 地面に触れ続けているときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isHitWallBuckStay_ = true;
        }
        //========================================
        // 敵に触れ続けているときの処理
        if (collision.gameObject.CompareTag(enemyTag_)) {
            // 敵に触れ続けているときのフラグを立てる
            isHitEnemyStay_ = true;
        }
    }

    ///--------------------------------------------------------------
    ///                     判定から離れた
    private void OnTriggerExit2D(Collider2D collision) {
        //========================================
        // 地面から離れたときの処理
        if (collision.gameObject.CompareTag(groundTag_)) {
            // 設置判定のフラグを立てる
            isHitWallBuckExit_ = true;
        }
        //========================================
        // 敵から離れたときの処理
        if (collision.gameObject.CompareTag(enemyTag_)) {
            // 敵から離れたときのフラグを立てる
            isHitEnemyExit_ = true;
        }
    }
}
