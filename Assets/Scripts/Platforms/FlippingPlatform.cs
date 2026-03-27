using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class FlippingPlatform : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private float _rotationAmount = 90f;
    [SerializeField] private float _speed = 90f;    
    [SerializeField] private float _pauseTime = 1.5f;

    public PlayerController Player;

    void Start()
    {

        if (Player.IsMoonActive)
        {
            {
                StartCoroutine(RotateRoutine());
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
        }

        else
        {

        }

    }
}

=======
    [SerializeField] private float _speed;
    [SerializeField] private float _pauseTime;

    public PlayerController Player;
   
    IEnumerator RotateRoutine(bool IsMoonActive)
    {
        while (true)
        {
            Quaternion startRot = transform.rotation;
            Quaternion targetRot;

            if (IsMoonActive)
            {
                targetRot = startRot * Quaternion.Euler(90, 0, 0);
            }

            else
            {
                targetRot = startRot * Quaternion.Euler(-90, 0, 0);
            }

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

    void Start()
    {
        StartCoroutine(RotateRoutine(Player.IsMoonActive));
    }
}
                


   
>>>>>>> Stashed changes
