using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class FlippingPlatform : MonoBehaviour
{

    [SerializeField] private float _rotationAmount = 90f;
    [SerializeField] private float _speed = 90f;
    [SerializeField] private float _pauseTime = 1.5f;




    public PlayerController Player;

    private void Awake()
    {
        Player.OnCharacterChanged += PlayerStateChanged;
        StartCoroutine(nameof(RotateRoutine));
    }

    private void OnDestroy()
    {
        Player.OnCharacterChanged -= PlayerStateChanged;

    }

    private void PlayerStateChanged(Character character)
    {

        if (character == Character.Sun)
            _rotationAmount = 90f;
        else
            _rotationAmount = -90f;
    }


    IEnumerator RotateRoutine()
    {
        while (true)
        {
            Quaternion startRot = transform.rotation;
            Quaternion targetRot = startRot * Quaternion.Euler(_rotationAmount, 0, 0);

            while (transform.rotation != targetRot)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    _speed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(_pauseTime);
        }
    }



    public float shakeDuration = 2;

    private bool animating;
    private Vector3 startPosition;
    private Vector3 fallOffset = new Vector3(0, -100, 0);
    private int animationStatus;
    private float animationTimer;
    private float animationDuration;

    void FixedUpdate()
    {
        if (!animating) return;

        animationTimer += Time.fixedDeltaTime;

        if (animationStatus == 1)
        {
            transform.position = startPosition + new Vector3(Mathf.Sin(animationTimer * UnityEngine.Random.Range(1, 3)), 0, -Mathf.Sin(animationTimer * UnityEngine.Random.Range(1, 3)));

        }
        else if (animationStatus == 2)
        {
            //how much percent are we in the animation
            float percentage = animationTimer / animationDuration;
            transform.position = startPosition + fallOffset * percentage;

        }



    }


    //void Start()
    //{

    //    if (Player.IsMoonActive)
    //    {
    //        {
    //            StartCoroutine(RotateRoutine());
    //        }

    //        IEnumerator RotateRoutine()
    //        {
    //            while (true)
    //            {
    //                Quaternion startRot = transform.rotation;
    //                Quaternion targetRot = startRot * Quaternion.Euler(_rotationAmount, 0, 0);

    //                while (transform.rotation != targetRot)
    //                {
    //                    transform.rotation = Quaternion.RotateTowards(
    //                        transform.rotation,
    //                        targetRot,
    //                        _speed * Time.deltaTime
    //                    );

    //                    yield return null;
    //                }

    //                yield return new WaitForSeconds(_pauseTime);
    //            }
    //        }
    //    }

    //    else
    //    {

    //    }

    //}
}




