using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private Transform player;

    public PlayerController Player;


    public float rotationSpeed = 30f;
    private bool clockwise = true;
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
            clockwise = true;
        else
            clockwise = false;
    }
    void Update()
    {
        if (clockwise)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }

        else
        { 
            transform.Rotate(Vector3.up * rotationSpeed * -Time.deltaTime, Space.Self);
        }


    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void SetPlayerParent()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();

            if (controller)
            {
                player = controller.transform;
                SetPlayerParent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null)
        {
            player.parent = null;
        }
    }

}