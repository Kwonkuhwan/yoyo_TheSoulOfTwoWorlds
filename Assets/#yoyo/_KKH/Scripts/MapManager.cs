using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static Oculus.Interaction.Context;

public class MapManager : MonoBehaviour
{
    #region 추가 : 권구환 - 20251106
    public static MapManager instance;

    [Header("시작")]
    public bool isStart = false;

    [Header("맵")]
    [SerializeField] private Volume volumePrefab;
    [SerializeField] private VolumeProfile volumeNight;
    [SerializeField] private VolumeProfile volumeDay;
    [SerializeField] private GameObject map1;
    [SerializeField] private GameObject map2;

    [SerializeField] private bool bSound = false;
    [SerializeField] private bool bFade = false;
    [SerializeField] private float fChangeFade = 5.0f;
    [SerializeField] private float fChangeDelay = 60.0f;
    [SerializeField] private float fChangeDelay2 = 300.0f;
    [SerializeField] private float fTimer = 0.0f;
    #endregion

    #region 추가 : 권구환 - 20251106
    public void MapChange()
    {
        OVRScreenFade.instance.fadeTime = 5.0f;

        OVRScreenFade.instance.FadeIn();

        Flashlight.instance.Toggle();

        if (map1 && map1.activeInHierarchy)
        {
            map2.SetActive(true);
            map1.SetActive(false);
            volumePrefab.profile = volumeNight;
        }
        else if (map2 && map2.activeInHierarchy)
        {
            map1.SetActive(true);
            map2.SetActive(false);
            volumePrefab.profile = volumeDay;
        }

        fTimer = 0.0f;
        bSound = false;
    }
    #endregion

    private IEnumerator FadeInOut()
    {
        if (!bSound)
        {
            bSound = true;
            SoundManager.I.PlaySFX("World_Change");
            Debug.Log("World Change Sound Play");
        }

        OVRScreenFade.instance.fadeTime = 0.5f;

        bFade = true;
        OVRScreenFade.instance.FadeIn();
        yield return new WaitForSeconds(0.5f);
        OVRScreenFade.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        bFade = false;
    }

    private void Awake()
    {
        instance = this;

        fTimer = 0.0f;
        bSound = false;
        bFade = false;

        volumePrefab.profile = volumeDay;
    }

    private void Update()
    {
        #region 추가 : 권구환 - 20251106
        if (isStart)
        {
            if (map1 && map1.activeInHierarchy)
            {
                fTimer += Time.deltaTime;

                if (!bFade && fChangeFade >= fChangeDelay - fTimer)
                {
                    StartCoroutine(FadeInOut());
                }

                if (fTimer >= fChangeDelay)
                {
                    MapChange();
                }
            }
            else if (map2 && map2.activeInHierarchy)
            {
                fTimer += Time.deltaTime;

                if (!bFade && fChangeFade >= fChangeDelay2 - fTimer)
                {
                    StartCoroutine(FadeInOut());
                }

                if (fTimer >= fChangeDelay2)
                {
                    MapChange();
                }
            }
        }
        #endregion
    }
}
