using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float rotationSpeed = 30f;

    private bool clockwise = true;

    private void Awake()
    {
        if (player != null)
        {
            player.OnCharacterChanged += PlayerStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnCharacterChanged -= PlayerStateChanged;
        }
    }

    private void PlayerStateChanged(Character character)
    {
        clockwise = (character == Character.Sun);
    }

    private void Update()
    {

        float direction = clockwise ? 1f : -1f;
        transform.Rotate(Vector3.up * direction * rotationSpeed * Time.deltaTime, Space.Self);


    }


}