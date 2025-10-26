using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallCollisionScript : MonoBehaviour
{
    public UnityEvent win;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Win")
        {
            Time.timeScale = 0f;
            win.Invoke();
        }
    }
}
