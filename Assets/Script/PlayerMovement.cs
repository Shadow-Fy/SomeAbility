using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private float horizontalmove_float;
    public int speed;
    public float raycastLength;
    float angle;
    public LayerMask layer;

    void Start()
    {

    }

    void Update()
    {
        horizontalmove_float = Input.GetAxis("Horizontal");
        Movement();
    }

    private void FixedUpdate()
    {
        Movement();
    }


    public virtual void Movement()
    {
        rb.velocity = new Vector2(horizontalmove_float * speed * Time.deltaTime, rb.velocity.y);
    }

}