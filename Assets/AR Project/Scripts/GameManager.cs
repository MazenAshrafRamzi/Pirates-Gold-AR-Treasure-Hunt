using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int score = 0;
    public TMP_Text scoreText;

    public GameObject winPanel; // assign in inspector
    public AudioSource winSound; // optional
    public CaptainController captainController; // assign in inspector
    public GameObject captainObject; // assign in inspector
    [SerializeField] private GameObject parrotPrefab;
    public AudioSource backgroundMusic; // assign via Inspector
   


    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optional, if you load scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        if (score >= 30)
        {
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
                Debug.Log("🎵 Background music stopped.");
            }

            TriggerWin();
            CaptainController captain = Object.FindFirstObjectByType<CaptainController>();

            if (captain != null)
            {
                captain.TriggerFinalWin();
            }
            PlaceObject.WINWIN = true;
            Debug.Log("🏁 WINWIN set to true from GameManager.");
            PlaceObject.WINWIN = true;

        }
    }

    void TriggerWin()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winSound != null)
            winSound.Play();

        //Hide score text
        if (scoreText != null)
            scoreText.text = "";

    }


    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
            Destroy(GameManager.Instance.gameObject);

        Destroy(GameObject.Find("AR Session"));

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit pressed - app will quit on device");
    }
}
