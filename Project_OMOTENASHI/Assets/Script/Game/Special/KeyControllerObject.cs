using UnityEngine;

public class KeyControllerObject : MonoBehaviour
{
    /// <summary>変更確認用オブジェクト</summary>
    [SerializeField, Header("変更確認用オブジェクト")] private KeyObjectData[] ChangeCheckObject;

    /// <summary>変更するキー選択用enum</summary>
    enum CHANGE_KEY_TYPE
    {
        PLAYER_A_JUMP = 0,
        PLAYER_A_ACCELERATION,
        PLAYER_B_JUMP,
        PLAYER_B_ACCELERATION,
    }

    /// <summary>変更するキー</summary>
    [SerializeField, Header("変更するキー")] private CHANGE_KEY_TYPE ChangeKeyType;

    private void Update()
    {
        KeyCode setKey = KeyCode.None;
        float hitRange = 0.0f;

        Vector2 playerLeftAndRightPos = new Vector2(this.transform.position.x - (this.transform.localScale.x * 0.5f), this.transform.position.x +(this.transform.localScale.x * 0.5f));

        for (int i = 0; i < ChangeCheckObject.Length; i++)
        {
            if (HitCheck(this.transform.position, this.transform.localScale, ChangeCheckObject[i].HitFieldPos, ChangeCheckObject[i].HitFieldSize))
            {
                float objectHitRange = 0.0f;

                if (playerLeftAndRightPos.x < ChangeCheckObject[i].GetLeftAndRightPos().x)
                {
                    objectHitRange = Mathf.Abs(ChangeCheckObject[i].GetLeftAndRightPos().x - playerLeftAndRightPos.y);
                }
                else if (playerLeftAndRightPos.y > ChangeCheckObject[i].GetLeftAndRightPos().y)
                {
                    objectHitRange = Mathf.Abs(playerLeftAndRightPos.x - ChangeCheckObject[i].GetLeftAndRightPos().y);
                }
                else
                {
                    objectHitRange = Mathf.Abs(playerLeftAndRightPos.x - playerLeftAndRightPos.y);
                }

                if (objectHitRange > hitRange)
                {
                    setKey = ChangeCheckObject[i].SetKey;
                }
            }
        }

        if (setKey != KeyCode.None)
        {
            switch (ChangeKeyType)
            {
                case CHANGE_KEY_TYPE.PLAYER_A_JUMP:
                    NowMoveKey.Instance.JumpPlayerAKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_A_ACCELERATION:
                    NowMoveKey.Instance.AccelerationPlayerAKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_B_JUMP:
                    NowMoveKey.Instance.JumpPlayerBKey = setKey;
                    break;

                case CHANGE_KEY_TYPE.PLAYER_B_ACCELERATION:
                    NowMoveKey.Instance.AccelerationPlayerBKey = setKey;
                    break;
            }
        }
    }

    private bool HitCheck(Vector2 srcPos, Vector2 srcSize, Vector2 dstPos, Vector2 dstSize)
    {
        bool hitFlag = true;
/*
        // X軸
        if ((srcPos.x - ()) && ())
        {
            hitFlag = false;
        }

        // Y軸
        if (hitFlag && (() && ()))
        {
            hitFlag = false;
        }*/

        return hitFlag;
    }
}