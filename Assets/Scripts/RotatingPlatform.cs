using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{

    public PlayerController Player;


    public float rotationSpeed = 30f;

    void Update()
    {
        if (Player.IsMoonActive)
        {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }

        else
        { 
        transform.Rotate(Vector3.up * rotationSpeed * -Time.deltaTime, Space.Self);
        }


    }
    
}