using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private float horizontalmove_float;
    public int speed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            rb.velocity = new Vector2(horizontalmove_float * speed, rb.velocity.y);
    }

}
