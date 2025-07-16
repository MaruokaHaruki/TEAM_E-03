using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ResultSimpleJump : MonoBehaviour
{
    [SerializeField] private float StartJumpTime = 2.0f;
    [SerializeField] private float SetJumpTime = 0.5f;
    [SerializeField] private float GravityPower = 200.0f;
    [SerializeField] private float JumpPower = 50.0f;
    private float GroundHeight;
    internal float SetGroundHeight
    {
        set { GroundHeight = value + StartGroundHeight; }
    }
    internal bool JumpFlag;
    private float StartGroundHeight;
    internal bool GroundFlag;
    private float JumpTime;
    private float Move;

    [SerializeField] private GameObject PlayerStartSprite;
    [SerializeField] private GameObject PlayerJumpSprite;
    [SerializeField] private bool JumpSpriteChangeFlag = true;

    void Awake()
    {
        JumpFlag = true;
        GroundFlag = true;
        StartGroundHeight = GroundHeight = this.transform.position.y;
        JumpTime = StartJumpTime;
        SetPlayerSprite(0);
    }

    void FixedUpdate()
    {
        Move -= GravityPower * Time.deltaTime;
        this.transform.position += (Vector3.up * Move * Time.deltaTime);
        if (this.transform.position.y <= GroundHeight)
        {
            Move = 0.0f;
            this.transform.position = new Vector3(this.transform.position.x, GroundHeight, this.transform.position.z);
        }

        if (GroundFlag == true)
        {
            JumpTime -= Time.deltaTime;
            if (JumpTime <= 0.0f)
            {
                if (JumpFlag)
                {
                    Move = JumpPower;
                    GroundFlag = false;
                    SetPlayerSprite(1);
                }
                else
                {
                    GroundFlag = false;
                }
            }
        }
        else
        {

            if (this.transform.position.y <= GroundHeight)
            {
                GroundFlag = true;
                JumpTime = SetJumpTime;
                SetPlayerSprite(0);
            }
        }
    }

    private void SetPlayerSprite(int setNumber)
    {
        if (JumpSpriteChangeFlag)
        {

            bool[] set = { false, false };

            switch (setNumber)
            {
                case 0:
                    set[0] = true;
                    break;

                case 1:
                    set[1] = true;
                    break;
            }

            PlayerStartSprite.SetActive(set[0]);
            PlayerJumpSprite.SetActive(set[1]);
        }
    }
}
