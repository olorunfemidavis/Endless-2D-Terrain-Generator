using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	
    public Camera playerCamera;
    public float speed = 1f;

    private CharacterController2D controller;

    private bool jump, crouch;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

		
        playerCamera.transparencySortMode = TransparencySortMode.Orthographic;
        controller = GetComponent<CharacterController2D>();
    }

    void Update()
    {    
        
        if (controller.IsGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                jump=true;
            }
            if (Input.touchCount > 0)
            {
                jump = true;
            }
           
        }
        controller.Move(Input.GetAxis("Horizontal") * speed, jump);


        //After we move, adjust the camera to follow the player
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 10, playerCamera.transform.position.z);
    }
}
