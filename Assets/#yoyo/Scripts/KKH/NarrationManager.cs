using System.Collections;
using TMPro;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    [SerializeField] private string[] strNarration;
    [SerializeField] private float[] fNarrationTime;
    [SerializeField] private TMP_Text text_Narration;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource  = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(NarrationStart());
    }

    private IEnumerator NarrationStart()
    {
        for(int i =0; i< strNarration.Length; i++)
        {
            text_Narration.text = strNarration[i];
            yield return new WaitForSeconds(fNarrationTime[i]);
        }

        audioSource.playOnAwake = false;
        audioSource.enabled = false;
        gameObject.SetActive(false);
    }
}
