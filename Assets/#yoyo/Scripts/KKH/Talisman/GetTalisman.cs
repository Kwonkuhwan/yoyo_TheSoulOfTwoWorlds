using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//2025-07-17 OJY
public class GetTalisman : MonoBehaviour
{
    [SerializeField] private bool isJumpSquare;
    [SerializeField] private bool isWall; // If true
    [SerializeField] private bool isDoor; // If true, the talisman will open a
    TalismanManager talismanManager;

    Collider talismanCollider;
    MeshRenderer talismanMeshRenderer;

    [SerializeField] private Animator doorAnimator;
    [SerializeField] private List<Animator> wallAnimators;
    [SerializeField] private List<Animator> doorAnimators;

    [SerializeField] private GameObject goNotion;
    [SerializeField] private string notionText = "";

    private void Awake()
    {
        talismanMeshRenderer = GetComponent<MeshRenderer>();
        talismanCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        talismanManager = TalismanManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            talismanManager.CountRenew(ChangeType.increase);

            SoundManager.Instance.Play3DSound("GetTalisman", transform.position);

            if (isWall)
            {
                if (wallAnimators.Count > 0)
                {
                    foreach (var wallAnimator in wallAnimators)
                    {
                        if (wallAnimator != null)
                        {
                            wallAnimator.SetTrigger("Open");
                            SoundManager.Instance.Play3DSound("벽 움직이는 소리", wallAnimator.transform);
                        }
                    }
                    StartCoroutine(Naration()); // Start the narration coroutine
                }

            }

            if (isDoor)
            {
                if (doorAnimators.Count > 0)
                {
                    foreach (var doorAnimator in doorAnimators)
                    {
                        if (doorAnimator != null)
                        {
                            doorAnimator.SetTrigger("Open");
                            SoundManager.Instance.Play3DSound("DoorOpen", doorAnimator.transform);
                        }
                    }
                    StartCoroutine(Naration()); // Start the narration coroutine
                }
            }

            if (isJumpSquare)
            {
                talismanCollider.enabled = false; // Disable the collider to prevent multiple triggers
                talismanMeshRenderer.enabled = false; // Hide the talisman mesh renderer
                StartCoroutine(JumpSquareStart()); // Start the coroutine to handle the jump square logic
            }

            if(!isJumpSquare && !isWall && !isDoor)
            {
                Destroy(gameObject); // Destroy the talisman object immediately if not a jump square, wall, or door
            }
        }
    }

    private IEnumerator JumpSquareStart()
    {
        doorAnimator.SetTrigger("Close");

        SoundManager.Instance.Play3DSound("귀신 소리1", transform.position);
        SoundManager.Instance.Play3DSound("남자 광기3", transform.position);
        SoundManager.Instance.Play3DSound("젊은 여자 웃음소리", transform.position);

        yield return new WaitForSeconds(3.5f);

        doorAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(0.5f);
       
        Destroy(gameObject); // Destroy the talisman object after the animation
    }

    private IEnumerator Naration()
    {
        if (!string.IsNullOrEmpty(notionText))
        {
            if (!goNotion.activeInHierarchy)
            {
                goNotion.SetActive(true);
            }
            goNotion.GetComponentInChildren<TMP_Text>().text = notionText;

        }
        yield return new WaitForSeconds(3.0f);
        goNotion.SetActive(false);

        if (!isJumpSquare)
        {
            Destroy(gameObject); // Destroy the talisman object after the narration
        }
    }
}
