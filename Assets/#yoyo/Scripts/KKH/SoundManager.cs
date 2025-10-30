using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;
    public AudioClip[] uiClips;
    public AudioClip[] sfx3dClips;

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> uiDict;
    private Dictionary<string, AudioClip> sfx3dDict;

    private Coroutine bgmFadeCoroutine;

    [Range(0.0f, 1.0f)] public float bgmVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float sfxVolume = 1.0f;
    [Range(0.0f, 1.0f)] public float uiVolume = 1.0f;

    void Awake()
    {
        // ΩÃ±€≈Ê º≥¡§
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM("BGM");
    }

    void InitDictionaries()
    {
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
        SetUIVolume(uiVolume);

        bgmDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();
        uiDict = new Dictionary<string, AudioClip>();
        sfx3dDict = new Dictionary<string, AudioClip>();

        foreach (var clip in bgmClips) bgmDict[clip.name] = clip;
        foreach (var clip in sfxClips) sfxDict[clip.name] = clip;
        foreach (var clip in uiClips) uiDict[clip.name] = clip;
        foreach(var clip in sfx3dClips) sfx3dDict[clip.name] = clip;
    }

    // BGM ¿Áª˝
    public void PlayBGM(string name, bool loop = true, float fadeTime = 1f)
    {
        if (bgmDict.TryGetValue(name, out var clip))
        {
            if (bgmFadeCoroutine != null)
                StopCoroutine(bgmFadeCoroutine);

            bgmFadeCoroutine = StartCoroutine(FadeInBGM(clip, loop, fadeTime));
        }
    }

    public void StopBGM(float fadeTime = 1f)
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        StartCoroutine(FadeOutBGM(fadeTime));
    }
    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayUI(string name)
    {
        if (uiDict.TryGetValue(name, out var clip))
        {
            uiSource.PlayOneShot(clip);
        }
    }

    // ------------------ 3D Sound ------------------

    public void Play3DSound(string name, Transform transform)
    {
        Play3DSound(name, transform.position);
    }

    public void Play3DSound(string name, Vector3 position)
    {
        if (sfx3dDict.TryGetValue(name, out var clip))
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxSource.volume);
        }
    }

    // ------------------ Volume Control ------------------

    public void SetBGMVolume(float volume) => bgmSource.volume = volume;
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;
    public void SetUIVolume(float volume) => uiSource.volume = volume;

    // ------------------ Mute ------------------

    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute;
        sfxSource.mute = mute;
        uiSource.mute = mute;
    }

    // ------------------ Fade In/Out ------------------

    IEnumerator FadeInBGM(AudioClip newClip, bool loop, float duration)
    {
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        bgmSource.volume = bgmVolume;
    }

    IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    // ------------------ Event Trigger Example ------------------

    public void PlaySFXByEvent(string eventName)
    {
        switch (eventName)
        {
            case "PlayerJump":
                PlaySFX("Jump");
                break;
            case "EnemyDeath":
                PlaySFX("Explosion");
                break;
            case "Coin":
                PlaySFX("Coin");
                break;
            default:
                Debug.LogWarning($"No SFX mapped for event: {eventName}");
                break;
        }
    }
}
