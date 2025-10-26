using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float lowJumpMultiplier = 2f;
    [SerializeField] int divingDegree = 0;

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
    private int timeMultiplyer = 0;

    public UnityEvent jump;
    public UnityEvent levelEnd;
    public UnityEvent gameOver;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        Cursor.visible = false;
    }

    void Update()
    {
        BetterJump();
        IsGround();
        Movement();
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(playerInput.actions["Horizontal"].ReadValue<float>() * timeMultiplyer * 30f * speed * Time.deltaTime, rb.velocity.y);
        if (rb.velocity.x != 0)
            isMoving = true;
        else
        {
            isMoving = false;
            timeMultiplyer = 0;
        }
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
            StartCoroutine(JumpTimer(0.15f));
        }
        if (playerInput.actions["Horizontal"].ReadValue<float>() != 0 && !isMoving)
        {
            StartCoroutine(MoveTimer(0.15f));
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

    private IEnumerator JumpTimer(float time)
    {
        yield return new WaitForSeconds(time * divingDegree);
        jump.Invoke();
        rb.velocity = Vector2.zero;
        rb.velocity = Vector2.up * jumpForce;
        isGrounded = false;
        airTimer = 0.06f;
    }
    private IEnumerator MoveTimer(float time)
    {
        timeMultiplyer = 0;
        yield return new WaitForSeconds(time * divingDegree);
        timeMultiplyer = 1;
    }
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("scene", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            gameOver.Invoke();
            StartCoroutine("GameOver");
        }
        if (collision.gameObject.tag == "EndLevel")
        {
            levelEnd.Invoke();
            StartCoroutine("EndTimer");
        }
    }
}
