using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MirrorManager : MonoBehaviour
{
    public UnityEvent OnStarted;
    public UnityEvent OnMirrored;
    public UnityEvent Exit;
    public UnityEvent GameOver;
    public Animator animator;
    bool canSwitch = true;
    public static bool isMirrored;

    private void Awake()
    {
        OnStarted.Invoke();
        isMirrored = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Mirror" && canSwitch)
        {
            OnMirrored.Invoke();
            isMirrored = true;
        }
        else if (collision.gameObject.tag == "Door")
        {
            Exit.Invoke();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameOver.Invoke();
        }
    }
    private void FixedUpdate()
    {
        animator.SetBool("IsMirrored", isMirrored);
    }
}
