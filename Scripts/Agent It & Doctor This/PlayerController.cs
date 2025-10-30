using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    /*
    Setup : 
    - Add Cinemachine and Input System to your project
    - Allow Physical Keys
    - Create a Physics material 2D with friction at 0.0001
    - Add a layer called Ground and assign it to your ground
    - Change your player's rigibody's Interpolate to Interpolate
    - Add a Jump and a Horizontal action on your playerInput
    - Put Speed at 12; JumpForce at 9; FallMultiplier at 5 and LowJumpMultiplier at 2
    - Add a Cinemachine Camera with Follow Player, OrthoSize 7 and Priority 10
    */

    // Initializing variables
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private GameObject overallCam;
    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject endPlayer;

    private bool facingRight = true;
    private bool isGrounded;

    private float groundTimer = 1f;
    private float airTimer;
    private PlayerStateManager psm;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    // private bool isSettingUp = true;

    public UnityEvent jump;
    //public UnityEvent levelEnd;
    //public UnityEvent gameOver;
    public UnityEvent onDied;

    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool audioDied = false;
    public bool Grounded => isGrounded;

    [SerializeField]
    private float jumpDebDelay = 0.2f;
    private float jumpDebounce = 0.0f;

    [SerializeField] private Animator animator;

    // Setup components
    private void Awake()
    {
        psm = GetComponent<PlayerStateManager>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        isDead = false ;
#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void Start()
    {
        overallCam.SetActive(false);
    }

    // Calling functions on every frame
    void Update()
    {
        BetterJump();
        IsGround();
        Movement();
        //ChoosePlace();

        if (playerInput.actions["CamSwitch"].triggered) overallCam.SetActive(!overallCam.activeSelf);
        if (playerInput.actions["RecordEnd"].triggered) psm.EndRecord();
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerInput.actions["Horizontal"].ReadValue<float>() * 30f * speed * Time.deltaTime, rb.linearVelocity.y);

        if (isDead) Respawn();
        

        if (rb.linearVelocity.x != 0)
            animator.SetBool("isMoving", true);
        else
        {
            animator.SetBool("isMoving", false);
        }
        
        /*if (playerInput.actions["Jump"].ReadValue<float>() != 1 || isGrounded)
        {
            animator.SetBool("Jump", false);
        }*/
    }


    // Handling gravity and fall speed values to make the jump better and more controllable
    void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && playerInput.actions["Jump"].ReadValue<float>() == 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // Checking if player grounded + coyote time
    void IsGround()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.10f, groundLayer) && airTimer <= 0)
        {
            groundTimer = 0.15f;
            isGrounded = true;
        }
        if (airTimer > 0)
        {
            airTimer -= Time.deltaTime;
        }
        else
        {
            if (groundTimer >= 0)
                groundTimer -= Time.deltaTime;
            else
                isGrounded = false;
        }

    }

    // Manage jumping input and flipping
    void Movement()
    {
        if (playerInput.actions["Jump"].ReadValue<float>() == 1 && isGrounded)
        {
            

            rb.linearVelocity = Vector2.zero;
            // rb.linearVelocity = Vector2.up * jumpForce;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;

            float time = Time.time;
            if (Time.time >= jumpDebounce + jumpDebDelay) {
                jumpDebounce = time;
                //animator.SetBool("Jump", true);
                //animator.SetBool("isMoving", false);
                animator.SetTrigger("JumpTrig");
                jump.Invoke();
            }
        }

        if (playerInput.actions["Horizontal"].ReadValue<float>() > 0 && !facingRight)
            Flip();
        else if (playerInput.actions["Horizontal"].ReadValue<float>() < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

/*
    private void ChoosePlace()
    {
        return;
        // This code should never execute, this is not how it is done
        if (playerInput.actions["Validate"].triggered)
        {
            Debug.Log("Bouton Appuyé !!!");
            if (isSettingUp)
            {
                isSettingUp = false;
                Instantiate(endPlayer, this.transform.position, Quaternion.identity);
                gameObject.transform.SetPositionAndRotation(spawn.position, Quaternion.identity);
                if (!facingRight) transform.Rotate(0f, 180f, 0f);
            }
        }
    }
    */
    void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.gameObject.tag == "Dangerous")
        {
            isDead = true;
            audioDied = true;
            onDied.Invoke();
        }
    }

    public void Respawn()
    {
        gameObject.transform.SetPositionAndRotation(spawn.position, Quaternion.identity);
        if (!facingRight) transform.Rotate(0f, 180f, 0f);

        isDead = false;
        audioDied = true;
    }
}