using UnityEngine;

public class ChestReveal : MonoBehaviour
{
    public float revealDistance = 1.2f;
    public float growSpeed = 3f;
    public GameObject parrotPrefab;

    private SkinnedMeshRenderer[] renderers;
    private bool isVisible = false;
    private bool parrotSpawned = false;
    private Vector3 targetScale;
    [HideInInspector]
    public bool destroyParrot = false;

    private GameObject spawnedParrot; // Track reference to spawned parrot

    private GameObject sparkleObject;

    void Start()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("❌ No SkinnedMeshRenderers found on chest.");
            return;
        }

        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        SetRenderers(true);

        Debug.Log("🕵️‍♂️ Chest is hidden via scale. Waiting for player proximity...");
    }

    void Update()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        Debug.Log($"📏 Distance to chest: {distance:F2}");

        if (!isVisible && distance <= revealDistance)
        {
            Debug.Log("👁️ Player entered range — scaling chest in.");
            isVisible = true;
            if (sparkleObject != null)
            {
                sparkleObject.SetActive(false);
                Debug.Log("✨ Sparkle deactivated after chest reveal.");
            }
            TrySpawnParrot();
        }

        if (isVisible && transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);
        }

        if (destroyParrot && spawnedParrot != null)
        {
            Destroy(spawnedParrot);
            spawnedParrot = null;
            destroyParrot = false;
            Debug.Log("🧨 Parrot destroyed manually by flag.");
        }
    }
    public void AssignSparkle(GameObject sparkle)
    {
        sparkleObject = sparkle;
    }

    void SetRenderers(bool enabled)
    {
        foreach (var rend in renderers)
        {
            rend.enabled = enabled;
        }
    }

    void TrySpawnParrot()
    {
        if (parrotSpawned || parrotPrefab == null) return;

        string baseName = gameObject.name.Replace("(Clone)", "").Trim();
        string lidPath = $"{baseName}_lid";

        Transform lid = transform.Find(lidPath);

        // Fallback for chest_medium: "chest/lid"
        if (lid == null)
        {
            lid = transform.Find("chest/lid");
        }

        if (lid == null)
        {
            Debug.LogWarning($"❌ Lid not found under chest '{gameObject.name}'. Tried '{lidPath}' and 'chest/lid'.");
            return;
        }

        // Step 1: instantiate parrot as child of lid (so rotation aligns automatically)
        spawnedParrot = Instantiate(parrotPrefab, lid);


        spawnedParrot.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        spawnedParrot.transform.localRotation = Quaternion.identity;
        spawnedParrot.transform.localScale = parrotPrefab.transform.localScale;

        Animator animator = spawnedParrot.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("ChestRevealed");
        }

        AudioSource audio = spawnedParrot.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }


        parrotSpawned = true;
        Debug.Log("🦜 Parrot successfully spawned as child of lid with offset.");
    }




}
