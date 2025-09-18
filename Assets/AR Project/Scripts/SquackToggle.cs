using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquawkToggle : MonoBehaviour
{
    public Sprite closedMouthSprite;
    public Sprite openMouthSprite;

    public GameObject dialogueBubble;       // The speech bubble GameObject
    public TMP_Text hintText;               // The TMP text inside the bubble
    [TextArea] public string hintMessage = "Tap the key that fits the chest!";

    private Image squawkImage;
    private bool isSpeaking = false;

    private AudioSource audioSource;
    public AudioClip squackSound;

    private PulseText pulseScript;

    private void Awake()
    {
        squawkImage = GetComponent<Image>(); // Gets the image on this button
        audioSource = GetComponent<AudioSource>();
        pulseScript = GetComponent<PulseText>();
    }

    public void OnSquawkTapped()
    {
        isSpeaking = !isSpeaking;

        if (squawkImage != null)
            squawkImage.sprite = isSpeaking ? openMouthSprite : closedMouthSprite;

        if (dialogueBubble != null)
            dialogueBubble.SetActive(isSpeaking);

        if (isSpeaking && hintText != null)
            hintText.text = hintMessage;

        if (isSpeaking && audioSource != null)
            audioSource.PlayOneShot(squackSound);

        if (pulseScript != null)
            pulseScript.isPulsing = !isSpeaking;
    }
}
