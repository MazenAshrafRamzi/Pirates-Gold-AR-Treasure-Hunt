using System.Collections;
using UnityEngine;

public class CaptainController : MonoBehaviour
{
    public AudioClip[] voiceClips; // 0: Start, 1: Sparkles, 2: Parok, 3: Final Win
    private Animator animator;
    private AudioSource audioSource;

    private readonly string[] animationTriggers = { "Start", "Sparkles", "Parok" };
    private int currentStateIndex = 0;

    public bool parrotSequenceComplete = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null || audioSource == null)
        {
            Debug.LogError("CaptainController: Animator or AudioSource missing.");
            return;
        }

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        for (int i = 0; i < animationTriggers.Length; i++)
        {
            string trigger = animationTriggers[i];
            AudioClip clip = voiceClips.Length > i ? voiceClips[i] : null;

            animator.SetTrigger(trigger);

            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();

                if (i == 2) // Parok clip – needs mid-play pause
                {
                    // Let the first second play (e.g., “Meet Parok!”)
                    yield return new WaitForSeconds(1.2f);
                    float resumeTime = audioSource.time;

                    audioSource.Pause();

                    Debug.Log("🎤 Captain audio paused. Waiting for parrot...");

                    yield return new WaitUntil(() => parrotSequenceComplete);

                    Debug.Log("✅ Parrot finished. Resuming captain audio...");

                    audioSource.time = resumeTime;
                 
                    audioSource.Play();

                    animator.SetTrigger("AfterParok");

                    yield return new WaitForSeconds(clip.length - resumeTime);
                    animator.SetBool("ParokDone", true);
                }
                else
                {
                    yield return new WaitForSeconds(clip.length);
                }
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            currentStateIndex++;
        }
    }

    public void TriggerFinalWin()
    {
        animator.SetTrigger("Win");

        if (voiceClips.Length > 3)
        {
            audioSource.clip = voiceClips[3];
            audioSource.Play();
        }
    }
}
