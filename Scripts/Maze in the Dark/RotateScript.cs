using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotateScript : MonoBehaviour
{
    private Vector3 screenPos;
    private float angleOffset;
    public Rigidbody2D rb;
    public AudioSource source;
    public UnityEvent playSfx;
    public UnityEvent stopSfx;
    bool isPlaying = false;
    bool isComplete = false;

    private void Update()
    {
        if (!isComplete)
        {
            if (Input.GetMouseButtonDown(0))
            {
                screenPos = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 vec3 = Input.mousePosition - screenPos;
                angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) * Mathf.Rad2Deg;
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 vec3 = Input.mousePosition - screenPos;
                float angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
                rb.MoveRotation(angle + angleOffset);
                if (!isPlaying)
                {
                    playSfx.Invoke();
                    isPlaying = true;
                    Invoke("SoundStops", source.clip.length);
                }
            }
            else
            {
                stopSfx.Invoke();
                isPlaying = false;
            }
        }
    }
    void SoundStops()
    {
        stopSfx.Invoke();
        isPlaying = false;
    }
    public void Complete()
    {
        isComplete = true;
        stopSfx.Invoke();
        isPlaying = true;
    }
}
