using UnityEngine;

public class ResultSimpleJump : MonoBehaviour
{
    [SerializeField] private float StartJumpTime;
    [SerializeField] private float SetJumpTime;
    [SerializeField] private float GravityPower;
    [SerializeField] private float JumpPower;
    private float GroundHeight;
    internal float SetGroundHeight
    {
        set { GroundHeight = value + StartGroundHeight; }
    }
    internal bool JumpFlag;
    private float StartGroundHeight;
    private bool GroundFlag;
    private float JumpTime;
    private float Move;

    void Start()
    {
        JumpFlag = true;
        GroundFlag = true;
        StartGroundHeight = GroundHeight = this.transform.position.y;
        JumpTime = StartJumpTime;
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
            if (JumpFlag)
            {
                JumpTime -= Time.deltaTime;
                if (JumpTime <= 0.0f)
                {
                    Move = JumpPower;
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
            }
        }
    }
}
