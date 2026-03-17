using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnterBossAreaScript : MonoBehaviour
{
    public CameraController cameraController; // Assign this in Inspector
    // boss
    [SerializeField] private GameObject boss; // Assign the boss prefab in Inspector
    // sound manager 
    [SerializeField] private GameObject soundManager; // Assign the sound manager prefab in Inspector
    // doors
    [SerializeField] private GameObject door1; // Assign the door prefab in Inspector
    [SerializeField] private GameObject door2; // Assign the door prefab in Inspector
    [FormerlySerializedAs("_enterDoorAnimator")] [SerializeField]
    private Animator enterDoorAnimator;
    [FormerlySerializedAs("_exitDoorAnimator")] [SerializeField] 
    private Animator exitDoorAnimator;
    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");
    private bool _bossEncounterStarted;

    public int targetSize = 3; // Zoom value for boss area
    public float transitionTime = 1f; // Duration of zoom

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _bossEncounterStarted) return;

        _bossEncounterStarted = true;
        Debug.Log("Entered Boss Area");
        if (cameraController != null)
        {
            Debug.Log("Calling ZoomTo()");
            cameraController.ZoomTo(targetSize, transitionTime);
            // activate boss music or other effects here
            if (soundManager != null)
            {
                // Assuming the SoundManager has a method to play boss music
                soundManager.GetComponent<SoundManager>().PlayBossMusic();
            }
            else
            {
                Debug.LogWarning("SoundManager is not assigned.");
            }
            // Wake up the boss
            if (boss != null)
            {
                boss.SetActive(true);
                var bossScript = boss.GetComponent<BossScript>();
                if (bossScript != null) bossScript.WakeUp();
            }
            else
            {
                Debug.LogWarning("Boss prefab is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("CameraController reference is null.");
        }
        // Close the doors
        if (door1 != null && door2 != null)
        {
            enterDoorAnimator.SetTrigger(Close);
            exitDoorAnimator.SetTrigger(Open);
            /*                door1.SetActive(false);
                             door2.SetActive(false);*/
        }
        else
        {
            Debug.LogWarning("Door prefabs are not assigned.");
        }
    }
}
