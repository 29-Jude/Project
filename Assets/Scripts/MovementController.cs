using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class MovementController : MonoBehaviour
{
    public float speed = 7.5f;
    public float sprintSpeed;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public string WinScene;
    public Animator anim;
    public Text CoinsCollected;
    public float Coins = 0;

    

    CharacterController characterController;
    [HideInInspector]
    public Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim.GetComponent<Animator>();
        rotation.y = transform.eulerAngles.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CoinsCollected.text = "Coins Collected: " + Coins;

    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
        }


        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);

            anim.SetBool("CanMove", true);
        }

        if(speed >= 1)
        {
            anim.SetBool("CanMove", true);
        }

        else
            anim.SetBool("CanMove", false);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed += sprintSpeed;
            anim.SetBool("Sprint", true);
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed -= sprintSpeed;
            anim.SetBool("Sprint", false);
        }

        CoinsCollected.text = "Coins Collected: " + Coins;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "WinZone")
        {
            SceneManager.LoadScene(WinScene);
        }

        else if(other.gameObject.tag == "Danger")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
        }

        else if(other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            Coins += 1;
        }
    }
}
