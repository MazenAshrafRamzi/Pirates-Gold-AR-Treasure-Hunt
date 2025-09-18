using UnityEngine;
using UnityEngine.UI;

public class ConfettiAnimator : MonoBehaviour
{
    public Sprite[] confettiSprites; // Size = 2
    public float switchInterval = 0.5f;

    private Image imageComponent;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (confettiSprites.Length > 0)
            imageComponent.sprite = confettiSprites[0];
    }

    void Update()
    {
        if (confettiSprites.Length < 2) return;

        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % confettiSprites.Length;
            imageComponent.sprite = confettiSprites[currentIndex];
        }
    }
}
