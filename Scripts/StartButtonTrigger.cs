using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    public TrialManager trialManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PokeTip") && !trialManager.GetIsTrialRunning())
        {
            Debug.Log("Start button pressed.");
            trialManager.StartTrial();
        }
    }
}

