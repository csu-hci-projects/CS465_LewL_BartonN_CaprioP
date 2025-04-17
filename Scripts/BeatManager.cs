using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static float startTime;
    public static float[] beatTimes;

    public static void GenerateBeatTimes(float firstBeatTime, float spacing, int count)
    {
        beatTimes = new float[count];
        for (int i = 0; i < count; i++)
        {
            beatTimes[i] = firstBeatTime + i * spacing;
        }
    }

    void Start()
    {
        startTime = Time.time;
    }
}
