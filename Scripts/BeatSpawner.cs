using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    public GameObject beatCuePrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public float travelTime = 1.5f;
    public AudioSource metronomeSource;

    private float startTime;
    private int index = 0;
    public bool isActive = false;

    public bool enableAudio = true;
    public bool enableVisual = true;

    void Start()
    {
        Debug.Log("BeatSpawner.Start() ran");
        isActive = false;
        startTime = BeatManager.startTime;
    }

    void Update()
    {
        if (!isActive || BeatManager.beatTimes == null || index >= BeatManager.beatTimes.Length)
            return;

        float currentTime = Time.time - startTime;
        float adjustedSpawnTime = BeatManager.beatTimes[index] - travelTime;

        if (currentTime >= adjustedSpawnTime)
        {
            if (enableVisual)
            {
                GameObject cue = Instantiate(beatCuePrefab, spawnPoint.position, Quaternion.identity);
                MoveToTarget cueScript = cue.GetComponent<MoveToTarget>();
                if (cueScript != null)
                {
                    cueScript.Init(targetPoint.position, travelTime - 0.2f, enableAudio ? metronomeSource : null);
                }
            }
            if (enableAudio && metronomeSource != null)
            {
                metronomeSource.Play();
                Debug.Log("Metronome Played Beat");
            }

            Debug.Log($"Spawned beat #{index + 1} at {currentTime:F2}s (Arrives at {BeatManager.beatTimes[index]}s)");
            index++;
        }
    }

    public void ResetBeats()
    {
        index = 0;
        startTime = BeatManager.startTime = Time.time;
        isActive = true;
    }
}
