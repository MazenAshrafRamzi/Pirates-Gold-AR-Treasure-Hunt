using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyPuzzle : MonoBehaviour
{
    public Image chestImage;
    public Sprite openChestSprite;

    public Button[] keyButtons; // Size = 3
    private int correctKeyIndex = 1; // Index of the correct key (0, 1, or 2)

    private AudioSource audioSource;
    public AudioClip incorrectKeyAudioClip;
    public AudioClip correctKeyAudioClip;

    private bool isSolved = false;

    public float winDelay = 3.0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        for (int i = 0; i < keyButtons.Length; i++)
        {
            int index = i;
            keyButtons[i].onClick.AddListener(() => OnKeyPressed(index));
        }
    }

    void OnKeyPressed(int index)
    {
        if (isSolved) return;

        if (index == correctKeyIndex)
        {
            
            isSolved = true;

            OpenChest();
            audioSource.PlayOneShot(correctKeyAudioClip);
            GameManager.Instance?.AddScore(10);
            // Close the puzzle panel after a delay using PuzzleUIManager
            Invoke(nameof(CallCloseFromManager), winDelay);
        }
        else
        {
            audioSource.PlayOneShot(incorrectKeyAudioClip);
        }
    }

    void OpenChest()
    {
        chestImage.sprite = openChestSprite;
        chestImage.rectTransform.sizeDelta = new Vector2(966f, 1109f);
        chestImage.rectTransform.localScale = new Vector3(0.31f, 0.31f, 0.31f);
        chestImage.rectTransform.anchoredPosition = new Vector2(
            chestImage.rectTransform.anchoredPosition.x,
            -157f
        );
    }

    void CallCloseFromManager()
    {
        if (PuzzleUIManager.Instance != null)
            PuzzleUIManager.Instance.ClosePuzzlePanel(gameObject);
    }
}
