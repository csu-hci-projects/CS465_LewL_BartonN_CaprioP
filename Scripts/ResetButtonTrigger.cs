using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButtonTrigger : MonoBehaviour
{
    public TrialManager trialManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PokeTip"))
        {
            Debug.Log("Reset button pressed.");
            trialManager.ResetTrial();
        }
    }
}

