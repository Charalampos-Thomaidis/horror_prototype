using UnityEngine;

public class RandomSpawnItem : MonoBehaviour
{
    public Transform item;
    public Transform[] spawnPoints;

    void Start()
    {
        int index = Random.Range(0, spawnPoints.Length);
        item.position = spawnPoints[index].position;
        item.SetParent(spawnPoints[index]);
    }
}
