using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private bool isLeft = false, isRight = false;

    [SerializeField]
    private Vector3 slideForce, jumpLeftForce, jumpRightForce;

    public UnityEvent addCoin;
    public UnityEvent gameOver;
    public UnityEvent addPoint;
    public UnityEvent addTwoPoints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "WallLeft")
        {
            isLeft = true;
            SfxManager.Instance.PlayOnce(1);
        }
        else if (collision.gameObject.tag == "WallRight")
        {
            isRight = true;
            SfxManager.Instance.PlayOnce(1);
        }
        else if (collision.gameObject.tag == "Trigger")
        {
            GameOver();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trigger")
        {
            GameOver();
        }
        if (other.gameObject.tag == "Coin")
        {
            addCoin.Invoke();
            addPoint.Invoke();
            SfxManager.Instance.PlayOnce(2);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "BumperLeft")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            rb.AddForce(jumpLeftForce, ForceMode.Impulse);
            SfxManager.Instance.PlayOnce(3);
            addTwoPoints.Invoke();
            other.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if (other.gameObject.tag == "BumperRight")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            rb.AddForce(jumpRightForce, ForceMode.Impulse);
            SfxManager.Instance.PlayOnce(3);
            addTwoPoints.Invoke();
            other.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "WallLeft")
        {
            isLeft = false;
            SfxManager.Instance.PlayOnce(0);
        }
        else if (collision.gameObject.tag == "WallRight")
        {
            isRight = false;
            SfxManager.Instance.PlayOnce(0);
        }
        else if (collision.gameObject.tag == "BumperLeft")
        {
            isLeft = false;
        }
        else if (collision.gameObject.tag == "BumperRight")
        {
            isRight = false;
        }
    }

    public void Jump()
    {
        if (isLeft == true)
        {
            rb.AddForce(jumpLeftForce, ForceMode.Impulse);
            addPoint.Invoke();
        }
        else if (isRight == true)
        {
            rb.AddForce(jumpRightForce, ForceMode.Impulse);
            addPoint.Invoke();
        }
    }

    void GameOver()
    {
        gameOver.Invoke();
    }

    public void Revive()
    {
        this.transform.position = new Vector3(0.375f, 2.02f, 0);
        isLeft = true;
        isRight = false;
    }

    void FixedUpdate()
    {
        if (isLeft == true || isRight == true)
        {
            rb.AddForce(slideForce, ForceMode.Force);
        }

    }
}
