using UnityEngine;

public class SoulUI : MonoBehaviour
{
    [Header("Flee Soul")]
    public GameObject fleeNotCollected;
    public GameObject fleeCollected;

    [Header("Attack Soul")]
    public GameObject attackNotCollected;
    public GameObject attackCollected;

    private void Start()
    {
        // Begin state
        fleeNotCollected.SetActive(true);
        fleeCollected.SetActive(false);

        attackNotCollected.SetActive(true);
        attackCollected.SetActive(false);
    }

    public void SetFleeCollected()
    {
        fleeNotCollected.SetActive(false);
        fleeCollected.SetActive(true);
    }

    public void SetAttackCollected()
    {
        attackNotCollected.SetActive(false);
        attackCollected.SetActive(true);
    }
}