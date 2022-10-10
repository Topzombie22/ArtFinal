using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController player;

    public float playerSpeed;

    public float projectedSpeed;
    public float xSpeed;
    public float ySpeed;
    public float lerpSpeed;
    public float slerpSpeed;
    public float stopLerpSpeed;
    public Vector3 movementVector;


    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float yMomentum;
    [SerializeField]
    private float fallSpeed;
    [SerializeField]
    private float fallVelocity;
    [SerializeField]
    private float raycastDist;
    [SerializeField]
    private LayerMask playerMask;
    [SerializeField]
    private float rotationTimer;
    private float timeUntil;

    public bool isGrounded;
    public bool isAirborne;
    public bool isMoving;
    public bool isJumping;
    public bool freeRotation;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        MovementParse();
        JumpHandler();
        PlayerFallLerp();
        RotationSpeed();
        Timer();
    }


    void MovementParse()
    {
        float XInput = Input.GetAxisRaw("Horizontal");
        float YInput = Input.GetAxisRaw("Vertical");


        if (YInput != 0 || XInput != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (isGrounded == true)
        {
            if (XInput > 0)
            {
                xSpeed = Mathf.Lerp(xSpeed, projectedSpeed, lerpSpeed * Time.deltaTime);
            }
            else if (XInput < 0)
            {
                xSpeed = Mathf.Lerp(xSpeed, projectedSpeed * -1, lerpSpeed * Time.deltaTime);
            }
            else if (XInput == 0)
            {
                xSpeed = Mathf.Lerp(xSpeed, 0, stopLerpSpeed * Time.deltaTime);
            }


            if (YInput > 0)
            {
                ySpeed = Mathf.Lerp(ySpeed, projectedSpeed, lerpSpeed * Time.deltaTime);
            }
            else if (YInput < 0)
            {
                ySpeed = Mathf.Lerp(ySpeed, projectedSpeed * -1, lerpSpeed * Time.deltaTime);
            }
            else if (YInput == 0)
            {
                ySpeed = Mathf.Lerp(ySpeed, 0, stopLerpSpeed * Time.deltaTime);
            }
        }
        else if (!isGrounded)
        {
            if (XInput > 0)
            {
                xSpeed = Mathf.Lerp(xSpeed, projectedSpeed, lerpSpeed / 5f * Time.deltaTime);
            }
            else if (XInput < 0)
            {
                xSpeed = Mathf.Lerp(xSpeed, projectedSpeed * -1, lerpSpeed / 5f * Time.deltaTime);
            }


            if (YInput > 0)
            {
                ySpeed = Mathf.Lerp(ySpeed, projectedSpeed, lerpSpeed / 5f * Time.deltaTime);
            }
            else if (YInput < 0)
            {
                ySpeed = Mathf.Lerp(ySpeed, projectedSpeed * -1, lerpSpeed / 5f * Time.deltaTime);
            }
        }

        movementVector = new Vector3(xSpeed, 0, ySpeed);
        movementVector = Vector3.ClampMagnitude(movementVector, 1f);
        MovePlayer();
    }

    private void RotationSpeed()
    {
        if (freeRotation)
        {
            Quaternion lookRoto = Quaternion.LookRotation(movementVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRoto, slerpSpeed * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        player.Move(movementVector * playerSpeed * Time.deltaTime);
        Debug.Log(movementVector.magnitude);
    }

    private void PlayerFallLerp()
    {
        if (isGrounded)
        {
            yMomentum = Mathf.Lerp(yMomentum, 0, 10f * Time.deltaTime);
        }
        else if (!isGrounded)
        {
            yMomentum = Mathf.Lerp(yMomentum, fallSpeed, fallVelocity * Time.deltaTime);
        }
        player.Move(new Vector3(0, yMomentum, 0) * Time.deltaTime);
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDist, ~playerMask))
        {
            if (hit.transform.tag == "Ground")
            {
                isGrounded = true;
                isJumping = false;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void JumpHandler()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yMomentum = jumpForce;
            isJumping = true;
            isGrounded = false;
        }
    }

    private void Timer()
    {
        if (isMoving)
        {
            timeUntil = rotationTimer;
            freeRotation = true;
        }
        else if (!isMoving)
        {
            timeUntil -= Time.deltaTime;

            if (timeUntil <= 0)
            {
                freeRotation = false;
            }
        }
    }

}
