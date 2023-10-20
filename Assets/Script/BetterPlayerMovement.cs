using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BetterPlayerMovement : MonoBehaviour
{
    private Animator anim;
    public PlayerData data;
    private Rigidbody2D rb; // 获得刚体


    public float LastOnGroundTime { get; private set; }
    public float LastInputJumpTime { get; private set; }


    public bool isJumping; // 判断是否跳跃
    public bool isJumpFalling;
    public bool jumpCut;
    private Vector2 moveInput; // 获取键盘输入的左右方向数值

    private bool isFacingRight;

    [Header("Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Layer & Tag")]
    [SerializeField] private LayerMask groundLayer;
    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        isFacingRight = false;
    }

    void Update()
    {
        #region 时间
        LastOnGroundTime -= Time.deltaTime;
        LastInputJumpTime -= Time.deltaTime;

        #endregion

        #region 输入
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");


        if (Input.GetKeyDown(KeyCode.Space))
        {
            LastInputJumpTime = data.jumpInputBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isJumping && rb.velocity.y > 0)
            {
                jumpCut = true;
            }

        }
        #endregion



        if (moveInput.x != 0)
        {

        }
            CheckDirectionToFace(moveInput.x > 0);


        AnimChange();

        #region 物理检测
        CheckOnGround();

        #endregion


        #region 重力
        if (rb.velocity.y < 0 && moveInput.y < 0)
        {
            rb.gravityScale = data.gravityScale * data.fastFallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fastFallMaxSpeed));
        }
        else if (jumpCut)
        {
            rb.gravityScale = data.gravityScale * data.jumpCutGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fallMaxSpeed));
        }
        else if ((isJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) > data.jumpHangTimeThreshold)
        {
            rb.gravityScale = data.gravityScale * data.jumpHangGravityMult;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = data.gravityScale * data.fallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -data.fallMaxSpeed));
        }
        else
            rb.gravityScale = data.gravityScale;
        #endregion

        #region 跳跃检测
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
            isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !isJumping)
        {
            jumpCut = false;
            isJumpFalling = false;
        }

        if (LastInputJumpTime > 0 && LastOnGroundTime > 0)
        {
            isJumping = true;
            isJumpFalling = false;
            jumpCut = false;
            Jump();
        }
        #endregion  
    }

    void FixedUpdate()
    {
        Run(1);
    }


    // 改变动画
    void AnimChange()
    {
        if (moveInput.x != 0) anim.SetBool("run", true);
        else anim.SetBool("run", false);
    }


    void Jump()
    {
        LastInputJumpTime = 0;
        LastOnGroundTime = 0;


        float force = data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }
    void Run(float lerpAmount)
    {
        float targetSpeed = moveInput.x * data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);
        float accelRate;

        //获取加速度，加速快，减速更快
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;

        if ((isJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < data.jumpHangTimeThreshold)
        {
            accelRate *= data.jumpHangAccelerationMult;
            targetSpeed *= data.jumpHangMaxSpeedMult;
        }

        // 为了时添加力后的速度最终提高到我们设定的加速度，通过速度差来提高速度，并且这样会有更舒适的手感
        float speedDif = targetSpeed - rb.velocity.x;

        // 添加加速度等同于提高加速的倍率
        float movement = speedDif * accelRate;
        Debug.Log(accelRate);
        // 添加力移动玩家
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }


    void OnDrawGizmosSelected()
    {
        // Gizmos.color = Color.black;
        Gizmos.DrawCube(groundCheckPoint.position, groundCheckSize);
    }

    void CheckOnGround()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            LastOnGroundTime = data.coyateTime;
        }
    }

    void CheckDirectionToFace(bool isMovingRight)
    {
        if (isFacingRight != isMovingRight)
            Turn();
    }

    void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }


}
