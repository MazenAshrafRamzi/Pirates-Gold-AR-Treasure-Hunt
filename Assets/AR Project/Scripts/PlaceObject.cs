using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject captainPrefab;
    [SerializeField] private GameObject parrotPrefab;
    [SerializeField] private GameObject[] chestPrefabs;
    [SerializeField] private GameObject sparklePrefab;
    [SerializeField] private AudioClip captainParrotClip;

    public static bool ParrotFinished = false;
    public static bool WINWIN = false; // Can be set from GameManager
    private GameObject parrot;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool hasPlacedCaptain = false;
    private GameObject captainObject;

    [SerializeField] private GameObject tapToPlaceText;
    [SerializeField] private GameObject scoreText;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        if (!hasPlacedCaptain)
        {
            if (aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                foreach (ARRaycastHit hit in hits)
                {
                    Pose pose = hit.pose;
                    ARPlane plane = aRPlaneManager.GetPlane(hit.trackableId);
                    if (plane == null || plane.alignment != PlaneAlignment.HorizontalUp) continue;

                    Vector3 position = pose.position + new Vector3(0, 0.05f, 0);
                    Vector3 cameraForward = Camera.main.transform.forward;
                    cameraForward.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(-cameraForward);

                    captainObject = Instantiate(captainPrefab, position, rotation);
                    hasPlacedCaptain = true;

                    //Hide placement hint
                    if (tapToPlaceText != null)
                        tapToPlaceText.SetActive(false);

                    //Show score text
                    if (scoreText != null)
                        scoreText.SetActive(true);

                    AttachParrotToCaptain();
                    SpawnChestsAround(position);
                    aRPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
                    break;
                }
            }
        }
        else if (!PuzzleUIManager.Instance.IsAnyPuzzleOpen())
        {
            Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ChestInteraction chest = hit.transform.GetComponent<ChestInteraction>();
                if (chest != null) chest.HandleTouch();
            }
        }
    }

    private void AttachParrotToCaptain()
    {
        StartCoroutine(SpawnParrotAfterAnimation());
    }

    private IEnumerator SpawnParrotAfterAnimation()
    {
        //Debug.Log("🔄 Starting SpawnParrotAfterAnimation...");

        Animator captainAnimator = captainObject.GetComponent<Animator>();
        if (captainAnimator == null)
        {
            //Debug.LogWarning("⚠️ No Animator found on captain.");
            yield break;
        }

        while (!captainAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleWithArmT"))
        {
            yield return null;
        }

        //Debug.Log("🎬 Captain reached IdleWithArmT state.");

        Transform headTop = captainObject.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End");
        if (headTop == null)
        {
            Debug.LogWarning("❌ Could not find HeadTop_End bone on captain.");
            yield break;
        }

        parrot = Instantiate(parrotPrefab, headTop);
        if (parrot == null)
        {
            //Debug.LogError("❌ Failed to instantiate parrot.");
            yield break;
        }

        parrot.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        parrot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        parrot.transform.localScale = parrotPrefab.transform.localScale;

        //Debug.Log("🟢 Parrot spawned and positioned on head.");

        AudioSource audio = parrot.GetComponent<AudioSource>();
        if (audio != null && captainParrotClip != null)
        {
            audio.clip = captainParrotClip;
            audio.Play();
            //Debug.Log("🔊 Parrot audio started.");
            yield return new WaitForSeconds(audio.clip.length);
        }
        else
        {
            //Debug.LogWarning("⚠️ No audio source or clip found. Using fallback delay.");
            yield return new WaitForSeconds(3f);
        }

        //Debug.Log("⏳ Audio complete. Scaling down parrot...");

        Vector3 originalScale = parrot.transform.localScale;
        float t = 0f;
        float duration = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            parrot.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        //Debug.Log("🧨 Parrot scaled down (hidden).");

        // Signal animation can continue
        captainObject.GetComponent<CaptainController>().parrotSequenceComplete = true;
        ParrotFinished = true;

        Debug.Log("⏸️ Waiting for WINWIN signal to reactivate parrot...");

        yield return new WaitUntil(() => WINWIN);

        Debug.Log("✅ WINWIN is true — Rescaling parrot...");

        // Rescale back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            parrot.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        Animator animator = parrot.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("WinParrot");
            Debug.Log("🎯 Parrot trigger 'WinParrot' set.");
        }
        else
        {
            Debug.LogWarning("⚠️ Animator not found on parrot.");
        }

        Debug.Log("🟩 Parrot rescaled and WinParrot animation triggered.");
    }





    private void SpawnChestsAround(Vector3 center)
    {
        if (chestPrefabs.Length != 3)
        {
            Debug.LogWarning("You must assign exactly 3 chest prefabs.");
            return;
        }

        List<Vector3> usedPositions = new List<Vector3>();
        float minDistance = 1.0f; // minimum distance required between any two chests
        int maxAttempts = 100;

        for (int i = 0; i < chestPrefabs.Length; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttempts)
            {
                attempts++;

                Vector3 offset = new Vector3(
                    Random.Range(-1.2f, 1.2f), 0, Random.Range(-1.2f, 1.2f)
                );
                Vector3 targetPoint = center + offset;

                if (!TryRaycastToPlane(targetPoint, out Pose chestPose)) continue;

                ARPlane plane = aRPlaneManager.GetPlane(hits[0].trackableId);
                if (plane == null || plane.alignment != PlaneAlignment.HorizontalUp) continue;

                Vector3 chestPos = chestPose.position + Vector3.up * 0.01f;

                // Ensure chestPos is far enough from all others
                bool valid = true;
                foreach (Vector3 existing in usedPositions)
                {
                    if (Vector3.Distance(existing, chestPos) < minDistance)
                    {
                        valid = false;
                        break;
                    }
                }

                if (!valid) continue;

                // Face camera
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 direction = cameraPos - chestPos;
                direction.y = 0f;
                Quaternion faceCamera = Quaternion.LookRotation(direction);

                GameObject chest = Instantiate(chestPrefabs[i], chestPos, faceCamera);

                ChestInteraction chestScript = chest.GetComponent<ChestInteraction>();
                if (chestScript != null)
                {
                    chestScript.chestID = $"chest{i + 1}";
                }

                Vector3 sparkleOffset = new Vector3(0, 0.3f, 0);
                GameObject sparkle = Instantiate(sparklePrefab, chestPos + sparkleOffset, Quaternion.identity);
                sparkle.transform.localScale = sparklePrefab.transform.localScale;

                ChestReveal reveal = chest.GetComponent<ChestReveal>();
                if (reveal != null)
                {
                    reveal.AssignSparkle(sparkle);
                }

                usedPositions.Add(chestPos);
                placed = true;

                Debug.Log($"✅ Chest {i + 1} spawned at: {chestPos} after {attempts} attempts.");
            }

            if (!placed)
            {
                Debug.LogWarning($"⚠️ Failed to place chest {i + 1} after {maxAttempts} attempts.");
            }
        }
    }


    private bool TryRaycastToPlane(Vector3 worldPoint, out Pose pose)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        if (aRRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            return true;
        }

        pose = default;
        return false;
    }
}
