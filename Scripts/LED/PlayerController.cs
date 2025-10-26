using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier;
    [SerializeField] float jumpGravity;
    [SerializeField] float fallGravity;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask platformLayer;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject playerCorpse;
    [SerializeField] GameObject hardcoreLogo;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem dust;
    [SerializeField] ParticleSystem repair;
    [SerializeField] ParticleSystem glassHit;
    [SerializeField] ParticleSystem glassDeath;
    [SerializeField] ParticleSystem dustJump;
    [SerializeField] Color yellow;
    [SerializeField] Color green;
    [SerializeField] Color blue;
    [SerializeField] Color red;

    private bool isGrounded;
    private bool isMiddleAir;
    private bool isMoving = false;
    private bool isRotating;
    private bool canGetHit = true;
    private bool dustBegan = false;
    private bool corpseSpawned = false;
    private bool enemyHit = false;
    private int life = 2;
    private int lightColor = 0;
    private float rotationValue = 1f;
    private float groundTimer = 1f;
    private float movingmultiplier;
    private float airTimer;

    private Rigidbody2D rb;
    private PlayerControls playerInput;
    private Light2D playerLight;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Training") == 0) PlayerPrefs.SetInt("CurrentLevelID", SceneManager.GetActiveScene().buildIndex);
        rb = GetComponent<Rigidbody2D>();
        playerInput = new PlayerControls();
        playerLight = GetComponent<Light2D>();
        if (PlayerPrefs.GetInt("GameType") == 3 && PlayerPrefs.GetInt("Training") == 0)
        {
            hardcoreLogo.SetActive(true);

            if (PlayerPrefs.GetInt("IsBroken") == 1)
            {
                enemyHit = false;
                StartCoroutine(PlayerHit());
                life = 1;
            }
        }
        else hardcoreLogo.SetActive(false);
        GameObject.Find("MusicManager").GetComponent<AudioSource>().loop = true;
        if (PlayerPrefs.GetInt("Training") == 1) GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(2);
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(2);
        }
        else if (SceneManager.GetActiveScene().buildIndex >= 3 && PlayerPrefs.GetInt("CurrentLevelID") <= 10 || PlayerPrefs.GetInt("CurrentLevelID") == 19)
        {
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(3);
        }
        else if (SceneManager.GetActiveScene().buildIndex >= 11 && PlayerPrefs.GetInt("CurrentLevelID") <= 18)
        {
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(4);
        }
    }

    // Input Initialization
    public void OnEnable()
    {
        playerInput.Enable();
    }
    public void OnDisable()
    {
        playerInput.Disable();
    }
    private void Start()
    {
        playerInput.Key.Move.canceled += StopRunning;
        playerInput.Key.Move.performed += StartRunning;
        playerInput.Key.FlipJump.canceled += Jump;
        playerInput.Key.FlipJump.performed += Flip;
    }

    void Update()
    {
        IsGround();
        if (isMoving && isGrounded && !dustBegan)
        {
            StartCoroutine(DustWalking());
        }
    }

    void StartRunning(InputAction.CallbackContext context)
    {
        isMoving = true;
        movingmultiplier = 1f;
    }
    void StopRunning(InputAction.CallbackContext context)
    {
        isMoving = false;
        movingmultiplier = 0f;
    }

    // Movement and Animations
    void FixedUpdate()
    {
        rb.velocity = new Vector2(movingmultiplier * 30f * rotationValue * speed * Time.deltaTime, rb.velocity.y);
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = jumpGravity;
        }
        if (isGrounded)
            isMiddleAir = false;        
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsMiddleAir", isMiddleAir);
        animator.SetBool("IsRotating", isRotating);
        if (lightColor == 0) playerLight.color = yellow;
        if (lightColor == 1) playerLight.color = green;
        if (lightColor == 2) playerLight.color = blue;
        if (lightColor == 3) playerLight.color = red;
    }
    IEnumerator DustWalking()
    {
        dustBegan = true;
        if (isMoving && isGrounded)
        {
            CreateParticle(dust);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(DustWalking());
        }
        else dustBegan = false;
    }
    IEnumerator GlassDamaged()
    {
        if (animator.GetBool("IsBroken") == true)
        {
            CreateParticle(glassHit);
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(GlassDamaged());
        }
    }

    public void ChangeLightColor(int colorID)
    {
        lightColor = colorID;
    }

    public void EnemyBoost()
    {
        CreateParticle(dustJump);
        rb.velocity = Vector2.zero;
        rb.velocity = Vector2.up * (jumpForce / 2);
    }

    // Ground Detection
    void IsGround()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.46f, groundLayer) && airTimer <= 0)
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

    // Performed Actions
    void Flip(InputAction.CallbackContext context)
    {
        isRotating = true;
    }

    public void Rotated()
    {
        isRotating = false;
        transform.Rotate(0f, 180f, 0f);
        rotationValue *= -1f;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && airTimer <= 0 && playerInput.Key.Move.ReadValue<float>() == 0)
        {
            CreateParticle(dustJump);
            rb.velocity = Vector2.zero;
            rb.velocity = Vector2.up * jumpForce;
            isGrounded = false;
            airTimer = 0.06f;
            isMiddleAir = true;
            GameObject.Find("SfxManager").GetComponent<SfxManager>().PlayOnce(1);
        }
    }

    // Collision Detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 && canGetHit)
        {
            life--;
            if (life <= 0)
            {
                StartCoroutine(PlayerDeath());
            }
            else
            {
                GameObject.Find("SfxManager").GetComponent<SfxManager>().PlayOnce(3);
                if (PlayerPrefs.GetInt("GameType") == 3) PlayerPrefs.SetInt("IsBroken", 1);
                enemyHit = true;
                StartCoroutine(PlayerHit());
            }
        }
    }

    IEnumerator PlayerDeath()
    {
        if (!corpseSpawned)
        {
            animator.SetBool("IsBroken", false);
            GameObject.Find("MusicManager").GetComponent<AudioSource>().loop = false;
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(5);
            Instantiate(playerCorpse, new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(new Vector3(0, 0, 25)));
            corpseSpawned = true;
            CreateParticle(glassDeath);
            GameObject.Find("SfxManager").GetComponent<SfxManager>().PlayOnce(4);
            if (PlayerPrefs.GetInt("Training") == 0) PlayerPrefs.SetInt("CurrentLevelID", SceneManager.GetActiveScene().buildIndex);
            GetComponent<SpriteRenderer>().enabled = false;
            playerInput.Key.Disable();
            yield return new WaitForSeconds(1f);
            Time.timeScale = 0f;
            GameObject.Find("Death Panel").GetComponent<Animator>().Play("DeathAnim");
        }
    }

    IEnumerator PlayerHit()
    {
        animator.SetBool("IsBroken", true);
        StartCoroutine(GlassDamaged());
        if (enemyHit)
        {
            GetComponent<SpriteRenderer>().material.color = new Color32(255, 255, 255, 130);
            canGetHit = false;
            yield return new WaitForSeconds(2f);
            GetComponent<SpriteRenderer>().material.color = new Color32(255, 255, 255, 255);
            canGetHit = true;
            enemyHit = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8) this.gameObject.transform.parent = collision.transform;
        else if (collision.gameObject.layer == 9)
        {
            if (PlayerPrefs.GetInt("GameType") == 3) PlayerPrefs.SetInt("IsBroken", 0);
            animator.SetBool("IsBroken", false);
            life = 2;
            CreateParticle(repair);
            Destroy(collision.gameObject);
            GameObject.Find("SfxManager").GetComponent<SfxManager>().PlayOnce(5);
        }
        else if (collision.gameObject.layer == 11) StartCoroutine(PlayerDeath());
        else if (collision.gameObject.layer == 12)
        {
            GameObject.Find("MusicManager").GetComponent<AudioSource>().loop = false;
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayMusic(6);
            GameObject.Find("Death Panel").GetComponent<Animator>().Play("FinishAnim");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            this.gameObject.transform.parent = null;
        }
    }

    private void CreateParticle(ParticleSystem particle)
    {
        particle.Play();
    }
}
