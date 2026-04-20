using UnityEngine;
using System.Collections;

public class FlippingPlatform : MonoBehaviour
{
    [Header("Flip Settings")]
    [SerializeField] private float _rotationAmount = 90f;
    [SerializeField] private float _speed = 90f;
    [SerializeField] private float _pauseTime = 1.5f;

    [Header("Shake Settings")]
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeAmount = 2f;

    public PlayerController Player;

    private Quaternion _originalRotation;

    private void Awake()
    {
        Player.OnCharacterChanged += PlayerStateChanged;
        StartCoroutine(RotateRoutine());
    }

    private void OnDestroy()
    {
        Player.OnCharacterChanged -= PlayerStateChanged;
    }

    private void PlayerStateChanged(Character character)
    {
        _rotationAmount = (character == Character.Sun) ? 90f : -90f;
    }

    IEnumerator RotateRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(Shake());

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

    IEnumerator Shake()
    {
        _originalRotation = transform.rotation;
        float timer = 0;

        while (timer < _shakeDuration)
        {
            timer += Time.deltaTime;

            float shake = Mathf.Sin(timer * 50f) * _shakeAmount;

            transform.rotation = _originalRotation * Quaternion.Euler(shake, 0, 0);

            yield return null;
        }

        transform.rotation = _originalRotation;
    }
}