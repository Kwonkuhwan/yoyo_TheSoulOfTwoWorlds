using FronkonGames.Glitches.Artifacts;
using System.Collections;
using UnityEngine;

public class JumpSquareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject Monster;
    [SerializeField] private MonsterJumpSquare monsterJumpSquare;

    [SerializeField] private bool isActive = false;

    [SerializeField] private float fTimer = 0f;
    [SerializeField] private float fMaxtimer = 5.0f;

    [SerializeField] private GameObject player;

    private void Awake()
    {
        if (!monsterJumpSquare)
        {
            monsterJumpSquare = Monster.GetComponent<MonsterJumpSquare>();
        }
    }

    private void Update()
    {
        if (isActive && fTimer < fMaxtimer)
        {
            fTimer += Time.deltaTime;
            if (fTimer >= fMaxtimer)
            {
                monsterJumpSquare.target = player.transform;
                monsterJumpSquare.PlayerIn(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            fTimer = 0f;
            monsterJumpSquare.PlayerIn(false);
            player = null;
        }
    }
}
