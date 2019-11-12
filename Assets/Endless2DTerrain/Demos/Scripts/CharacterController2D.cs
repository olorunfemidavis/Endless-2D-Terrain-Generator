using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
   public float JumpForce = 400f;                          // Amount of force added when the player jumps.
    public float MovementSmoothing = .05f;  // How much to smooth out the movement
    public bool AirControl = false;                         // Whether or not a player can steer while jumping;
    public LayerMask WhatIsGround;                          // A mask determining what is ground to the character
    public Transform GroundCheck;                           // A position marking where to check if the player is grounded.

    const float GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool Grounded, wasGrounded;            // Whether or not the player is grounded.
    const float CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D rb2D;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public bool IsGrounded { get => Grounded;}

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    public void Move(float move, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (Grounded || AirControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, rb2D.velocity.y);
            // And then smoothing it out and applying it to the character
            rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (Grounded && jump)
        {
            // Add a vertical force to the player.
            Grounded = false;
            rb2D.AddForce(new Vector2(0f, JumpForce));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}