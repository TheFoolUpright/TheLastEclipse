using StarterAssets;
using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private StarterAssetsInputs inputs;


    [SerializeField] private GameObject moonVisual;
    [SerializeField] private GameObject sunVisual;
    [SerializeField] [Range(0.1f, 2f)] float delayBetweenChanges;
    private float timer = 0f;

    private void Awake()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        timer = delayBetweenChanges;
    }

    private void Update()
    {
        if (timer  > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }


        if (inputs.changeVisual)
        {
            if (moonVisual.activeInHierarchy)
            {
                moonVisual.SetActive(false);
                sunVisual.SetActive(true);
            }else
            {
                moonVisual.SetActive(true);
                sunVisual.SetActive(false);
            }
            timer = delayBetweenChanges;
        }
    }

}
