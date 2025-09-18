using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public string chestID; // Will be set when instantiated by PlaceObject

    public void HandleTouch()
    {
        Debug.Log("Tapped chest: " + chestID);
        PuzzleUIManager.Instance.OpenPuzzle(chestID);
    }
}
