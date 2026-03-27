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

    private void Update()
    {
        
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




