using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    public float moveSpeed;
    Rigidbody2D body;
    Vector2 moveDir;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    // FixedUpdate is good for physics calculation since it is updated independently from frame rate
    void FixedUpdate()
    {
        Move();
    }

    // Player's Input System
    void InputManagement()
    {
        // Set X to be Horizontal movement and Y to be Vertical movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Get ONLY direction of movement by normalizing the Vector's length to 1
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        // Get the direction then multiply it with moveSpeed to get the velocity
        body.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
}
