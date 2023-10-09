
using UnityEngine;

public class PlayerMovement_3 : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private float horizontalmove_float;
    public int speed;
    public float raycastLength;
    public LayerMask layer;

    void Update()
    {
        horizontalmove_float = Input.GetAxis("Horizontal");
        Movement();
        // Debug.DrawRay(transform.position, Vector2.down, Color.blue);//  画出斜面检测的线

    }

    private void FixedUpdate()
    {
        Movement();
    }

    public virtual void Movement()
    {
        if (!IsSlope())
            rb.velocity = new Vector2(horizontalmove_float * speed * Time.fixedDeltaTime, rb.velocity.y);
        else
            rb.velocity = SlopeVector() * horizontalmove_float * speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// 计算斜坡的方向向量
    /// </summary>
    /// <returns>返回斜坡方向的单位向量</returns>
    private Vector2 SlopeVector()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, raycastLength, layer);//    碰撞检测斜面
        Vector3 currrentVector = new Vector3(Mathf.Abs(transform.position.x), Mathf.Abs(transform.position.y));// 玩家当前位置得转换为绝对值，否则可能会根据玩家的位置导致屏幕方向向量反向
        return (Vector2)Vector3.ProjectOnPlane(currrentVector, hit.normal).normalized;
    }

    /// <summary>
    /// 判断是否处于斜面上
    /// </summary>
    /// <param name="rb">2d刚体</param>
    /// <returns>返回bool</returns>
    private bool IsSlope()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(rb.position, Vector2.down, raycastLength, layer);
        if (hit != false)
        {
            if (hit.normal != Vector2.up)   //  根据斜面的法向量与垂直地面的法向量进行对比方向是否一样判断是不是在斜面上
            {
                return true;
            }
        }
        return false;
    }
}
