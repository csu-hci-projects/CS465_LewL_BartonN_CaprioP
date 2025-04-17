using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;

public class RhythmPad : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hitMaterial;
    public Material missMaterial;
    public Renderer buttonRenderer;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI finalScoreText;

    private float colorTimer = 0f;
    private float colorDuration = 0.2f;

    private int beatIndex = 0;
    private int hitCount = 0;
    private int missCount = 0;
    private bool trialFinished = false;

    private float resultClearTimer = 0f;
    private float resultClearDelay = 3f;

    private float finalClearTimer = 0f;
    private float finalClearDelay = 6f;

    // Logging
    private List<string> logLines = new List<string>();
    private int currentTrialNumber = 0;
    private string currentModality = "Unknown";
    private float currentTempo = 1f;

    public void SetTrialInfo(int trialNum, string modality, float tempo)
    {
        currentTrialNumber = trialNum;
        currentModality = modality;
        currentTempo = tempo;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PokeTip") || trialFinished)
            return;

        float hitTime = Time.time - BeatManager.startTime;
        float[] beatTimes = BeatManager.beatTimes;

        if (beatIndex >= beatTimes.Length)
        {
            resultText.text = "No beats left!";
            return;
        }

        float expectedTime = beatTimes[beatIndex];
        float offset = hitTime - expectedTime;

        bool isGoodHit = Mathf.Abs(offset) <= 0.2f;
        string result = isGoodHit
            ? $"HIT! Offset: {offset:F2}s"
            : $"MISS! Offset: {offset:F2}s";

        if (isGoodHit)
        {
            hitCount++;
            buttonRenderer.material = hitMaterial;
        }
        else
        {
            missCount++;
            buttonRenderer.material = missMaterial;
        }

        resultText.text = result;
        colorTimer = colorDuration;

        // Log the hit
        logLines.Add($"{currentTrialNumber},{currentModality},{currentTempo},{beatIndex + 1},{expectedTime:F3},{hitTime:F3},{offset:F3},{isGoodHit}");

        beatIndex++;

        if (beatIndex >= beatTimes.Length)
        {
            trialFinished = true;
            ShowFinalScore();
            SaveCSV();
            resultClearTimer = resultClearDelay;
            finalClearTimer = finalClearDelay;
        }
    }

    void ShowFinalScore()
    {
        int total = hitCount + missCount;
        float accuracy = total > 0 ? (hitCount * 100f) / total : 0f;

        finalScoreText.text =
            $"Trial Complete!\n" +
            $"Hits: {hitCount} / {total}\n" +
            $"Accuracy: {accuracy:F1}%";
    }

    public void ResetScoring()
    {
        beatIndex = 0;
        hitCount = 0;
        missCount = 0;
        trialFinished = false;
        resultClearTimer = 0f;
        finalClearTimer = 0f;

        if (resultText != null) resultText.text = "";
        if (finalScoreText != null) finalScoreText.text = "";

        logLines.Clear();
        logLines.Add("Trial,Modality,Tempo,BeatIndex,ExpectedTime,HitTime,Offset,GoodHit");
    }

    void SaveCSV()
    {
        string folderPath = Application.persistentDataPath;
        string filePath = Path.Combine(folderPath, $"trial_data.csv");

        bool fileExists = File.Exists(filePath);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Trial,Modality,Tempo,BeatIndex,ExpectedTime,HitTime,Offset,GoodHit");
                }

                for (int i = 1; i < logLines.Count; i++)
                {
                    writer.WriteLine(logLines[i]);
                }
            }

            Debug.Log($"✅ Trial data saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to write CSV: {ex.Message}");
        }
    }

    void Update()
    {
        if (colorTimer > 0f)
        {
            colorTimer -= Time.deltaTime;
            if (colorTimer <= 0f && buttonRenderer != null && defaultMaterial != null)
            {
                buttonRenderer.material = defaultMaterial;
            }
        }

        if (trialFinished && resultClearTimer > 0f)
        {
            resultClearTimer -= Time.deltaTime;
            if (resultClearTimer <= 0f && resultText != null)
            {
                resultText.text = "";
            }
        }

        if (trialFinished && finalClearTimer > 0f)
        {
            finalClearTimer -= Time.deltaTime;
            if (finalClearTimer <= 0f && finalScoreText != null)
            {
                finalScoreText.text = "";
            }
        }
    }
}
