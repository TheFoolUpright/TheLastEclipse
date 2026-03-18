using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlippingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation; 
   
    void Update()
    {
        transform.Rotate(_rotation * );
    }
}
