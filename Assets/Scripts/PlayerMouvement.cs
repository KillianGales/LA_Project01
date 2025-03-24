using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get Rigidbody2D component
    }

    void Update()
    {
        // Get input from keyboard (WASD or Arrow Keys)
        movement.x = Input.GetAxisRaw("Horizontal"); // Left/Right
        movement.y = 0;
        movement.z = Input.GetAxisRaw("Vertical");   // Up/Down

        // Normalize movement so diagonal isn't faster
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody2D
        rb.linearVelocity = new Vector3 (movement.x, 0, movement.z) * moveSpeed;
        
    }
}

