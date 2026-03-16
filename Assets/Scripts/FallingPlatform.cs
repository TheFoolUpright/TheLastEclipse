using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    private bool animating;
    private Vector3 startPosition;
    private Vector3 fallOffset = new Vector3(0, -100, 0);
    private int animationStatus;
    private float animationTimer;
    private float animationDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!animating) return;

        animationTimer += Time.fixedDeltaTime;
        
        if (animationStatus == 1)
        {
            transform.position = startPosition + new Vector3(Mathf.Sin(animationTimer * UnityEngine.Random.Range(5, 10)), 0, -Mathf.Sin(animationTimer * UnityEngine.Random.Range(5, 10)));
            if(animationTimer >= animationDuration)
            {
                Fall();
            }
        } 
        else if (animationStatus == 2) 
        {
            //how much percent are we in the animation
            float percentage = animationTimer / animationDuration;
            transform.position = startPosition + fallOffset * percentage;
            if (animationTimer >= animationDuration)
            {
                StayHidden();
            }
        } 
        else if(animationStatus == 3)
        {
            if (animationTimer >= animationDuration)
            {
                Comeback();
            }
        } else if (animationStatus == 4)
        {
            float percentage = animationTimer / animationDuration;
            transform.position = startPosition + fallOffset * (1 - percentage);
            if (animationTimer >= animationDuration)
            {
                EndAnimation();
            }
        }
    }

    private void EndAnimation()
    {
        animationTimer = 0;
        animationStatus = 0;
        transform.position = startPosition;
        animating = false;
    }

    private void Comeback()
    {
        animationTimer = 0;
        animationStatus = 4;
        animationDuration = 2;
        transform.position = startPosition + fallOffset;
    }

    private void StayHidden()
    {
        animationTimer = 0;
        animationStatus = 3;
        animationDuration = 5;
        transform.position = startPosition + fallOffset;
    }

    private void Fall()
    {
        animationTimer = 0;
        animationStatus = 2;
        animationDuration = 2;
        transform.position = startPosition;
    }

    private void Shake()
    {
        if (animating) return;
        animating = true;
        animationStatus = 1;
        animationDuration = 5;
        animationTimer = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I was here, I collided");
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                Shake();
            }
        }
    }

   
}
