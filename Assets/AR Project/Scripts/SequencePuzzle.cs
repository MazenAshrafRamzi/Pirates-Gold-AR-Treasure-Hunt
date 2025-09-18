using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SequencePuzzle : MonoBehaviour
{
    public Button[] symbolButtons; // Skull, Sword, Compass
    public int[] correctSequence = { 0, 1, 2 }; // Sequence indices

    public Image chestImage;
    public Sprite openChestSprite;

    private AudioSource audioSource;
    public AudioClip incorrectKeyAudioClip;
    public AudioClip correctKeyAudioClip;

    public float winDelay = 3.0f;

    private List<int> playerInput = new List<int>();
    public Color selectedColor = new Color(0.9320754f, 0.3537428f, 0.1107938f); // burnt orange
    public Color defaultColor = Color.white;

    private bool isSolved = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        for (int i = 0; i < symbolButtons.Length; i++)
        {
            int index = i;
            symbolButtons[i].onClick.AddListener(() => OnSymbolPressed(index));
        }
    }

    void OnSymbolPressed(int index)
    {
        if (isSolved) return;

        playerInput.Add(index);

        // Change button color to show selection
        Image btnImage = symbolButtons[index].GetComponent<Image>();
        if (btnImage != null)
            btnImage.color = selectedColor;

        if (playerInput[playerInput.Count - 1] != correctSequence[playerInput.Count - 1])
        {
            ResetSequence();
            return;
        }

        if (playerInput.Count == correctSequence.Length)
        {
            isSolved = true;
            OpenChest();
            audioSource.PlayOneShot(correctKeyAudioClip);
            GameManager.Instance?.AddScore(10);
            Invoke(nameof(CallCloseFromManager), winDelay);
        }
    }

    void ResetSequence()
    {
        playerInput.Clear();

        // Reset button colors
        foreach (Button btn in symbolButtons)
        {
            Image img = btn.GetComponent<Image>();
            if (img != null)
                img.color = defaultColor;
        }

        audioSource.PlayOneShot(incorrectKeyAudioClip);
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