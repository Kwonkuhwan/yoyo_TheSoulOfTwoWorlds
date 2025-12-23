using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{  
    [Header("Emitter")]
    [SerializeField] private bool isUseEmitter = false;
    [SerializeField] private SoundEmitter soundEmitter; // Reference to the SoundEmitter component

    private void Awake()
    {
        if (isUseEmitter)
        {
            if (soundEmitter == null)
            {
                soundEmitter = GetComponent<SoundEmitter>();
            }

            if (soundEmitter == null)
            {
                soundEmitter = GetComponentInChildren<SoundEmitter>();
            }

            if (soundEmitter == null)
            {
                soundEmitter = GetComponentInParent<SoundEmitter>();
            }
        }
    }

    public void PlaySound()
    {
        if(SoundManager.I == null) return;

        SoundManager.I.PlaySFXAt("Crutches Tip", transform.position);

        if (isUseEmitter && soundEmitter != null)
        {
            soundEmitter.Emit();
        }
    }
}
