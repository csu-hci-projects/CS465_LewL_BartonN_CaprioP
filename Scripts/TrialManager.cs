using UnityEngine;
using TMPro;
using System.Collections;

public class TrialManager : MonoBehaviour
{
    public BeatSpawner spawner;
    public RhythmPad rhythmPad;
    public TextMeshProUGUI countdownText;

    private bool trialStarted = false;

    public void StartTrial()
    {
        if (trialStarted) return;
        StartCoroutine(RunAllTrials());
    }

    IEnumerator RunAllTrials()
    {
        trialStarted = true;

        string[] modalities = { "Audio Only", "Visual Only", "Audio + Visual" };
        bool[] audioModes =  { true, false, true };
        bool[] visualModes = { false, true, true };

        float[] tempos = { 1.15f, 0.75f }; // Two tempos
        int totalBeats = 12;
        float firstBeatAt = 4f;

        for (int t = 0; t < tempos.Length; t++)
        {
            for (int m = 0; m < modalities.Length; m++)
            {
                float tempo = tempos[t];
                string label = modalities[m];
                bool useAudio = audioModes[m];
                bool useVisual = visualModes[m];

                countdownText.text = $"Trial {t * 3 + m + 1}: {label} @ {tempo:0.00}s/beat";
                yield return new WaitForSeconds(2f);

                BeatManager.GenerateBeatTimes(firstBeatAt, tempo, totalBeats);
                rhythmPad.ResetScoring();
                spawner.enableAudio = useAudio;
                spawner.enableVisual = useVisual;

                bool clickIn = useAudio;
                yield return StartCoroutine(CountdownRoutine(clickIn, tempo));

                spawner.ResetBeats();

                float trialDuration = BeatManager.beatTimes[BeatManager.beatTimes.Length - 1] + 2f;
                yield return new WaitForSeconds(trialDuration);

                //countdownText.text = $"Trial {t * 3 + m + 1} complete!";
                
                yield return new WaitForSeconds(3f);
                countdownText.text = "";
            }
        }

        countdownText.text = "Experiment Complete!";
        yield return new WaitForSeconds(5f);
        countdownText.text = "";
        trialStarted = false;
    }

    IEnumerator CountdownRoutine(bool withClickIn, float tempoSpacing)
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.text = "Go!";

        if (withClickIn && spawner.metronomeSource != null && spawner.metronomeSource.clip != null)
        {
            for (int i = 0; i < 4; i++)
            {
                spawner.metronomeSource.PlayOneShot(spawner.metronomeSource.clip);
                yield return new WaitForSeconds(tempoSpacing);
            }
        }

        countdownText.text = "";
    }

    public void ResetTrial()
    {
        trialStarted = false;
        countdownText.text = "Trial restarting...";
        Debug.Log("Trial reset.");
    }
}
