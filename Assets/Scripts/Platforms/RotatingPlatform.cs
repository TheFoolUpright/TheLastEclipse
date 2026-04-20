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
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }

        else
        { 
            transform.Rotate(Vector3.forward * rotationSpeed * -Time.deltaTime, Space.Self);
        }


    }

    void OnCollisionEnter(Collision collision)
    {
       
    }

    void OnCollisionExit(Collision collision)
    {

    }

    private void SetPlayerParent()
    {
        player.SetParent(this.transform, true);
    }

}