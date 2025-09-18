using UnityEngine;

public class PulseText : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;
    public bool isPulsing = true; // toggle this externally

    private Vector3 originalScale;
    private float pulseTime = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!isPulsing)
        {
            transform.localScale = originalScale; // ensure it returns to normal
            return;
        }

        pulseTime += Time.deltaTime * pulseSpeed;

        float scaleFactor = 1 + Mathf.Sin(pulseTime) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }
}
