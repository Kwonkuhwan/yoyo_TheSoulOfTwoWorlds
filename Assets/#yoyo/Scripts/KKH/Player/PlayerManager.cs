using BNG;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private string strNextSceneName = "MainScene"; // Name of the next scene to load after death

    [SerializeField] private BNGPlayerController playerController; // Reference to the BNGPlayerController component
    [SerializeField] private SmoothLocomotion locomotion; // Reference to the SmoothLocomotion component

    [SerializeField] private Transform trMonsterShow;

    public GameObject faderObj;

    public bool isPlayerDie = false;
    public bool isStern = false;

    public GameObject bookCanvas;
    [SerializeField] private float bookOpenCoolTime = 20.0f; // Delay before the book opens

    [SerializeField] private WallOpenTrigger wallOpenTrigger; // Reference to the WallOpenTrigger component

    private void Awake()
    {
        playerController = GetComponent<BNGPlayerController>(); // Ensure the BNGPlayerController component is attached
        locomotion = GetComponent<SmoothLocomotion>();
        isPlayerDie = false;
    }

    public void Die()
    {
        StartCoroutine(IEDie());
    }

    private IEnumerator IEDie()
    {
        isPlayerDie = true;

        //locomotion.MovementSpeed = 0.0f;
        locomotion.UpdateMovement = false; // Disable movement

        trMonsterShow.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        PlayerFader fader = faderObj.GetComponent<PlayerFader>();
        fader.DoFadeIn();

        StartCoroutine(NextSceneLoad()); // Start the coroutine to load the next scene after a delay
    }

    public void Stern()
    {
        StartCoroutine(IEStern());
    }

    private IEnumerator IEStern()
    {
        isStern = true;
        Debug.Log("PlayerManager: Stern called, player is dying.");
        locomotion.MovementSpeed = 0.0f;
        PlayerFader fader = faderObj.GetComponent<PlayerFader>();
        fader.DoFadeIn();
        yield return new WaitForSeconds(3.0f);
        fader.DoFadeOut();
        locomotion.MovementSpeed = 2.0f;
        //locomotion.UpdateMovement = true;
        isStern = false;
    }

    IEnumerator NextSceneLoad()
    {
        yield return new WaitForSeconds(3f); // Wait for 1 second before loading the next scene
        GameManager.Instance.LoadNextScene(strNextSceneName); // Call the GameManager to load the next scene
    }

    public void OpenBook()
    { 
        StartCoroutine(IEOpenBook()); // Start the coroutine to handle the book opening process
    }

    private IEnumerator IEOpenBook()
    {
        bookCanvas.SetActive(true); // Activate the book canvas to show the book UI
        
        yield return new WaitForSeconds(bookOpenCoolTime); // Wait for 3 seconds before closing the book

        CloseBook();
    }

    public void CloseBook()
    {
        bookCanvas.SetActive(false); // Deactivate the book canvas to hide the book UI
        wallOpenTrigger.isBookOpen = true;
    }
}
