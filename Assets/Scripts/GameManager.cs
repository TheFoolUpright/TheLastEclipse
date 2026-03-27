using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelData LevelData;
    public CheckpointData CheckpointData;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        CheckpointData.Initialize();
    }


}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public int Level;
    public float priceToSomething = 1f;
}

[System.Serializable]
public class CheckpointData
{
    public List<Checkpoint> CheckPoints;
    public Checkpoint activeCheckpoint;

    public void Initialize()
    {
        CheckPoints = GameObject.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None).ToList();
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        this.activeCheckpoint = checkpoint;
    }
}