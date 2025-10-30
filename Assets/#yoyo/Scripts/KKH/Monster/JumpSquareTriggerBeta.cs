using FronkonGames.Glitches.Artifacts;
using System.Collections;
using UnityEngine;

public class JumpSquareTriggerBeta : MonoBehaviour
{
    [SerializeField] private GameObject Monster;
    [SerializeField] private MonsterJumpSquareBeta monsterJumpSquare;

    [SerializeField] private bool isActive = false;

    private void Awake()
    {
        if (!monsterJumpSquare)
        {
            monsterJumpSquare = Monster.GetComponent<MonsterJumpSquareBeta>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
            monsterJumpSquare.target = other.gameObject.transform;
            monsterJumpSquare.PlayerIn(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            monsterJumpSquare.PlayerIn(false);
            monsterJumpSquare.target = null;
        }
    }
}
