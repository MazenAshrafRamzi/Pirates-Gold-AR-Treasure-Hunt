using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RiddlePuzzle : MonoBehaviour
{
    public Image chestImage;
    public Sprite openChestSprite;

    public Button[] answerButtons;
    private int correctAnswerIndex = 2;

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
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        if (isSolved) return;

        if (index == correctAnswerIndex)
        {
            isSolved = true;
            OpenChest();
            audioSource.PlayOneShot(correctKeyAudioClip);
            GameManager.Instance?.AddScore(10);
            Invoke(nameof(ClosePuzzlePanel), winDelay);
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
            -223.0001f
        );
    }

    void ClosePuzzlePanel()
    {
        if (PuzzleUIManager.Instance != null)
            PuzzleUIManager.Instance.ClosePuzzlePanel(gameObject);
    }
}