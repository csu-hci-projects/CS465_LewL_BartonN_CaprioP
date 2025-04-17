using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float duration;
    private float startTime;
    private bool ready = false;

    public void Init(Vector3 target, float time, AudioSource audio)
    {
        startPoint = transform.position;
        endPoint = target;
        duration = time;
        startTime = Time.time;
        ready = true;
    }

    void Update()
    {
        if (!ready) return;

        float elapsed = Time.time - startTime;
        float t = elapsed / duration;
        transform.position = Vector3.Lerp(startPoint, endPoint, t);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
