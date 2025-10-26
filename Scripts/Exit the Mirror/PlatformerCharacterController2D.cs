using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlatformerCharacterController2D : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Animator animator;

    private bool facingRight = true;
    private bool isGrounded;
    private bool isMiddleAir;
    private bool isMoving;
    private float groundTimer = 1f;
    private float airTimer;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    public UnityEvent jump;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        BetterJump();
        IsGround();
        Movement();
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(playerInput.actions["Horizontal"].ReadValue<float>() * 30f * speed * Time.deltaTime, rb.velocity.y);
        if (playerInput.actions["Horizontal"].ReadValue<float>() != 0)
            isMoving = true;
        else
            isMoving = false;

        if (!isGrounded)
            isMiddleAir = true;
        else
            isMiddleAir = false;
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsMiddleAir", isMiddleAir);
    }

    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && playerInput.actions["Jump"].ReadValue<float>() == 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void IsGround()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer) && airTimer <= 0)
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
            if (groundTimer < 0)
                isGrounded = false;
            else
                groundTimer -= Time.deltaTime;
        }
    }

    void Movement()
    {
        //rb.MovePosition(rb.position + new Vector2(Mathf.Abs(playerInput.actions["Horizontal"].ReadValue<float>() * speed * Time.deltaTime), 0f));
        //transform.Translate(Mathf.Abs(playerInput.actions["Horizontal"].ReadValue<float>()) * speed * Time.deltaTime, 0, 0);
        //rb.AddForce(Vector2.right * new Vector2(playerInput.actions["Horizontal"].ReadValue<float>() * speed * Time.deltaTime, 0f), ForceMode2D.Force);
        if (playerInput.actions["Jump"].triggered && isGrounded && airTimer <= 0)
        {
            jump.Invoke();
            rb.velocity = Vector2.zero;
            rb.velocity = Vector2.up * jumpForce;
            isGrounded = false;
            airTimer = 0.06f;
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
}
