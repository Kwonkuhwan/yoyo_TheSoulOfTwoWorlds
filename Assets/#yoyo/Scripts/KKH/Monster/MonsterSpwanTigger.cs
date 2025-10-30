using System.Collections.Generic;
using UnityEngine;

public class MonsterSpwanTigger : MonoBehaviour
{
    public GameObject monsterPrefab; // The monster prefab to spawn
    public GameObject monsterObj;
    public bool isPlayerPoint = false; // Flag to check if the player is at the spawn point
    public GameObject Player;
    
    public Transform spawnPoint;

    public List<GameObject> diableObj = new List<GameObject>(); // List of objects to disable when the monster spawns


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            SpawnMonster(); // Call the method to spawn the monster
        }
    }

    private void SpawnMonster()
    {
        if (isPlayerPoint)
        {
            if (monsterObj != null)
            {
                monsterObj.SetActive(true);
                monsterObj.transform.position = Player.transform.position; // Move the existing monster to the spawn point
            }
            else
            {
                if (monsterPrefab != null)
                {
                    // If the player is at the spawn point, instantiate the monster at the spawn point's position and rotation
                    monsterObj = Instantiate(monsterPrefab, Player.transform.position, Player.transform.rotation);
                }
            }
        }
        else
        {
            if (monsterObj != null)
            {
                monsterObj.SetActive(true);
                //monsterObj.transform.position = spawnPoint.position; // Move the existing monster to the spawn point
            }
            else
            {
                if (monsterPrefab != null)
                {
                    // If the player is at the spawn point, instantiate the monster at the spawn point's position and rotation
                    monsterObj = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }

        if(diableObj.Count > 0)
        {
            foreach (GameObject obj in diableObj)
            {
                if (obj != null)
                {
                    obj.SetActive(false); // Disable each object in the list
                }
            }
        }
    }
}
