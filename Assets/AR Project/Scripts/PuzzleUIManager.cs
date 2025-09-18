using UnityEngine;

public class PuzzleUIManager : MonoBehaviour
{
    public static PuzzleUIManager Instance;

    public GameObject puzzleChest1;
    public GameObject puzzleChest2;
    public GameObject puzzleChest3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenPuzzle(string chestID)
    {
        CloseAll();

        switch (chestID)
        {
            case "chest1":
                puzzleChest1.SetActive(true);
                break;
            case "chest2":
                puzzleChest2.SetActive(true);
                break;
            case "chest3":
                puzzleChest3.SetActive(true);
                break;
        }
    }

    public void CloseAll()
    {
        puzzleChest1.SetActive(false);
        puzzleChest2.SetActive(false);
        puzzleChest3.SetActive(false);
    }

    public void ClosePuzzlePanel(GameObject panel)
    {
        panel.SetActive(false);

        string chestName = panel.name switch
        {
            "PuzzleUI_Chest3" => "chest_medium(Clone)",
            "PuzzleUI_Chest2" => "chest_large(Clone)",
            "PuzzleUI_Chest1" => "chest_epic(Clone)",
            _ => null
        };

        if (chestName == null)
        {
            Debug.LogWarning($"❌ Unknown panel name: {panel.name}");
            return;
        }

        GameObject chest = GameObject.Find(chestName);
        if (chest == null)
        {
            Debug.LogWarning($"❌ Chest GameObject '{chestName}' not found in scene.");
            return;
        }

        // Try both lid naming conventions
        Transform lid = chest.transform.Find($"{chestName.Replace("(Clone)", "")}_lid");
        if (lid == null)
        {
            lid = chest.transform.Find("chest/lid");  // specifically for chest_medium
        }

        if (lid == null)
        {
            Debug.LogWarning($"❌ Lid not found under chest '{chestName}'.");
            return;
        }

        // Destroy parrot child if it exists
        bool parrotDestroyed = false;
        foreach (Transform child in lid)
        {
            if (child.name.ToLower().Contains("parrot"))
            {
                Destroy(child.gameObject);
                Debug.Log($"🗑️ Destroyed parrot on chest: {chestName}");
                parrotDestroyed = true;
                break;
            }
        }

        // Trigger chest animation after parrot is removed
        Animator animator = chest.GetComponent<Animator>();
        if (animator != null)
        {
            string triggerName = chestName switch
            {
                "chest_medium(Clone)" => "WinMedium",
                "chest_large(Clone)" => "WinLarge",
                "chest_epic(Clone)" => "WinEpic",
                _ => null
            };

            if (!string.IsNullOrEmpty(triggerName))
            {
                animator.SetTrigger(triggerName);
                Debug.Log($"🎬 Triggered animation '{triggerName}' on {chestName}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Unknown trigger for chest: {chestName}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ No Animator component found on {chestName}");
        }

        if (!parrotDestroyed)
        {
            Debug.LogWarning($"⚠️ No parrot found to destroy on: {chestName}");
        }
    }





    public bool IsAnyPuzzleOpen()
    {
        return puzzleChest1.activeSelf || puzzleChest2.activeSelf || puzzleChest3.activeSelf;
    }

}
