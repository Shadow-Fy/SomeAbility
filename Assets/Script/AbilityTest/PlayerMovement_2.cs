using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_2 : MonoBehaviour
{
    private Rigidbody2D rb;

    public float normalAcceleration;

    [HideInInspector] public float acceleration;
    [HideInInspector] public Vector2 movementInput;

    public Transform arrow;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        acceleration = normalAcceleration;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        arrow.up = (mousePos - (Vector2)transform.position).normalized;
    }

    private void FixedUpdate()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity += movementInput * acceleration * Time.fixedDeltaTime;
    }
}
