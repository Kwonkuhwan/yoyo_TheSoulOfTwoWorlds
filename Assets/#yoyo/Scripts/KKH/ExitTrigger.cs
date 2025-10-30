using BNG;
using System.Collections;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    BNGPlayerController playerController;
    PlayerManager playerManager;
    [SerializeField] private GameObject Monster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<BNGPlayerController>();
            playerManager = other.GetComponent<PlayerManager>();
            StartCoroutine(IEExit());
        }
    }

    IEnumerator IEExit()
    {
        GameManager.Instance.ReSetArtifacts();

        Monster.SetActive(false);

        PlayerFader fader = playerManager.faderObj.GetComponent<PlayerFader>();
        fader.DoFadeIn();

        yield return new WaitForSeconds(3.0f);
        GameManager.Instance.LoadNextScene("4.EndTitle"); // Call the GameManager to load the next scene
    }
}
