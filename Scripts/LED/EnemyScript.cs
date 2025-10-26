using UnityEngine;
using UnityEngine.UIElements;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform playerCheck;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool turnedLeft;

    private bool hasStarted = false;
    private float rotationValue = -1f;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (turnedLeft == false)
        {
            Flip();
        }
    }
    private void OnBecameVisible()
    {
        hasStarted = true;
    }
    private void Update()
    {
        if (Physics2D.OverlapCircle(wallCheck.position, 0.03f, wallLayer))
        {
            Flip();
        }
        if (Physics2D.OverlapCircle(playerCheck.position, 0.5f, playerLayer))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            GameObject.Find("Player").GetComponent<PlayerController>().EnemyBoost();
            GameObject.Find("SfxManager").GetComponent<SfxManager>().PlayOnce(2);
            animator.SetBool("IsDead", true);
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }
    void FixedUpdate()
    {
        if (hasStarted && animator.GetBool("IsDead") == false)
        {
            rb.velocity = new Vector2(30f * rotationValue * speed * Time.deltaTime, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            Death();
        }
    }
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        rotationValue *= -1f;
    }
}
