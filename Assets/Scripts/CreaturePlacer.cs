using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePlacer : MonoBehaviour
{
    public GameObject SpawnArea;
    public List<Creature> CreatureList;
    public int NumberOfCreatures = 5;

    Vector3 spawnPosition;

    private void Start()
    {
        AssignSpawnPosition();
    }

    public void AssignSpawnPosition()
    {
        if (SpawnArea == null || CreatureList.Count == 0)
            return;

        for (int i = 0; i < NumberOfCreatures; ++i)
        {
            //  Grabs a random location in the SpawnArea
            var spawnX = Random.Range(-SpawnArea.transform.localScale.x / 2, SpawnArea.transform.localScale.x / 2);
            var spawnZ = Random.Range(-SpawnArea.transform.localScale.z / 2, SpawnArea.transform.localScale.z / 2);
            spawnPosition = new Vector3(spawnX, 0f, spawnZ);

            //  Picks a random creature in CreatureList
            //var chooseCreature = Random.Range(0, CreatureList.Count);

            //  Start with "Ground Whale" index 1 and evolve it from there
            var chooseCreature = CreatureList[0];

            //  Instantiates that creature at the spawnPosition
            Instantiate(chooseCreature, spawnPosition, Quaternion.identity, this.transform);
        }




    }
}
