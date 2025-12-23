using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundData
{
    public string id;                 // 고유 키 (예: "Footstep", "Door", "BGM_Main")
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    [Tooltip("3D 공간감 적용(=1), 2D UI 사운드(=0)")]
    [Range(0f, 1f)] public float spatialBlend = 0f;
    [Tooltip("SFX 전용은 보통 false, BGM 전용은 true")]
    public bool loop = false;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private bool isDestroyOnLoad = false;

    // ==== Singleton ==========================================================
    public static SoundManager I { get; private set; }

    [Header("🎧 사운드 목록(List)")]
    public List<SoundData> sounds = new List<SoundData>();

    [Header("🔊 풀링(OneShot 3D)")]
    public int sfxPoolSize = 8;

    [Header("🎼 BGM 설정")]
    public float bgmCrossFadeSeconds = 0.8f;

    // 런타임
    Dictionary<string, SoundData> map;
    AudioSource bgmA, bgmB;     // 크로스페이드용 2트랙
    bool bgmUsingA = true;

    List<AudioSource> sfxPool = new List<AudioSource>();

    // 볼륨 (0~1), PlayerPrefs 저장 키
    const string KEY_MASTER = "vol_master";
    const string KEY_BGM = "vol_bgm";
    const string KEY_SFX = "vol_sfx";

    float _master = 1f, _bgm = 1f, _sfx = 1f;

    void Awake()
    {
        // Singleton
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        if (isDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Map 빌드
        map = new Dictionary<string, SoundData>();
        foreach (var s in sounds)
        {
            if (s == null || s.clip == null || string.IsNullOrEmpty(s.id)) continue;
            if (!map.ContainsKey(s.id)) map.Add(s.id, s);
        }

        // BGM 채널 2개 (크로스페이드용)
        bgmA = gameObject.AddComponent<AudioSource>();
        bgmB = gameObject.AddComponent<AudioSource>();
        foreach (var a in new[] { bgmA, bgmB })
        {
            a.playOnAwake = false;
            a.loop = true;
            a.spatialBlend = 0f; // BGM은 2D
        }

        // SFX 풀 생성
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var go = new GameObject($"SFX_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            sfxPool.Add(src);
        }

        // 볼륨 로드 & 반영
        _master = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        _bgm = PlayerPrefs.GetFloat(KEY_BGM, 1f);
        _sfx = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        _ApplyVolumes();
    }

    private void Start()
    {
        PlayBGM("BGM");
    }

    // ===================== Public API =======================================

    // --- 볼륨 ---
    public void SetMasterVolume(float v) { _master = Mathf.Clamp01(v); SaveVolumes(); _ApplyVolumes(); }
    public void SetBgmVolume(float v) { _bgm = Mathf.Clamp01(v); SaveVolumes(); _ApplyVolumes(); }
    public void SetSfxVolume(float v) { _sfx = Mathf.Clamp01(v); SaveVolumes(); _ApplyVolumes(); }

    public float GetMasterVolume() => _master;
    public float GetBgmVolume() => _bgm;
    public float GetSfxVolume() => _sfx;

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat(KEY_MASTER, _master);
        PlayerPrefs.SetFloat(KEY_BGM, _bgm);
        PlayerPrefs.SetFloat(KEY_SFX, _sfx);
        PlayerPrefs.Save();
    }

    // --- BGM ---
    public void PlayBGM(string id)
    {
        if (!TryGet(id, out var d))
        {
            Debug.LogWarning($"[SoundManager] BGM '{id}' not found");
            return;
        }
        StartCoroutine(CoCrossFadeBGM(d));
    }

    public void StopBGM(float fadeSeconds = 0.5f)
    {
        StartCoroutine(CoFadeOutAllBGM(fadeSeconds));
    }

    // --- SFX (2D/3D) ---
    public void PlaySFX(string id)
    {
        if (!TryGet(id, out var d)) return;

        Debug.Log($"PlaySFX: {id}");

        var src = GetFreeSFX();
        SetupSfxSource(src, d);
        src.transform.position = Camera.main ? Camera.main.transform.position : Vector3.zero; // 2D 기준
        src.spatialBlend = d.spatialBlend; // 0이면 완전 2D
        src.Play();
    }

    public void PlaySFXAt(string id, Vector3 worldPos)
    {
        if (!TryGet(id, out var d)) return;

        var src = GetFreeSFX();
        SetupSfxSource(src, d);
        src.transform.position = worldPos;
        // 3D 재생이 목적이라면 spatialBlend 1 권장
        src.spatialBlend = Mathf.Max(d.spatialBlend, 1f);
        // 3D 기본 설정(필요 시 조정)
        src.minDistance = 2f;
        src.maxDistance = 25f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.Play();
    }

    public void StopAllSFX()
    {
        foreach (var s in sfxPool) s.Stop();
    }

    // --- 유틸 ---
    public bool Has(string id) => map.ContainsKey(id);

    // ===================== Internal =========================================

    bool TryGet(string id, out SoundData d)
    {
        if (!map.TryGetValue(id, out d) || d.clip == null)
        {
            Debug.LogWarning($"[SoundManager] '{id}' not found or clip missing");
            return false;
        }
        return true;
    }

    AudioSource GetFreeSFX()
    {
        foreach (var s in sfxPool)
            if (!s.isPlaying) return s;
        return sfxPool[0]; // 다 차면 0번 재활용
    }

    void SetupSfxSource(AudioSource src, SoundData d)
    {
        src.clip = d.clip;
        src.volume = d.volume * _sfx * _master;
        src.pitch = d.pitch;
        src.loop = d.loop; // 보통 false
    }

    IEnumerator CoCrossFadeBGM(SoundData d)
    {
        var from = bgmUsingA ? bgmA : bgmB;
        var to = bgmUsingA ? bgmB : bgmA;
        bgmUsingA = !bgmUsingA;

        to.clip = d.clip;
        to.loop = true;
        to.volume = 0f;
        to.Play();

        float dur = Mathf.Max(0.01f, bgmCrossFadeSeconds);
        float t = 0f;
        float fromStart = from.volume;
        float toTarget = d.volume * _bgm * _master;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, toTarget, k);
            yield return null;
        }

        from.Stop();
        from.volume = 0f;
        to.volume = toTarget;
    }

    IEnumerator CoFadeOutAllBGM(float dur)
    {
        var a0 = bgmA.volume;
        var b0 = bgmB.volume;
        dur = Mathf.Max(0.01f, dur);
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            bgmA.volume = Mathf.Lerp(a0, 0f, k);
            bgmB.volume = Mathf.Lerp(b0, 0f, k);
            yield return null;
        }
        bgmA.Stop(); bgmB.Stop();
        bgmA.volume = 0f; bgmB.volume = 0f;
    }

    void _ApplyVolumes()
    {
        // BGM 채널(현재/대기 둘 다)
        foreach (var a in new[] { bgmA, bgmB })
        {
            if (a == null) continue;
            if (a.clip == null) { a.volume = 0f; continue; }
            // clip별 개별 volume은 SoundData.volume로 관리하므로 여기선 상한만
            a.volume = Mathf.Clamp01(_bgm * _master);
        }

        // 이미 재생 중인 SFX도 즉시 반영하려면:
        foreach (var s in sfxPool)
        {
            if (s.isPlaying)
                s.volume = Mathf.Clamp01(s.volume * _master); // 간단 반영
        }
    }
}
